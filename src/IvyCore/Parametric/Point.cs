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
    public class Point : IGridElement
    {
        protected double[] coordinates;

        /// <summary>
        /// The Grid this Point belongs to.
        /// </summary>
        public Grid Grid { get; protected set; }

        /// <summary>
        /// Point dimension.
        /// </summary>
        public int Dim { get { return this.Grid.Dim; } }

        /// <summary>
        /// Point coordinate for a particular dimension.
        /// Idem as calling.
        /// </summary>
        public double this[int d]
        {
            get
            {
                return coordinates[d];
            }
            set
            {
                coordinates[d] = value;
            }
        }

        /// <summary>
        /// Point coordinates as a double[] of length Dim.
        /// </summary>
        public double[] Coord
        {
            get { return coordinates; }
            protected set
            {
                int dim = value.Length;
                if (dim != this.Dim)
                {
                    throw new System.IndexOutOfRangeException("This NState is of dimension " + this.Dim + " but the given double array is of length " + dim + ".");
                }
                else
                {
                    // make a deep copy
                    coordinates = value.ToArray<double>();
                }
            }
        }

        /// <summary>
        /// Create a new Point instance.
        /// </summary>
        /// <param name="grid">The Grid that node belongs to.</param>
        /// <param name="coordinates">Node coordinates</param>
        public Point(Grid grid, double[] coordinates)
        {
            this.Grid = grid;

            // make a deep copy of the coord (bad ? should I hold a reference to coordinates instead ?)
            this.Coord = coordinates; 
        }

        public virtual string Info()
        {
            throw new NotImplementedException();
        }
    }
}
