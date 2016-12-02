using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    /// <summary>
    /// A Cell element of 2^D connected Nodes.
    /// </summary>
    public class Cell : ISortedGridElement<Cell>
    {
        private Node[][] nodes;

        /// <summary>
        /// The Grid this Cell belongs to.
        /// </summary>
        public Grid Grid { get; protected set; }

        /// <summary>
        /// The index in the Grid list of cells.
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// The tuple representing the position of the cell in the Grid.
        /// </summary>
        public int[] Tuple { get; protected set; }

        /// <summary>
        /// Cell dimension.
        /// </summary>
        public int Dim { get { return this.Grid.Dim; } }

        public IList<Cell> List
        {
            get
            {
                return this.Grid.Cells;
            }
        }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
