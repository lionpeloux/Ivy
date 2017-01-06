using IvyCore.MultiDimGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.MultiDimGrid
{
    public enum AddressType
    {
        Node,
        Cell,
    }

    /// <summary>
    /// An Address that belongs to a given Grid.
    /// Concrete GridTuple are of type NodeTuple and CellTuple
    /// </summary>
    public abstract partial class Address : IAddress, IGridElement
    {
        #region FIELDS
        public AddressType Type { get; protected set; }
        public Grid Grid { get; protected set; }
        public abstract int Index { get; }
        #endregion

        #region CONSTRUCTORS

        protected Address(Grid grid, IList<int> tuple) : base(tuple)
        {
            this.Grid = grid;
        }
        
        #endregion

        #region FACTORY

        /// <summary>
        /// Creates a NodeAddress for a given Address in a Given Grid.
        /// </summary>
        /// <param name="grid">The Grid this NodeAddress belongs to.</param>
        /// <param name="address">The address to considere.</param>
        /// <returns>A NodeAddress.</returns>
        public static Address CreateNodeAddress(Grid grid, IList<int> address)
        {
            return new NodeAddress(grid, address);
        }

        /// <summary>
        /// Creates a CellAddress for a given Address in a Given Grid.
        /// </summary>
        /// <param name="grid">The Grid this CellAddress belongs to.</param>
        /// <param name="address">The address to considere.</param>
        /// <returns>A CellAddress.</returns>
        public static Address CreateCellAddress(Grid grid, IList<int> address)
        {
            return new CellAddress(grid, address);
        }
        
        #endregion

        #region INSTANCE METHODS
        public string Info()
        {
            return ToString();
        }
        #endregion
    }
}
