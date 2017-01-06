using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.MultiDimGrid
{
    /// <summary>
    /// Address interface to manipulate mainly addresses as tuples of integers.
    /// Gets lots of static methods to manipulate coords and addresses, independently of any grid.
    /// </summary>
    public abstract class IAddress : ICloneable, IComparable<IAddress>
    {
        #region FIELDS
        /// <summary>
        /// Internal representation of this Address as an array of integers (int[]).
        /// </summary>
        protected int[] address;
        #endregion

        #region PROPERTIES   

        /// <summary>
        /// Address dimension.
        /// </summary>
        public int Dim
        {
            get { return address.Length; }
        }

        /// <summary>
        /// Gets the i-th coordinate of this Address.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>The Address coordinate for the i-th dimension.</returns>
        public int this[int i]
        {
            get
            {
                return address[i];
            }
            set
            {
                this.address[i] = value; 
            }
        }

        /// <summary>
        /// Gets a copy of the internal array (int[]) representing this Address.
        /// </summary>
        public int[] Value { get { return address.ToArray<int>(); } }

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Construct an Address from a list of integers.
        /// </summary>
        /// <param name="tuple">The list of integers that describes the Address.</param>
        protected IAddress(IList<int> tuple)
        {
            this.address = tuple.ToArray<int>();
        }
        #endregion

        #region INSTANCE METHODS

        /// <summary>
        /// For a given indexRange, compute the contiguous index of this tuple.
        /// i{0} ∈ [0,n{0}-1] | i{1} ∈ [0,n{1}-1] | ... | i{D-1} ∈ [0,n{D-1}-1]
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// index = i0 + n0 * (i1 + n1 * (i2 + n2 * (i3 + ... + n{D-2} * i{D-1})))
        /// </summary>
        /// <param name="count">
        /// An array that holds for each dimension the number of indices tu use 
        /// for the computation of the Address's index.
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// </param>
        /// <returns>The contiguous index.</returns>
        protected int AddressToIndex(IList<int> count)
        {
            return AddressToIndex(address, count);
        }

        /// <summary>
        /// Fast computation of Address index with a given coefficient basis.
        /// </summary>
        /// <remarks>
        /// i{0} ∈ [0,n{0}-1] | i{1} ∈ [0,n{1}-1] | ... | i{D-1} ∈ [0,n{D-1}-1]
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// Basis = [1, n{0}, n{0}*n{1}, ..., n{0}*n{1}*...*n{D-2}]
        /// index = i0 + n0 * (i1 + n1 * (i2 + n2 * (i3 + ... + n{D-2} * i{D-1}))).
        /// index = 1*i0 + n0*i1 + n0*n1*i2 + ... + n0*n1*...*n{D-2}*i{D-1}.
        /// </remarks>
        /// <param name="basis">
        /// An array that hodls the basis to use for fast computation of the Address's index.
        /// [1, n{0}, n{0}*n{1}, ..., n{0}*n{1}*...*n{D-2}]
        /// </param>
        /// <returns>The contiguous index.</returns>
        protected int FastAddressToIndex(IList<int> basis)
        {
            return FastAddressToIndex(address, basis);
        }
   
        public int[] Add(IList<int> tuple)
        {
            return Add(this.address, tuple);
        }
        
        public int CompareTo(IAddress address)
        {
            int dim = address.Dim;
            if (dim != this.Dim)
            {
                throw new System.ArgumentOutOfRangeException("The address must be of same dimension to be comparable");
            }
            // Backward Loop
            for (int d = dim; d > 0; d--)
            {
                int i1 = this[d];
                int i2 = address[d];

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

        /// <summary>
        /// String representation of a tuple.
        /// </summary>
        /// <param name="address">The tuple.</param>
        /// <returns>A string representing the tuple (i0,i1,...,in-1).</returns>
        public override string ToString()
        {
            var s = "(" + this.address[0];
            for (int i = 1; i < this.address.Length; i++)
            {
                s += ", " + address[i];
            }
            return s += ")";
        }

        #endregion

        #region STATIC METHODS

        /// <summary>
        /// Get a tuple from its contiguous index. This is achived by recursive euclidean division.
        /// </summary>
        /// <remarks>
        /// i{0} ∈ [0,n{0}-1] | i{1} ∈ [0,n{1}-1] | ... | i{D-1} ∈ [0,n{D-1}-1]
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// index = i0 + n0 * (i1 + n1 * (i2 + n2 * (i3 + ... + n{D-2} * i{D-1})))
        /// </remarks>
        /// <param name="index">The index to convert.</param>
        /// <param name = "count">
        /// An array that holds for each dimension the number of indices tu use 
        /// for the computation of the Address's index.
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// </param>
        /// <returns>The tuple corresponding to the given index.</returns>
        public static int[] IndexToAddress(int index, IList<int> count)
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

        /// <summary>
        /// For a given indexRange, compute the contiguous index of this tuple.
        /// i{0} ∈ [0,n{0}-1] | i{1} ∈ [0,n{1}-1] | ... | i{D-1} ∈ [0,n{D-1}-1]
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// index = i0 + n0 * (i1 + n1 * (i2 + n2 * (i3 + ... + n{D-2} * i{D-1})))
        /// </summary>
        /// <param name="address">
        /// The address to convert.
        /// </param>
        /// <param name="count">
        /// An array that holds for each dimension the number of indices tu use 
        /// for the computation of the Address's index.
        /// Count = [n{0}, n{1}, ..., n{D-1}]
        /// </param>
        /// <returns>The contiguous index.</returns>
        public static int AddressToIndex(IList<int> address, IList<int> count)
        {
            int index = address[0];
            int nprod = 1;
            for (int i = 0; i < address.Count - 1; i++)
            {
                nprod *= count[i];
                index += nprod * address[i + 1];
            }
            return index;
        }

        /// <summary>
        /// Converts an address to the corresponding index.
        /// </summary>
        /// <param name="basis">The conversion basis to use for the conversion.</param>
        /// <param name="address">The address to convert.</param>
        /// <returns>The index.</returns>
        public static int FastAddressToIndex(IList<int> address, IList<int> basis)
        {
            int index = 0;
            for (int d = 0; d < address.Count; d++)
            {
                index += basis[d] * address[d];
            }
            return index;
        }

        /// <summary>
        /// Addition 2 tuples of same dimension.
        /// (i1{0}, i1{1}, ..., i1{N-1}) + (i2{0}, i2{1}, ..., i2{N-1})
        /// = (i1{0} + i2{0}, i1{1} + i2{1}, ..., i1{N1-1} + i2{N-1})
        /// </summary>
        /// <param name="tuple1">First tuple (i1{0}, i1{1}, ..., i1{N-1}).</param>
        /// <param name="tuple2">Second tuple (i2{0}, i2{1}, ..., i2{N-1}).</param>
        /// <returns>The sum as.</returns>
        public static int[] Add(IList<int> tuple1, IList<int> tuple2)
        {
            var tuple = tuple1.ToArray<int>();
            for (int i = 0; i < tuple.Length; i++)
            {
                tuple[i] += tuple2[i];
            }
            return tuple;
        }

        /// <summary>
        /// Cartesian product of 2 tuples.
        /// (i1{0}, i1{1}, ..., i1{N1-1}) x (i2{0}, i2{1}, ..., i2{N2-1})
        /// = (i1{0}, i1{1}, ..., i1{N1-1}, i2{0}, i2{1}, ..., i2{N2-1})
        /// </summary>
        /// <param name="tuple1">First tuple (i1{0}, i1{1}, ..., i1{N1-1}).</param>
        /// <param name="tuple2">Second tuple (i2{0}, i2{1}, ..., i2{N2-1}).</param>
        /// <returns>A tuple.</returns>
        public static int[] CartesianProduct(IList<int> tuple1, IList<int> tuple2)
        {
            int n1 = tuple1.Count;
            int n2 = tuple2.Count;
            var tuple = new int[n1 + n2];
            for (int i = 0; i < n1; i++)
            {
                tuple[i] = tuple1[i];
            }
            for (int i = 0; i < n2; i++)
            {
                tuple[n1 + i] = tuple2[i];
            }
            return tuple;
        }

        /// <summary>
        /// Cartesian product of 2 tuples.
        /// (i1{0}, i1{1}, ..., i1{N1-1}) x (i2{0}, i2{1}, ..., i2{N2-1})
        /// = (i1{0}, i1{1}, ..., i1{N1-1}, i2{0}, i2{1}, ..., i2{N2-1})
        /// </summary>
        /// <param name="tuple1">First tuple (i1{0}, i1{1}, ..., i1{N1-1}).</param>
        /// <param name="tuple2">Second tuple (i2{0}, i2{1}, ..., i2{N2-1}).</param>
        /// <returns>A tuple.</returns>
        public static double[] CartesianProduct(IList<double> tuple1, IList<double> tuple2)
        {
            int n1 = tuple1.Count;
            int n2 = tuple2.Count;
            var tuple = new double[n1 + n2];
            for (int i = 0; i < n1; i++)
            {
                tuple[i] = tuple1[i];
            }
            for (int i = 0; i < n2; i++)
            {
                tuple[n1 + i] = tuple2[i];
            }
            return tuple;
        }

        /// <summary>
        /// Partition a tuple (invert of the cartesian product) at given index
        /// </summary>
        /// <param name="tuple">The tuple to partition.</param>
        /// <param name="indexes">A list of indexes such that Pij = [i,j[ </param>
        /// <returns>A list of tuples {Pij}.</returns>
        public static int[][] Partition(IList<int> tuple, IList<int> indexes)
        {
            // make sure indexes are in [1,Dim-1] where Dim = tuple.Count
            var Dim = tuple.Count;
            if (indexes.Min() < 1)
                throw new ArgumentOutOfRangeException("Partition Indexes must be in [1,Dim-1] where Dim = tuple.Count");
            if (indexes.Max() > Dim - 1)
                throw new ArgumentOutOfRangeException("Partition Indexes must be in [1,Dim-1] where Dim = tuple.Count");

            // make sur indexes are sorted in ascent order
            var sorted_indexes = new int[indexes.Count + 2];
            sorted_indexes[0] = 0;
            indexes.CopyTo(sorted_indexes, 1);
            Array.Sort(sorted_indexes);
            sorted_indexes[sorted_indexes.Length - 1] = tuple.Count;

            int np = indexes.Count + 1;
            var partitions = new int[np][];

            for (int p = 0; p < np; p++)
            {
                int i0 = sorted_indexes[p];
                int i1 = sorted_indexes[p + 1];
                int n = i1 - i0;

                if (n < 1)
                    throw new ArgumentOutOfRangeException("Partition must have a length > 1");

                partitions[p] = new int[i1 - i0];
                for (int i = i0; i < i1; i++)
                {
                    partitions[p][i - i0] = tuple[i];
                }
            }
            return partitions;
        }

        /// <summary>
        /// Parse a string representation of a tuple of doubles.
        /// </summary>
        /// <param name="str">The input string : "(1.234,2.23,-0.20938)".</param>
        /// <param name="tuple">The parsed tuple as an array of doubles.</param>
        /// <returns>True if parsing was successful.</returns>
        public static bool TryParseDouble(string str, out double[] tuple)
        {
            var values = str.TrimStart('(').TrimEnd(')').Split(',');
            tuple = new double[values.Length];
            for (int i = 0; i < tuple.Length; i++)
                if (!Double.TryParse(values[i], out tuple[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Parse a string representation of a tuple of integers.
        /// </summary>
        /// <param name="str">The input string : "(1,7,-18)".</param>
        /// <param name="tuple">The parsed tuple as an array of integers.</param>
        /// <returns>True if parsing was successful.</returns>
        public static bool TryParseInt(string str, out int[] tuple)
        {
            var values = str.TrimStart('(').TrimEnd(')').Split(',');
            tuple = new int[values.Length];
            for (int i = 0; i < tuple.Length; i++)
                if (!Int32.TryParse(values[i], out tuple[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// Gets the string representation of a tuple of integers.
        /// </summary>
        /// <param name="tuple">The tuple of integers.</param>
        /// <returns>The string representation of the tuple.</returns>
        public static string ToString(IList<double> tuple)
        {
            var s = "(" + String.Format("{0:F2}", tuple[0]);
            for (int i = 1; i < tuple.Count; i++)
            {
                s += ", " + String.Format("{0:F2}", tuple[i]);
            }
            return s += ")";
        }

        /// <summary>
        /// Gets the string representation of a tuple of doubles.
        /// </summary>
        /// <param name="tuple">The tuple of doubles.</param>
        /// <returns>The string representation of the tuple.</returns>
        public static string ToString(IList<int> tuple)
        {
            var s = "(" + tuple[0];
            for (int i = 1; i < tuple.Count; i++)
            {
                s += ", " + tuple[i];
            }
            return s += ")";
        }

        #endregion    
    }


}
