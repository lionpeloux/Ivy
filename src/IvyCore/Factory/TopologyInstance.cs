using IvyCore.Parametric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Factory
{
    /// <summary>
    /// A TopologyInstance consist of a concrete topologic instance made of 
    /// a shell object with two connected actuators.
    /// This part can be actuated. Each actuation node leads to an ActuationInstance. 
    /// </summary>
    public class TopologyInstance
    {
        /// <summary>
        /// A TopologyInstance belongs to an InstanceFactory
        /// </summary>
        public InstanceFactory Factory { get; protected set; }

        /// <summary>
        /// The topology node that defines this topology instance.
        /// </summary>
        public Node topologyNode { get; protected set; }

        // Conrete Topology
        public Actuator Actuator1 { get; protected set; }
        public Actuator Actuator2 { get; protected set; }
        public Shell Shell { get; protected set; }

        /// <summary>
        /// This list allows the user to keep track of certain remarkable nodes.
        /// Indeed, while benchmarking several instance of a topology, 
        /// remarkable nodes index may not be consistent from one instance to another 
        /// The same (x,y,z) point could have different mesh index from shape to shape.
        /// </summary>
        public List<int> RemarkableNodes { get; set; }

        /// <summary>
        /// Basis of ActuationInstances.
        /// A list of ActuationInstance for each ActuationNode in the ActuationGrid of the InstanceFactory.
        /// </summary>
        public ActuationInstance[] ActuationInstanceBasis { get; set; }
    }
}
