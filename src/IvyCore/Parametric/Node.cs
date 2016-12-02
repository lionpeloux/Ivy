using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    public class Node
    {
        private int dim;
        private double[] node;

        /// <summary>
        /// State dimension.
        /// </summary>
        public int Dim
        {
            get { return dim; }
            protected set { dim = value; }
        }

        /// <summary>
        /// State value as double[] of length Dim.
        /// </summary>
        public double[] Value
        {
            get { return node; }
            protected set {
                int dim = value.Length;
                if (dim != this.Dim)
                {
                    throw new System.IndexOutOfRangeException("This NState is of dimension " + this.Dim + " but the given double array is of length " + dim + ".");
                }
                else
                {
                    node = value;
                }             
            }
        }

        /// <summary>
        /// State value for a particular dimension.
        /// Idem as calling Value[d].
        /// </summary>
        public double this[int d]
        {
            get {
                return node[d];
            }
            set
            {
                node[d] = value;
            }
        }

        /// <summary>
        /// Create a new NState with an actual state.
        /// </summary>
        /// <param name="state">The actual state.</param>
        public Node(double[] state)
        {
            this.Dim = state.Length;
            this.Value = state;
        }

        /// <summary>
        /// Create a new NState with an actual state of zeros.
        /// </summary>
        /// <param name="dim">The dimension of the state.</param>
        public Node(int dim) : this(new double[dim])
        {
        }
        
        public override string ToString()
        {
            var s = "(" + node[0];
            for (int i = 1; i < node.Length; i++)
            {
                s += ", " + node[i];
            }
            return s += ")";
        }

    }
}
