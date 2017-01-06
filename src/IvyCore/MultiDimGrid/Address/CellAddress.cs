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
                    return base.FastAddressToIndex(this.Grid.CellIndexBasis);
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
    }
}
