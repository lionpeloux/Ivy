using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using IvyGh.Type;

namespace IvyGh.Component
{
    public class Param_Grid : GH_Param<GH_Grid>
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Param_Grid()
          : base("Grid", "Grid", "N dimensional Grid.", "Ivy", "Params", GH_ParamAccess.item)
        {
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{40192098-498e-4309-97de-cf7a56d1f11f}"); }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }
    }
}