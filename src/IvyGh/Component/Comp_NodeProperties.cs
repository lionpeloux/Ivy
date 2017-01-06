using System;
using Grasshopper.Kernel;
using IvyGh.Type;

namespace IvyGh
{
    public class Comp_NodeProperties : GH_Component
    {
        GH_Grid ghGrid;

        public override Guid ComponentGuid
        {
            get { return new Guid("{3497dc88-ad39-437a-8321-609b72afef5f}"); }
        }

        public Comp_NodeProperties()
          : base("Node Properties", "Node",
              "Get Node's properties.",
              "Ivy", "Node")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "node", "The Node.", GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Node Index", "index", "The NodeIndex in the Grid.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Coordinates", "coord", "The coordinates of the Node.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Normalized Coordinates", "normCoord", "The normalized coordinates of the Node.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Node Address", "addr", "The Address of the Node.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Normalized Node Address", "normAddr", "The Address of the Node.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ghNode = new GH_Node();

            if (!DA.GetData(0, ref ghNode)) { return; }

            DA.SetData(0, ghNode.Value.Index);
            DA.SetDataList(1, ghNode.Value.Coord);
            DA.SetDataList(2, ghNode.Value.Normalize());
            DA.SetDataList(3, ghNode.Value.Address.Value);

            // Normalized address not implemented yet.
           // DA.SetDataList(4, ghNode.Value.Tuple.Value);

        }

    }
}