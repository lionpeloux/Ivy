using System;
using Grasshopper.Kernel;
using IvyGh.Type;
using IvyGh.Properties;

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
              "Normalize a Grid.",
              "Ivy", "Grid")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "grid", "Input grid to normalized.", GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "grid", "The normalized grid.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref ghGrid1)) { return; }

            ghGrid2 = new GH_Grid(ghGrid1.Value.Normalize());

            DA.SetData(0, ghGrid2);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.grid_norm;
            }
        }



    }
}