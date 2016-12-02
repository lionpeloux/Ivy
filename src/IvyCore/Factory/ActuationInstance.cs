using IvyCore.Parametric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Factory
{
    /// <summary>
    /// ActuationInstance of a given TopologyInstance.
    /// </summary>
    public class ActuationInstance
    {
        /// <summary>
        /// An ActuationInstance belongs to a TopologyInstance.
        /// </summary>
        public TopologyInstance TopologyInstance { get; protected set; }

        /// <summary>
        /// The actuation node that defines this actuation instance.
        /// </summary>
        public Node ActuationNode { get; protected set; }
    }
}
