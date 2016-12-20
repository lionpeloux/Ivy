using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.MultiDimGrid
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

            // make sure this point is in the grid domain.
            this.Legalize();
        }        
        public Point(Grid grid) 
            : this(grid, new double[grid.Dim]){}

        /// <summary>
        /// Get the Cell index this point belongs to. 
        /// </summary>
        /// <returns>The corresponding Cell Index in the grid.</returns>
        public int CellIndex()
        {
            var tuple = new int[Dim];
            for (int d = 0; d < Dim; d++)
            {
                var x = this[d];
                for (int i = 1; i < Grid.Data[d].Length; i++)
                {
                    var xi = Grid.Data[d][i];
                    if (x <= xi)
                    {
                        tuple[d] = i - 1;
                        break;
                    }
                }
            }
            return Grid.CellIndex(tuple);
        }

        /// <summary>
        /// Enforce (in-place) a given Point to be constrained in the Grid domain.
        /// I = I0 x I1 x ... x I{D-1} where I{i} = [T{i}_0, T{i}_1]
        /// </summary>
        public void Legalize()
        {
            for (int d = 0; d < Dim; d++)
            {
                this[d] = Grid.Intervals[d].Legalize(this[d]);
            }
        }

        /// <summary>
        /// Test if a given Point is in the Grid domaine.
        /// </summary>
        /// <returns>True is the Point is in already in the Grid domain.</returns>
        public bool IsLegal()
        {
            for (int d = 0; d < Dim; d++)
            {
                if (!Grid.Intervals[d].IsLegal(this[d]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Normalized a given point in [0,1]^D.
        /// It is not consistent to return a Point object
        /// as it may not be in the Grid domain.
        /// </summary>
        /// <returns>A new normalized vector in [0,1]^D.</returns>
        public double[] Normalized()
        {
            var p = new double[Dim];
            for (int d = 0; d < Dim; d++)
            {
                p[d] = Grid.Intervals[d].Normalize(this[d]);
            }
            return p;
        }

        public virtual string Info()
        {
            throw new NotImplementedException();
        }
    }
}
