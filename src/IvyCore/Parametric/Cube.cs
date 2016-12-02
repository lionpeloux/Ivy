using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    /// <summary>
    /// A continuous N-cube manifold.
    /// </summary>
    public class Cube
    {
        private int dim;
        private Interval[] range;

        /// <summary>
        /// Cube dimension.
        /// </summary>
        public int Dim
        {
            get { return dim; }
            protected set { dim = value; }
        }

        /// <summary>
        /// Return the interval for a given dimension.
        /// </summary>
        /// <param name="d">The dimension to consider.</param>
        /// <returns>The interval in that dimension.</returns>
        public Interval this[int d]
        {
            get
            {
                return range[d];
            }
        }

        public Cube(Interval[] range)
        {
            this.Dim = range.Length;
            this.range = range;
        }
    }
}
