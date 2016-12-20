using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using IvyGh.Type;
using IvyCore.MultiDimGrid;
using IvyGh.Properties;

namespace IvyGh
{
    public class Comp_GridPoint : GH_Component
    {
        GH_Grid ghGrid;
        Point point;
        double[] coord;

        public override Guid ComponentGuid
        {
            get { return new Guid("{f7eda9b3-d85f-4577-8332-a8080685005b}"); }
        }

        public Comp_GridPoint()
          : base("Point From Coordinates", "Point",
              "Creates a point on a grid from coordinates. The point will be constrained to be in the space defined by the Grid.",
              "Ivy", "Node")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Input Grid", "grid", "The input Grid.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Point coordinates", "ptCoord", "The coordinates of the point to evaluate.", GH_ParamAccess.list);
            pManager[0].Optional = false;
            pManager[1].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Point Coordinates", "ptCoord", "The coordinates of the point in the Grid.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Cell Number", "cellIndex", "The index of the Cell this Point belongs to.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ghGrid = new GH_Grid();
            var list = new List<double>();

            if (!DA.GetData(0, ref ghGrid)) { return; }
            if (!DA.GetDataList(1, list)) { return; }

            point = new IvyCore.MultiDimGrid.Point(ghGrid.Value, list.ToArray());

            DA.SetDataList(0, point.Coord);
            DA.SetData(1, point.CellIndex());
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.point;
            }
        }
    }

    
}