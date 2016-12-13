using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using IvyGh.Type;

namespace IvyGh.Component
{
    public class Param_Node : GH_Param<GH_Node> // TODO : implement GH_ExpressionParam for normalized
    {

        public Param_Node()
          : base("Node", "Node", "Node in a Grid.", "Ivy", "Params", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{a7f45a64-fd0e-4e4f-a576-5e059f9530c1}"); }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }
    }
}