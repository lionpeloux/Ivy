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
    public class Comp_ProblemBuilder : GH_Component
    {

        public Comp_ProblemBuilder()
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

            CleanWorkingDir(workingDir);

            int n = ghNode_list.Count;
            var inp_tree = new DataTree<string>();

            var grid_actu = ghActuationGrid.Value;
            var grid_shape = ghShapeGrid.Value;
            var grid_global = grid_actu * grid_shape;
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
                header.Add("Author : Lionel du Peloux");
                header.Add("email : lionel.dupeloux@gmail.com");
                header.Add("date : " + localDate);
                header.Add("node : " + node.Index.ToString("D2"));

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

            // CREATE PY CONTENT
            var py_str2 = AbaqusProblemBuilder.GetPy(workingDir, grid_actu);

            // WRITE PY FILE
            var filePath2 = workingDir + "\\" + "model.py";
            File.WriteAllLines(filePath2, py_str2);
            File.Move(filePath2, Path.ChangeExtension(filePath2, ".py"));

            // WRITE DB FILE
            var dbPath = workingDir + "\\" + "data.db";
            try
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            catch (Exception)
            {
            }

            using (var connection = new SQLiteConnection("Data Source = " + dbPath))
            {         
                connection.Open();

                using (var cmd = new SQLiteCommand(connection))
                {
                    cmd.Transaction = connection.BeginTransaction();

                   
                    // FIELD ACTU
                    cmd.CommandText = "CREATE TABLE FIELD (ACT INT, SHP INT, NODE INT, X REAL, Y REAL, Z REAL, DX REAL, DY REAL, DZ REAL)";
                    cmd.ExecuteNonQuery();

                    // GRIDS
                    SqlWriteGrid(grid_actu, "ACT", cmd);
                    SqlWriteGrid(grid_shape, "SHP", cmd);
                    SqlWriteGrid(grid_global, "GLB", cmd);

                    // SHELL TOPO
                    SqlWriteShellTopology(mesh_list[0], cmd);

                    SqlWriteShapeField(grid_shape.Nodes, mesh_list, cmd);
                    cmd.Transaction.Commit();
                }
                connection.Close();
            }

            DA.SetDataTree(0, inp_tree);

        }


        public void SqlWriteGrid(Grid grid, string name, SQLiteCommand cmd)
        {
            // NODES GRID
            cmd.CommandText = "CREATE TABLE " + "GNODE_" + name + " (NODE INT, TUPLE TEXT, COORD TEXT)";
            cmd.ExecuteNonQuery();

            for (int i = 0; i < grid.NodeCount; i++)
            {
                var node = grid.Nodes[i];
                cmd.CommandText = String.Format(
                        "INSERT INTO " + "GNODE_" + name + " (NODE, TUPLE, COORD) " +
                        "VALUES ({0},\"{1}\",\"{2}\")",
                        node.Index + 1,
                        node.Tuple.ToString(),
                        node.ToString()
                    );
                cmd.ExecuteNonQuery();
            }

            cmd.CommandText = "CREATE TABLE " + "GRID_" + name + " (DIM INT, LABEL TEXT, RANGE TEXT)";
            cmd.ExecuteNonQuery();

            for (int i = 0; i < grid.Dim; i++)
            {
                var range = grid.Data[i];
                cmd.CommandText = String.Format(
                        "INSERT INTO " + "GRID_" + name + " (DIM, LABEL, RANGE) " +
                        "VALUES ({0},\"{1}\",\"{2}\")",
                        i + 1,
                        grid.Labels[i],
                        ITuple.ToString(range)
                    );
                cmd.ExecuteNonQuery();
            }
        }
        public void SqlWriteShellTopology(Mesh mesh, SQLiteCommand cmd)
        {
            // TOPOLOGY TABLE
            cmd.CommandText = "CREATE TABLE TOPO_SHELL_FACE (FACE INT, N1 INT, N2 INT, N3 INT, N4 INT)";
            cmd.ExecuteNonQuery();

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                var face = mesh.Faces[i];
                cmd.CommandText = String.Format(
                        "INSERT INTO TOPO_SHELL_FACE (FACE, N1,  N2,  N3,  N4) " +
                        "VALUES ({0},{1},{2},{3},{4})",
                        i + 1,
                        face[0] + 1,
                        face[1] + 1,
                        face[2] + 1,
                        face[3] + 1
                    );
                cmd.ExecuteNonQuery();
            }

            // TOPOLOGY TABLE
            cmd.CommandText = "CREATE TABLE TOPO_SHELL_NODE (NODE INT)";
            cmd.ExecuteNonQuery();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                cmd.CommandText = String.Format(
                        "INSERT INTO TOPO_SHELL_NODE (NODE) " +
                        "VALUES ({0})",
                        i + 1
                    );
                cmd.ExecuteNonQuery();
            }
        }
        public void SqlWriteShapeField(IList<Node> nodes, IList<Mesh> meshes, SQLiteCommand cmd)
        {
            //CREATE TABLE
            cmd.CommandText = "CREATE TABLE FIELD_SHP (SHAPE INT, NODE INT, X REAL, Y REAL, Z REAL)";
            cmd.ExecuteNonQuery();

            // POPULATE EACH FIELD FOR EACH NODE 
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var mesh = meshes[i];

                for (int j = 0; j < mesh.Vertices.Count; j++)
                {
                    var pt = mesh.Vertices[j];
                    cmd.CommandText = String.Format(
                        "INSERT INTO " + "FIELD_SHP" + " (SHAPE, NODE, X, Y, Z) " +
                        "VALUES ({0},{1},{2},{3},{4})",
                        node.Index + 1,
                        j+1,
                        pt.X,
                        pt.Y,
                        pt.Z
                    );
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CleanWorkingDir(string dirPath)
        {
            // CLEAN FOLDER
            try
            {
                Array.ForEach(Directory.GetFiles(dirPath), File.Delete);
            }
            catch (Exception)
            {
            }

            // DELETE ALL SUBDIRS FROM WKDIR
            foreach (string path in Directory.GetDirectories(dirPath))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
