using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    interface IGridElement
    {
        /// <summary>
        /// The Grid an Element belongs to.
        /// </summary>
        Grid Grid { get; }

        /// <summary>
        /// Grid Dimension.
        /// </summary>
        int Dim { get; }

        /// <summary>
        /// Get detailed informations on a given Grid Element
        /// </summary>
        /// <returns></returns>
        string Info();
    }
}
