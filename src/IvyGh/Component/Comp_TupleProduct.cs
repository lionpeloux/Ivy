﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using IvyCore.MultiDimGrid;
using IvyGh.Properties;

namespace IvyGh
{
    public class Comp_TupleProduct : GH_Component // make it expandable
    {

        public override Guid ComponentGuid
        {
            get { return new Guid("{5a30403a-a364-4617-838b-d60c4e32bb1a}"); }
        }

        public Comp_TupleProduct()
          : base("Cartesian Product", "T1xT2",
              "Create a new tuple as the cartesian product of two tuples.",
              "Ivy", "Tuple")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("First Tuple", "T1", "The first tuple.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Second Tuple", "T2", "The second tuple.", GH_ParamAccess.list);
            pManager[0].Optional = false;
            pManager[1].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("The Cartesian Product", "T", "The cartesian product T1xT2.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var tuple1 = new List<double>();
            var tuple2 = new List<double>();

            if (!DA.GetDataList(0, tuple1)) { return; }
            if (!DA.GetDataList(1, tuple2)) { return; }

            var tuple = IAddress.CartesianProduct(tuple1, tuple2);

            DA.SetDataList(0, tuple);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.grid_prod;
            }
        }



    }
}