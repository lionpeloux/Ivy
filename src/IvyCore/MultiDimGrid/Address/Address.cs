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
    /// A Address that belongs to a given Grid.
    /// Concrete GridTuple are of type NodeTuple and CellTuple
    /// </summary>
    public abstract class Address : IAddress, IGridElement
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

        // FACTORY
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

        #region PRIVATE SUBCLASSES
        /// <summary>
        /// Conrete NodeTuple that exposes the Address interface.
        /// A NodeTuple belongs to a grid so it can be associated with a unique contigous index.
        /// </summary>
        private sealed class NodeAddress : Address
        {
            public override int Index
            {
                get
                {
                    //return base.IndexFromTuple(this.Grid.NodeDimCount);
                    return base.FastIndexFromAddress(this.Grid.NodeIndexBasis);
                }
            }
            public NodeAddress(Grid grid, IList<int> tuple) : base(grid, tuple)
            {
                this.Type = AddressType.Node;
            }

            public override object Clone()
            {
                return new NodeAddress(this.Grid, this.address);
            }
        }
        /// <summary>
        /// Conrete CellTuple that exposes the Address interface.
        /// A CellTuple belongs to a grid so it can be associated with a unique contigous index.
        /// </summary>
        private sealed class CellAddress : Address
        {
            public override int Index
            {
                get
                {
                    //return base.IndexFromTuple(this.Grid.CellDimCount);
                    return base.FastIndexFromAddress(this.Grid.CellIndexBasis);
                }
            }
            public CellAddress(Grid grid, IList<int> address) : base(grid, address)
            {
                this.Type = AddressType.Cell;
            }

            public override object Clone()
            {
                return new CellAddress(this.Grid, this.address);
            }
        }
        #endregion
    }
}
