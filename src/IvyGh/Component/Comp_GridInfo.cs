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
    public class Comp_GridInfo : GH_Component
    {
        GH_Grid ghGrid;

        public override Guid ComponentGuid
        {
            get { return new Guid("{b5917e00-9704-4713-9524-0bcae2adfa75}"); }
        }

        public Comp_GridInfo()
          : base("Grid Info", "Info",
              "Display grid informations.",
              "Ivy", "Grid")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "G", "Input grid.", GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "info", "Grid info.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref ghGrid)) { return; }


            DA.SetData(0, ghGrid.Value.Info());
        }



}
}