using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using IvyGh.Type;

namespace IvyGh.Component
{
    public class Param_Interpolant : GH_Param<GH_Interpolant> // TODO : implement GH_ExpressionParam for normalized
    {

        public Param_Interpolant()
          : base("Interpolant", "Inter", "Interpolant for a given point in the grid", "Ivy", "Params", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{5728e257-bd52-4133-a177-b8898d7bce34}"); }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
    }
}