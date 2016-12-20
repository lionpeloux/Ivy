using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using IvyGh.Properties;
using IvyGh.Type;
using System;
using System.Collections.Generic;

namespace IvyGh.Component
{
    public class Comp_ConstructGrid : GH_Component
    {

        GH_Grid ghGrid;

        public Comp_ConstructGrid()
          : base("Construct Grid", "Grid",
              "Create a new Grid from a tree of numbers and an (optional) set of labels.",
              "Ivy", "Grid")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Input Range", "D", "The data to create the grid from. As a tree of numbers.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Labels", "L", "A list of labels, one for each dimension", GH_ParamAccess.list);
            pManager[0].Optional = false;
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Grid Informations", "info", "Detailed informations about the Grid.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grid", "G", "A N-dimensional grid.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {         
            var tree = new GH_Structure<GH_Number>();
            if (!DA.GetDataTree(0, out tree)) { return; }

            var X = new double[tree.Branches.Count][];
            for (int d = 0; d < tree.Branches.Count; d++)
            {
                X[d] = new double[tree.Branches[d].Count];
                for (int i = 0; i < tree.Branches[d].Count; i++)
                {
                    X[d][i] = tree.Branches[d][i].Value;
                }
            }

            var labels = new List<string>();
            if (!DA.GetDataList(1, labels))
            {
                ghGrid = new GH_Grid(X);
            }
            else
            {
                ghGrid = new GH_Grid(X, labels);
            }

            DA.SetData(0, ghGrid.Value.Info());
            DA.SetData(1, ghGrid);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ebab2030-8d6d-4e28-8aa7-0b9baefa62d1}"); }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.grid;
            }
        }

    }
}