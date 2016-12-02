using IvyCore.Parametric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Factory
{
    public class InstanceFactory
    {
        // Topology Manifold (Dim <=3)
        public Grid TopologyGrid { get; set; }
        public int TopologyDim { get { return TopologyGrid.Dim; } }

        // Actuation Manifold (Dim <= 3)
        public Grid ActuationGrid { get; set; }
        public int ActuationDim { get { return ActuationGrid.Dim; } }

        /// <summary>
        /// Basis of TopologyInstance.
        /// A list of TopologyInstance for each ActuationNode in the ActuationGrid of the InstanceFactory.
        /// </summary>
        public TopologyInstance[] TopologyInstanceBasis { get; set; }

        // Data for each topolgy node
        // Data for each actuation node
        // Data Interpolation
    }
}
