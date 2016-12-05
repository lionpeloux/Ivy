using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using IvyCore.Parametric;
using Grasshopper.Kernel.Special;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel.Parameters;
using System.Windows.Forms;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using IvyGh.Type;

namespace IvyGh
{
    public class Comp_GridPoint : GH_Component
    {
        GH_Grid ghGrid;
        IvyCore.Parametric.Point point;
        double[] coord;

        public override Guid ComponentGuid
        {
            get { return new Guid("{f7eda9b3-d85f-4577-8332-a8080685005b}"); }
        }

        public Comp_GridPoint()
          : base("Point On Grid", "Point",
              "Create a point on a grid regarding its coordinates",
              "Ivy", "Grid")
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
            pManager.AddNumberParameter("Cell Number", "C", "The cell this point belongs to.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ghGrid = new GH_Grid();
            var list = new List<double>();

            if (!DA.GetData(0, ref ghGrid)) { return; }
            if (!DA.GetDataList(1, list)) { return; }

            point = new IvyCore.Parametric.Point(ghGrid.Value, list.ToArray());

            DA.SetDataList(0, point.Coord);
            DA.SetData(1, point.CellIndex());
        }



}
}