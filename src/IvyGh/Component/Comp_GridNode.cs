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
    public class Comp_GridNode : GH_Component
    {
        GH_Grid ghGrid;

        public override Guid ComponentGuid
        {
            get { return new Guid("{ec376b1c-1f9a-46ab-ae2e-9381e2cb1267}"); }
        }

        public Comp_GridNode()
          : base("Grid Node", "Grid Node",
              "Get one or all Nodes in a given Grid.",
              "Ivy", "Node")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "G", "The grid.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Node Index", "i", "The i-th Node if it exists. All the Nodes otherwise.", GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager[1].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Node(s)", "N", "The selected Nodes in the Grid.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ghGrid = new GH_Grid();
            var index = -1;

            if (!DA.GetData(0, ref ghGrid)) { return; }

            var nodes = new List<GH_Node>();

            if (!DA.GetData(1, ref index))
            {
                for (int i = 0; i < ghGrid.Value.NodeCount; i++)
                {
                    nodes.Add(new GH_Node(ghGrid.Value.Nodes[i]));
                }     
            }
            else
            {
                if (index < 0 || index >= ghGrid.Value.NodeCount)
                {
                    for (int i = 0; i < ghGrid.Value.NodeCount; i++)
                    {
                        nodes.Add(new GH_Node(ghGrid.Value.Nodes[i]));
                    }
                }
                else
                {
                    nodes.Add(new GH_Node(ghGrid.Value.Nodes[index]));
                }       
            }

            DA.SetDataList(0, nodes);
        }



}
}