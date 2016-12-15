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
    public class Comp_NodeIndexFromAddress : GH_Component
    {
        GH_Grid ghGrid;

        public override Guid ComponentGuid
        {
            get { return new Guid("{eb769004-0623-4ae0-983d-6f72fda47695}"); }
        }

        public Comp_NodeIndexFromAddress()
          : base("Node Index From Address", "Node Index",
              "Convert a Node Address to its corresponding Index.",
              "Ivy", "Node")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "G", "The Grid the Node belongs to.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Address", "A", "The Node address in the Grid.", GH_ParamAccess.list);
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Index", "I", "The Node Index in the Grid.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ghGrid = new GH_Grid();
            var address = new List<int>();

            if (!DA.GetData(0, ref ghGrid)) { return; }
            if (!DA.GetDataList(1, address)) { return; }


            var index = ghGrid.Value.NodeIndex(address);
            DA.SetData(0, index);
        }



    }
}