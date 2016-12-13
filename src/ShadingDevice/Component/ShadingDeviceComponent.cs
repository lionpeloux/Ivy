using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using ShadingDevice.Kernel;
using IvyCore.Parametric;
using IvyGh.Type;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper;
using System.Globalization;
using System.Data.SQLite;

namespace ShadingDevice
{
    public class ShadingDeviceComponent : GH_Component
    {

        public ShadingDeviceComponent()
          : base("Abaqus Builder", "Builder", "Build .inp and .py file for abaqus analysis", "Ivy", "Shading Device")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{64906d57-b708-41e2-b685-b9858534e4b7}"); }
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Working Directory", "wd", "Path to working directory.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Shape Grid", "grid_shape", "The shape grid.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Actuation Grid", "grid_actu", "The actuation grid.", GH_ParamAccess.item);

            pManager.AddGenericParameter("Node", "N", "The node to build the problem for.", GH_ParamAccess.list);
            pManager.AddMeshParameter("Shell Mesh", "M", "Mesh that represents the shell part.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("First Actuator", "A1", "Pair of shell nodes for the first actuator.", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Second Actuator", "A2", "Pair of shell nodes for the first actuator.", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Boundary Condition", "BC", "List of shell nodes or boundary condition.", GH_ParamAccess.tree);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("out", "out", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var path_wd = "";

            var ghActuationGrid = new GH_Grid();
            var ghShapeGrid = new GH_Grid();

            var ghNode_list = new List<GH_Node>();
            var mesh_list = new List<Mesh>();
            var A1_tree = new GH_Structure<GH_Integer>();
            var A2_tree = new GH_Structure<GH_Integer>();
            var BC_tree = new GH_Structure<GH_Integer>();


            if (!DA.GetData(0, ref path_wd)) { return; }
            if (!DA.GetData(1, ref ghShapeGrid)) { return; }
            if (!DA.GetData(2, ref ghActuationGrid)) { return; }

            if (!DA.GetDataList(3,  ghNode_list)) { return; }
            if (!DA.GetDataList(4,  mesh_list)) { return; }

            if (!DA.GetDataTree(5, out A1_tree)) { return; }
            if (!DA.GetDataTree(6, out A2_tree)) { return; }
            if (!DA.GetDataTree(7, out BC_tree)) { return; }

            string workingDir = Path.GetDirectoryName(path_wd.ToString());
            string filename = Path.GetFileName(path_wd.ToString());

            int n = ghNode_list.Count;
            var inp_tree = new DataTree<string>();

            var grid_actu = ghActuationGrid.Value;
            var grid_shape = ghShapeGrid.Value;
            var culture = new CultureInfo("en-GB");

            for (int i = 0; i < n; i++)
            {
                var node = ghNode_list[i].Value;
                var mesh = mesh_list[i];
                var A1 = A1_tree[i].ConvertAll<int>(ghInt => ghInt.Value);
                var A2 = A2_tree[i].ConvertAll<int>(ghInt => ghInt.Value);
                var BC = BC_tree[i].ConvertAll<int>(ghInt => ghInt.Value);
                
                // HEADER
                var localDate = DateTime.Now.ToString(culture);
                var header = new List<string>();
                header.Add("**Author : Lionel du Peloux");
                header.Add("**email : lionel.dupeloux@gmail.com");
                header.Add("**date : " + localDate);
                header.Add("**node : " + node.Index.ToString("D2"));

                // CREATE INP BUILDER
                var vertexList = AbaqusProblemBuilder.MeshVerticesToArray(mesh);
                var faceList = AbaqusProblemBuilder.MeshFacesToArray(mesh);
                var builder = new AbaqusProblemBuilder(vertexList, faceList, A1, A2, BC);

                // CREATE INP CONTENT
                var inp_str = builder.GetInp(header);
                inp_tree.AddRange(inp_str, new GH_Path(i));

                // CREATE SUBDIR FOR INP FILE
                var subDir = Path.Combine(workingDir, node.Index.ToString("D2"));
                Directory.CreateDirectory(subDir);

                // WRITE INP FILE
                var filePath = subDir + "\\" + "model" + ".inp";
                File.WriteAllLines(filePath, inp_str);
                File.Move(filePath, Path.ChangeExtension(filePath, ".inp"));
            }

            // CLEAN FOLDER
            try
            {
                Array.ForEach(Directory.GetFiles(workingDir), File.Delete);
            }
            catch (Exception)
            {
            }
            

            // WRITE DB FILE
            var dbPath = workingDir + "\\" + "data2.db";
            SQLiteConnection.CreateFile(dbPath);

            var connection_str = "Data Source = " + dbPath + ";" + "Version = " + 3;
            var connection = new SQLiteConnection(connection_str);
            connection.Open();

            string sql;
            SQLiteCommand command;

            // TOPOLOGY TABLE
            sql = "CREATE TABLE TOPO_SHELL (FACE INT, N1 INT, N2 INT, N3 INT, N4 INT)";
            command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            var topo = mesh_list[0];
            for (int i = 0; i < topo.Faces.Count; i++)
            {
                var face = topo.Faces[i];
                sql = String.Format(
                        "INSERT INTO TOPO_SHELL (FACE, N1,  N2,  N3,  N4) " +
                        "VALUES ({0},{1},{2},{3},{3})",
                        i + 1,
                        face[0] + 1,
                        face[1] + 1,
                        face[2] + 1,
                        face[3] + 1
                    );
                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }

            SqlWriteGrid(grid_actu, "ACT", connection);
            SqlWriteGrid(grid_shape, "SHP", connection);
            SqlWriteGrid(grid_actu * grid_shape, "GLB", connection);




            connection.Close();
            //    System.IO.Directory.CreateDirectory(folderPath);

            //var actuGrid = (Grid)grid_actu;

            //// DELETE ALL SUBDIRS FROM WKDIR
            //foreach (string path in Directory.GetDirectories(workingDir))
            //{
            //    Directory.Delete(path, true);
            //}

            //for (int i = 0; i < N.Count; i++)
            //{
            //    
            //    Print("" + i);
            //}

            DA.SetDataTree(0, inp_tree);

        }


        public void SqlWriteGrid(Grid grid, string name, SQLiteConnection connection)
        {
            // NODES GRID
            var sql = "CREATE TABLE "+ "GNODES_" + name + " (NODE INT, TUPLE TEXT, COORD TEXT)";
            var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            for (int i = 0; i < grid.NodeCount; i++)
            {
                var node = grid.Nodes[i];
                sql = String.Format(
                        "INSERT INTO " + "GNODES_" + name + " (NODE, TUPLE, COORD) " +
                        "VALUES ({0},\"{1}\",\"{2}\")",
                        node.Index + 1,
                        node.Tuple.ToString(),
                        node.ToString()
                    );
                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }

            sql = "CREATE TABLE " + "GRID_" + name + " (DIM INT, RANGE TEXT)";
            command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
            for (int i = 0; i < grid.Dim; i++)
            {
                var range = grid.Data[i];
                sql = String.Format(
                        "INSERT INTO " + "GRID_" + name + " (DIM, RANGE) " +
                        "VALUES ({0},\"{1}\")",
                        i + 1,
                        ITuple.ToString(range)
                    );
                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }

    }
}
