using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.MultiDimGrid
{
     partial class Address
    {
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
                    return base.FastAddressToIndex(this.Grid.NodeIndexBasis);
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
    }
}
