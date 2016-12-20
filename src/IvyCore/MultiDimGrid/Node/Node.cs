using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.MultiDimGrid
{
    public class Node : Point, ISortedGridElement<Node>
    {
        #region PROPERTIES

        /// <summary>
        /// The index in the Grid list of nodes.
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// The tuple representing the position of the node in the Grid.
        /// </summary>
        public Address Address { get; protected set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a Node in a given Grid.
        /// </summary>
        /// <param name="grid">The Grid that this Node belongs to.</param>
        /// <param name="address">The Node address in the Grid.</param>
        /// <param name="coordinates">The Node coordinates in the Grid.</param>
        public Node(Grid grid, IList<int> address, double[] coordinates)
            :base(grid, coordinates)
        {
            this.Address = Address.CreateNodeAddress(grid, address);
            this.Index = this.Address.Index;
        }

        /// <summary>
        /// Creates a Node in a given Grid without specifing its coordinates.
        /// </summary>
        /// <param name="grid">The Grid that this Node belongs to.</param>
        /// <param name="address">The Node address in the Grid.</param>
        public Node(Grid grid, IList<int> address)
            : this(grid, address, new double[grid.Dim]) { }

        #endregion

        #region INSTANCE METHODS

        /// <summary>
        /// Gets the list of Nodes this Node belongs to.
        /// </summary>
        public IList<Node> List
        {
            get
            {
                return Grid.Nodes;
            }
        }

        public override string ToString()
        {
            var s = "(" + String.Format("{0:F2}", coordinates[0]);
            for (int i = 1; i < coordinates.Length; i++)
            {
                s += ", " + String.Format("{0:F2}", coordinates[i]);
            }
            return s += ")";
        }
        public override string Info()
        {
            return String.Format("NODE[{0,3}|{1}] = {2}", this.Index, this.Address, this.ToString());
        }

        #endregion

    }
}
