using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using IvyGh.Type;
using IvyGh.Properties;

namespace IvyGh.Component
{
    public class Param_Cell : GH_Param<GH_Cell> // TODO : implement GH_ExpressionParam for normalized
    {

        public Param_Cell()
          : base("Cell", "Cell", "Cell in a Grid.", "Ivy", "Params", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{64906d57-b708-41e2-b685-b9858534e4b7}"); }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        //protected override System.Drawing.Bitmap Icon
        //{
        //    get
        //    {
        //        return Resources.param_node;
        //    }
        //}
    }
}