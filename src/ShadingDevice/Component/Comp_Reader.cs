using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using System.IO;
using IvyGh.Type;
using Grasshopper.Kernel.Data;
using Grasshopper;
using System.Data.SQLite;
using IvyCore.MultiDimGrid;

namespace ShadingDevice
{
    public class Comp_Reader : GH_Component
    {

        public Comp_Reader()
          : base("Data Base Reader", "Reader", "Read a data.db file and display results", "Ivy", "Shading Device")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{13a0da4f-6ca6-4b73-b789-4d245c98b3b7}"); }
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("DataBase Directory", "wd", "Path to working directory.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("out", "out", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("Global Grid", "G_glb", "Combined Grid for all parameters.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Actuation Partial Grid", "G_act", "Grid for actuation parameters.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Shape Partial Grid", "G_shp", "Grid for shape parameters.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Shell Node", "N", "Mesh topology of the shell part (vertices).", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Shell Face", "F", "Mesh topology of the shell part (faces).", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Fields", "Fields", "", GH_ParamAccess.tree);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var path_db = "";
            
            if (!DA.GetData(0, ref path_db)) { return; }

            Grid grid_act;
            Grid grid_shp;
            Grid grid_glb;
            int[][] topo_shell_face;
            int[] topo_shell_node;
            int n_glb, n_act, n_shp, n_shell;
            double[][,] Fields;


            // OPEN DB FILE
            string workingDir = Path.GetDirectoryName(path_db);
            string dbName = Path.GetFileName("data.db");
            string dbPath = workingDir + "\\" + dbName;

            using (var connection = new SQLiteConnection("Data Source = " + dbPath))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand(connection))
                {
                    grid_act = SqlReadGrid("GRID_ACT", cmd);
                    grid_shp = SqlReadGrid("GRID_SHP", cmd);
                    grid_glb = SqlReadGrid("GRID_GLB", cmd);
                    topo_shell_face = SqlReadTopoShellFace("TOPO_SHELL_FACE", cmd);
                    topo_shell_node = SqlReadTopoShellNode("TOPO_SHELL_NODE", cmd);

                    n_glb = grid_glb.NodeCount;
                    n_act = grid_act.NodeCount;
                    n_shp = grid_shp.NodeCount;
                    n_shell = topo_shell_node.Length;

                    // déclaration des champs vectoriels
                    // un champ vertcoriel par noeud parametric global            
                    Fields = new double[n_shell][,];

                    // ALLOCATION
                    for (int i = 0; i < n_shell; i++)
                    {
                        // X,Y,Z,DX,DY,DZ for each global parametric node
                        Fields[i] = new double[n_glb, 6];
                    }

                    cmd.CommandText = "select * from FIELD";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var nodeActIndex = reader.GetInt32(0)-1;
                            var nodeShpIndex = reader.GetInt32(1)-1;

                            var tuple_act = grid_act.Nodes[nodeActIndex].Address.Value;
                            var tuple_shp = grid_shp.Nodes[nodeShpIndex].Address.Value;
                            var tuple_glb = IAddress.CartesianProduct(tuple_act, tuple_shp);


                            var nodeGlbIndex = grid_glb.NodeIndex(tuple_glb);

                            var nodeShell = reader.GetInt32(2) - 1;

                            Fields[nodeShell][nodeGlbIndex, 0] = reader.GetDouble(3);
                            Fields[nodeShell][nodeGlbIndex, 1] = reader.GetDouble(4);
                            Fields[nodeShell][nodeGlbIndex, 2] = reader.GetDouble(5);

                            Fields[nodeShell][nodeGlbIndex, 3] = reader.GetDouble(6);
                            Fields[nodeShell][nodeGlbIndex, 4] = reader.GetDouble(7);
                            Fields[nodeShell][nodeGlbIndex, 5] = reader.GetDouble(8);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }

            var topo_tree = new DataTree<int>();
            for (int i = 0; i < topo_shell_face.Length; i++)
            {
                topo_tree.AddRange(topo_shell_face[i], new GH_Path(i));
            }

            var field_tree = new DataTree<double>();
            for (int i = 0; i < n_shell; i++)
            {
                for (int j = 0; j < n_glb; j++)
                {
                    var values = new double[6];
                    var path = new GH_Path(new int[2] { i, j });
                    for (int k = 0; k < 6; k++)
                    {
                        values[k] = Fields[i][j, k];
                    }
                    field_tree.AddRange(values, path);
                }
            }

            DA.SetData(0, grid_glb.Info());
            DA.SetData(1, new GH_Grid(grid_glb));
            DA.SetData(2, new GH_Grid(grid_act));
            DA.SetData(3, new GH_Grid(grid_shp));
            DA.SetDataList(4, topo_shell_node);
            DA.SetDataTree(5, topo_tree);
            DA.SetDataTree(6, field_tree);

        }


        public Grid SqlReadGrid(string tableName, SQLiteCommand cmd)
        {
            cmd.CommandText = "select * from " + tableName;
            var data = new List<double[]>();
            var labels = new List<string>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    double[] range;
                    IAddress.TryParseDouble(reader.GetString(2), out range);
                    data.Add(range);
                    labels.Add(reader.GetString(1));
                }
                reader.Close();
            }
            var grid = new Grid(data.ToArray(), labels.ToArray());
            return grid;
        }
        public int[][] SqlReadTopoShellFace(string tableName, SQLiteCommand cmd)
        {
            cmd.CommandText = "select * from " + tableName;
            var topo = new List<int[]>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var face = new int[4] {
                                            reader.GetInt32(1)-1,
                                            reader.GetInt32(2)-1,
                                            reader.GetInt32(3)-1,
                                            reader.GetInt32(4)-1
                    };
                    topo.Add(face);
                }
                reader.Close();
            }
            return topo.ToArray();
        }
        public int[] SqlReadTopoShellNode(string tableName, SQLiteCommand cmd)
        {
            cmd.CommandText = "select * from " + tableName;
            var topo = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    topo.Add(reader.GetInt32(0) - 1);
                }
                reader.Close();
            }
            return topo.ToArray();
        }


    }
}
