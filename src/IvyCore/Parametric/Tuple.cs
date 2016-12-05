using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    public enum TupleType
    {
        Node,
        Cell,
    }

    /// <summary>
    /// A Tuple that belongs to a given Grid.
    /// Concrete GridTuple are of type NodeTuple and CellTuple
    /// </summary>
    public abstract class Tuple : ITuple, IGridElement
    {
        #region FIELDS
        public TupleType Type { get; protected set; }
        public Grid Grid { get; protected set; }
        public abstract int Index { get; }
        #endregion

        #region CONSTRUCTORS
        protected Tuple(Grid grid, IList<int> tuple) : base(tuple)
        {
            this.Grid = grid;
        }

        // FACTORY
        public static Tuple CreateNodeTuple(Grid grid, IList<int> tuple)
        {
            return new NodeTuple(grid, tuple);
        }
        public static Tuple CreateCellTuple(Grid grid, IList<int> tuple)
        {
            return new CellTuple(grid, tuple);
        }
        #endregion

        #region OPERATORS    
        #endregion

        #region INSTANCE METHODS
        public string Info()
        {
            return ToString();
        }
        #endregion

        #region STATIC METHOD
        
        #endregion  

        #region PRIVATE SUBCLASSES
        /// <summary>
        /// Conrete NodeTuple that exposes the Tuple interface.
        /// A NodeTuple belongs to a grid so it can be associated with a unique contigous index.
        /// </summary>
        private sealed class NodeTuple : Tuple
        {
            public override int Index
            {
                get
                {
                    //return base.IndexFromTuple(this.Grid.NodeDimCount);
                    return base.FastIndexFromTuple(this.Grid.NodeIndexBasis);
                }
            }
            public NodeTuple(Grid grid, IList<int> tuple) : base(grid, tuple)
            {
                this.Type = TupleType.Node;
            }

            public override object Clone()
            {
                return new NodeTuple(this.Grid, this.tuple);
            }
        }
        /// <summary>
        /// Conrete CellTuple that exposes the Tuple interface.
        /// A CellTuple belongs to a grid so it can be associated with a unique contigous index.
        /// </summary>
        private sealed class CellTuple : Tuple
        {
            public override int Index
            {
                get
                {
                    //return base.IndexFromTuple(this.Grid.CellDimCount);
                    return base.FastIndexFromTuple(this.Grid.CellIndexBasis);
                }
            }
            public CellTuple(Grid grid, IList<int> tuple) : base(grid, tuple)
            {
                this.Type = TupleType.Cell;
            }

            public override object Clone()
            {
                return new CellTuple(this.Grid, this.tuple);
            }
        }
        #endregion
    }
}
