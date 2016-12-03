using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using IvyCore.Parametric;

namespace IvyGh
{
    public class GHComponentLerpTest : GH_Component
    {
        private Grid grid;
        private IvyCore.Parametric.Point gp;
        private double[] Z;
        private Point3d[] Zpts;
        /// <summary>
        /// Initializes a new instance of the GHComponentLerpTest class.
        /// </summary>
        public GHComponentLerpTest()
          : base("GHComponentLerpTest", "Lerp",
              "",
              "Ivy", "Test")
        {

            
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "p", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "p", "", GH_ParamAccess.item);
            pManager.AddPointParameter("Field", "f", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("Cell", "c", "", GH_ParamAccess.item);
            pManager.AddTextParameter("Info", "out", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var X2D = new double[2][]{
                        new double[2] {0,1},
                        new double[2] {0,1}
            };

            X2D = new double[2][]{
                        new double[3] {0,0.25, 1},
                        new double[3] {0,0.5,1}
            };
            //var Z2D = new double[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var Z2D = new double[9] { 1, 2, 3, 4, 2, -3, 1, 2, 3 };
            //var Z2D = new double[4] { 1, 2, 3, 4 };
            //Z2D = new double[6] { 1, 2, 3, 1, 2, 3 };


            Z = Z2D;

            grid = new Grid(X2D);
            gp = new IvyCore.Parametric.Point(grid);
            Zpts = this.ConstructFieldView(grid, Z);


            var p = new Point3d();
            DA.GetData(0, ref p);

            int n = Math.Min(gp.Dim, 3);
            for (int i = 0; i < n; i++)
            {
                gp[i] = p[i];
            }

            int cellIndex = gp.CellIndex();
            var cell = gp.Grid.Cells[cellIndex];

            double[] LERP = cell.LerpCoefficients(gp);

            double zlerp = 0;
            for (int i = 0; i < LERP.Length; i++)
            {
                int index = cell.VerticesIndex[i];
                zlerp += LERP[i] * Z[index];
            }

            var cellPts = new Point3d[5];
            double[] node;
            node = grid.Nodes[cell.VerticesIndex[0]].Coord;
            cellPts[0] = new Point3d(node[0], node[1], 0);

            node = grid.Nodes[cell.VerticesIndex[1]].Coord;
            cellPts[1] = new Point3d(node[0], node[1], 0);

            node = grid.Nodes[cell.VerticesIndex[3]].Coord;
            cellPts[2] = new Point3d(node[0], node[1], 0);

            node = grid.Nodes[cell.VerticesIndex[2]].Coord;
            cellPts[3] = new Point3d(node[0], node[1], 0);

            cellPts[4] = cellPts[0];

            var contour = new PolylineCurve(cellPts);

            p.Z = zlerp;

            DA.SetData(0, p);
            DA.SetDataList(1, Zpts);
            DA.SetData(2, contour);
            DA.SetData(3, grid.Info());
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7aff228b-e898-4be7-a426-95bf77dc6ba4}"); }
        }
        
        public Point3d[] ConstructFieldView(Grid grid, double[] field)
        {
            var pts = new Point3d[grid.NodeCount];

            for (int i = 0; i < pts.Length; i++)
            {
                pts[i].X = grid.Nodes[i][0];
                pts[i].Y = grid.Nodes[i][1];
                pts[i].Z = field[i];
            }

            return pts;
        }
    }
}