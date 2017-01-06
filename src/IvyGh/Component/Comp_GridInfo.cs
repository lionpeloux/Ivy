using Grasshopper.Kernel;
using IvyGh.Properties;
using IvyGh.Type;
using System;

namespace IvyGh
{
    public class Comp_GridInfo : GH_Component
    {
        GH_Grid ghGrid;

       

        public Comp_GridInfo()
          : base("Grid Info", "Info",
              "Display grid informations.",
              "Ivy", "Grid")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{b5917e00-9704-4713-9524-0bcae2adfa75}"); }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.grid_info;
            }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "grid", "The input Grid.", GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Grid Informations", "info", "Detailed informations about the Grid.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref ghGrid)) { return; }


            DA.SetData(0, ghGrid.Value.Info());
        }

        



    }
}