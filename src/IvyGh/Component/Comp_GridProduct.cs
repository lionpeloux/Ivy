using System;
using Grasshopper.Kernel;
using IvyGh.Type;
using IvyGh.Properties;

namespace IvyGh
{
    public class Comp_GridProduct : GH_Component
    {
        GH_Grid ghGrid1;
        GH_Grid ghGrid2;
        GH_Grid ghGrid;



        public Comp_GridProduct()
          : base("Cartesian Product", "G1xG2",
              "Create a new grid as the cartesian product of the two input grids.",
              "Ivy", "Grid")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{4ae212dd-72de-4d5f-942e-3d5a480b0f8b}"); }
        }
        
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.grid_prod;
            }
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("First Grid", "G1", "The first grid.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Second Grid", "G2", "The second grid.", GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager[1].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "info", "Grid info.", GH_ParamAccess.item);
            pManager.AddGenericParameter("The Cartesian Product G1xG2", "G", "The cartesian product G1xG2.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref ghGrid1)) { return; }
            if (!DA.GetData(1, ref ghGrid2)) { return; }

            ghGrid = new GH_Grid(ghGrid1.Value * ghGrid2.Value);

            DA.SetData(0, ghGrid.Value.Info());
            DA.SetData(1, ghGrid);
        }

        


    }
}