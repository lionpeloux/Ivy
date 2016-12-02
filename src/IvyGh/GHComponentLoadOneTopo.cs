using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using IvyCore.Factory;
using IvyCore.Parametric;

namespace IvyGh
{
    public class GHComponentLoadOneTopo : GH_Component
    {

        public GHComponentLoadOneTopo()
          : base("LoadOneTopo", "LoadOneTopo", "","Ivy", "Test")
        {
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{cfa28a96-798e-4d5d-a801-ee077840ba85}"); }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Vertices", "Vertices", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Faces", "Faces", "", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Thickness", "e", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("shell_out", "shell_out", "");
            pManager.Register_StringParam("grid_out", "grid_out", "");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var vertices = new List<GH_Point>();
            var faces = new GH_Structure<GH_Integer>();
            var thickness = new List<GH_Number>();

            DA.GetDataList(0, vertices);
            DA.GetDataTree(1, out faces);
            DA.GetDataList(2, thickness);

            var properties = new List<AbaqusShellElementProperty>();
            double E = 5e9;
            double d = 1.0;
            for (int i = 0; i < faces.Branches.Count; i++)
            {
                double e = thickness[i].Value;
                var prop = new AbaqusShellElementProperty(e, E, d, 0);
                properties.Add(prop);
            }

            var shell = new Shell(ToList(vertices), ToList(faces), properties);

            var X3D = new double[3][]{
                                new double[2] {1,2},
                                new double[4] {10,-4,2,-15},
                                new double[3] {5,6,7},
                            };

            var grid3D = new Grid(X3D);
            DA.SetData(0, shell.ToString());
            DA.SetData(1, grid3D.ToString());
        }
    
        public static List<int> ToList(IList<GH_Integer> list)
        {
            var res = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i].Value;
                res.Add(value);
            }
            return res;
        }
        public static List<double> ToList(IList<GH_Number> list)
        {
            var res = new List<double>();
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i].Value;
                res.Add(value);
            }
            return res;
        }
        public static List<double[]> ToList(IList<GH_Point> list)
        {
            var res = new List<double[]>();
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i].Value;
                res.Add(new double[3] { value.X, value.Y, value.Z });
            }
            return res;
        }
        public static List<int[]> ToList(GH_Structure<GH_Integer> tree)
        {
            var res = new List<int[]>();
            for (int i = 0; i < tree.Branches.Count; i++)
            {
                res.Add(tree.Branches[i].ConvertAll<int>(gh => gh.Value).ToArray());
            }
            return res;
        }
    }
}
