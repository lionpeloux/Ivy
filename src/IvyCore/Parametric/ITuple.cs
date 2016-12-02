using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    /// <summary>
    /// Tuple interface as an abstract base class.
    /// </summary>
    public abstract class ITuple : ICloneable, IComparable<ITuple>
    {
        protected int[] tuple;

        /// <summary>
        /// Tuple dimension.
        /// </summary>
        public int Dim
        {
            get { return tuple.Length; }
        }
        public int this[int i]
        {
            get
            {
                return tuple[i];
            }
            set
            {
                this.tuple[i] = value; 
            }
        }
        public int[] Value { get { return tuple.ToArray<int>(); } }

        protected ITuple(IList<int> tuple)
        {
            this.tuple = tuple.ToArray<int>();
        }

        /// <summary>
        /// For a given indexRange, compute the contiguous index of this tuple.
        /// i{0} ∈ [0,n{0}-1] | i{1} ∈ [0,n{1}-1] | ... | i{D-1} ∈ [0,n{D-1}-1]
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// index = i0 + n0 * (i1 + n1 * (i2 + n2 * (i3 + ... + n{D-2} * i{D-1})))
        /// </summary>
        /// <param name="count">
        /// An array that holds for each dimension the number of indices tu use 
        /// for the computation of the Tuple's index.
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// </param>
        /// <returns>The contiguous index.</returns>
        protected int IndexFromTuple(IList<int> count)
        {
            int index = this.tuple[0];
            int nprod = 1;
            for (int i = 0; i < this.tuple.Length - 1; i++)
            {
                nprod *= count[i];
                index += nprod * tuple[i + 1];
            }
            return index;
        }

        /// <summary>
        /// Fast computation of Tuple index with a given coefficient basis.
        /// i{0} ∈ [0,n{0}-1] | i{1} ∈ [0,n{1}-1] | ... | i{D-1} ∈ [0,n{D-1}-1]
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// Basis = [1, n{0}, n{0}*n{1}, ..., n{0}*n{1}*...*n{D-2}]
        /// index = i0 + n0 * (i1 + n1 * (i2 + n2 * (i3 + ... + n{D-2} * i{D-1}))).
        /// index = 1*i0 + n0*i1 + n0*n1*i2 + ... + n0*n1*...*n{D-2}*i{D-1}.
        /// </summary>
        /// <param name="basis">
        /// An array that hodls the basis to use for fast computation of the Tuple's index.
        /// [1, n{0}, n{0}*n{1}, ..., n{0}*n{1}*...*n{D-2}]
        /// </param>
        /// <returns>The contiguous index.</returns>
        protected int FastIndexFromTuple(IList<int> basis)
        {
            int index = 0;
            for (int d = 0; d < tuple.Length; d++)
            {
                index += basis[d] * tuple[d];
            }
            return index;
        }
        public static int FastIndexFromTuple(IList<int> basis, IList<int> tuple)
        {
            int index = 0;
            for (int d = 0; d < tuple.Count; d++)
            {
                index += basis[d] * tuple[d];
            }
            return index;
        }

        /// <summary>
        /// Get a tuple from its contiguous index.
        /// This is achived by recursive euclidean division.
        /// i{0} ∈ [0,n{0}-1] | i{1} ∈ [0,n{1}-1] | ... | i{D-1} ∈ [0,n{D-1}-1]
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// index = i0 + n0 * (i1 + n1 * (i2 + n2 * (i3 + ... + n{D-2} * i{D-1})))
        /// </summary>
        /// <param name="index">The index to convert.</param>
        /// <param name = "count">
        /// An array that holds for each dimension the number of indices tu use 
        /// for the computation of the Tuple's index.
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// </param>
        /// <returns>The tuple corresponding to the given index.</returns>
        protected static IList<int> TupleFromIndex(int index, IList<int> count)
        {
            int n = count.Count;
            var tuple = new int[n];

            int q = index;
            int r;
            for (int i = 0; i < n - 1; i++)
            {
                q = Math.DivRem(q, count[i], out r);
                tuple[i] = r;
            }
            tuple[n - 1] = q;
            return tuple;
        }

        public static string ToString(IList<double> array)
        {
            var s = "(" + array[0];
            for (int i = 1; i < array.Count; i++)
            {
                s += ", " + array[i];
            }
            return s += ")";
        }
        public static string ToString(IList<int> array)
        {
            var s = "(" + array[0];
            for (int i = 1; i < array.Count; i++)
            {
                s += ", " + array[i];
            }
            return s += ")";
        }


        /// <summary>
        /// String representation of a tuple.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <returns>A string representing the tuple (i0,i1,...,in-1).</returns>
        public override string ToString()
        {
            var s = "(" + this.tuple[0];
            for (int i = 1; i < this.tuple.Length; i++)
            {
                s += ", " + tuple[i];
            }
            return s += ")";
        }
        public int CompareTo(ITuple tuple)
        {
            int dim = tuple.Dim;
            if (dim != this.Dim)
            {
                throw new System.ArgumentOutOfRangeException("The tuples must be of same dimension to be comparable");
            }
            // Backward Loop
            for (int d = dim; d > 0; d--)
            {
                int i1 = this[d];
                int i2 = tuple[d];

                if (i2 > i1)
                {
                    // this < tuple
                    return -1;
                }
                else if (i2 < i1)
                {
                    // this > tuple
                    return 1;
                }
            }
            // tuples are equals
            return 0;
        }
        public abstract object Clone();
    }


}
