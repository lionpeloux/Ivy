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
    public class Comp_GridNormalize : GH_Component
    {
        GH_Grid ghGrid1;
        GH_Grid ghGrid2;

        public override Guid ComponentGuid
        {
            get { return new Guid("{05c90214-ff50-4e02-8e66-461ace615147}"); }
        }

        public Comp_GridNormalize()
          : base("Normalize", "Normalize",
              "Create normalized grid.",
              "Ivy", "Grid")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "G", "Input grid to normalize.", GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "info", "Grid info.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grid", "G", "The normalized grid.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref ghGrid1)) { return; }

            ghGrid2 = new GH_Grid(ghGrid1.Value.Normalize());

            DA.SetData(0, ghGrid2.Value.Info());
            DA.SetData(1, ghGrid2);
        }



}
}