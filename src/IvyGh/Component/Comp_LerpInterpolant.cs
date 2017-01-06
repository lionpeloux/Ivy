using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using IvyGh.Type;
using IvyCore.MultiDimGrid;

namespace IvyGh
{
    public class Comp_LerpInterpolant : GH_Component
    {
        GH_Grid ghGrid;
        IvyCore.MultiDimGrid.Point point;
        double[] coord;

        public override Guid ComponentGuid
        {
            get { return new Guid("{911c70b3-e3ce-4a9c-bfcc-1a21a86f32de}"); }
        }

        public Comp_LerpInterpolant()
          : base("Lerp Interpolant", "Lerp",
              "Get the interpolant for a given point in the grid.",
              "Ivy", "Interpolation")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "G", "The grid.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Point coordinates", "P", "The coordinates of the point to evaluate.", GH_ParamAccess.list);
            pManager[0].Optional = false;
            pManager[1].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Point Coordinates", "P", "The coordinates of the point on the grid.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Point Normalized Coordinates", "N", "The normalized coordinates of the point on the grid.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Cell Number", "C", "The cell this point belongs to.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Lerp Interpolant", "I", "The LERP interpolant.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ghGrid = new GH_Grid();
            var list = new List<double>();

            if (!DA.GetData(0, ref ghGrid)) { return; }
            if (!DA.GetDataList(1, list)) { return; }

            point = new Point(ghGrid.Value, list.ToArray());
            var cellIndex = point.GetCellIndex();
            var LERP = ghGrid.Value.Cells[cellIndex].GetInterpolant(point);

            DA.SetDataList(0, point.Coord);
            DA.SetDataList(1, point.Normalize());
            DA.SetData(2, cellIndex);
            DA.SetData(3, new GH_Interpolant(LERP));
        }



    }
}