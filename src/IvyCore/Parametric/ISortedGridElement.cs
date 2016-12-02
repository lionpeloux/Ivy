using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    interface ISortedGridElement<T> : IGridElement where T : IGridElement
    {
        /// <summary>
        /// A pointer to the List that Element belongs to in the Grid.
        /// </summary>
        IList<T> List { get; }

        /// <summary>
        /// The index of that element in the sorted List.
        /// </summary>
        int Index { get; }
    }
}
