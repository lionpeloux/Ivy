using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    public class Node : Point, ISortedGridElement<Node>
    {
        /// <summary>
        /// The index in the Grid list of nodes.
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// The tuple representing the position of the node in the Grid.
        /// </summary>
        public Tuple Tuple { get; protected set; }

        /// <summary>
        /// Create a new Node instance.
        /// </summary>
        /// <param name="grid">The Grid that node belongs to.</param>
        /// <param name="coordinates">Node coordinates</param>
        public Node(Grid grid, IList<int> tuple, double[] coordinates)
            :base(grid, coordinates)
        {
            this.Tuple = Tuple.CreateNodeTuple(grid, tuple);
        }

        public IList<Node> List
        {
            get
            {
                return Grid.Nodes;
            }
        }
        public override string ToString()
        {
            var s = "(" + coordinates[0];
            for (int i = 1; i < coordinates.Length; i++)
            {
                s += ", " + coordinates[i];
            }
            return s += ")";
        }

    }
}
