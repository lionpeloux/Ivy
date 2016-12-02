using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{

    /// <summary>
    /// A N-grid composed by a set of N-nodes each of dimension D.
    /// Each dimension (di) in [1,D] is composed of a finite set of Ni values.
    /// Thus, the grid has 'Count = N1 x N2 x ... x ND' of N-nodes.
    /// </summary>
    public class Grid
    {
        private int dim;
        private int n;
        private double[][] range;
        private int[] rangeCount;
        private List<Node> nodes;
        private List<int[]> tuples;
        private List<Interval> intervals;

        /// <summary>
        /// Number of N-nodes in the grid.
        /// </summary>
        public int NodeCount
        {
            get { return n; }
        }

        /// <summary>
        /// Grid dimension.
        /// </summary>
        public int Dim
        {
            get { return dim; }
        }

        /// <summary>
        /// Initialize a N-Grid of dimension D.
        /// </summary>
        /// <param name="X">
        /// X is a 2 dimensionnal array that gives for each dimension the discrete states to consider :
        /// 
        ///     - X[0]      = [x{0}_0, x{0}_1, ..., x{0}_(N{0}-1)]
        ///     - X[1]      = [x{1}_0, x{1}_1, ..., x{1}_(N{1}-1)]
        ///     - ...       ...
        ///     - X[i]      = [x{i}_0, x{i}_1, ..., x{i}_(N{i}-1)]
        ///     - ...       ...
        ///     - X[D-1]    = [x{D-1}_0, x{D-1}_1, ..., x{D-1}_(N{D-1}-1)]
        /// 
        /// Where N is the dimension of the N-Grid.
        /// X[i] = [x{i}_0, x{i}_1, ..., x{i}_(N{i}-1)] must be sorted in ascending order.
        /// </param>
        public Grid(double[][] X)
        {
            this.dim = X.Length;

            this.range = new double[this.Dim][];
            this.rangeCount = new int[this.Dim];

            for (int d = 0; d < this.Dim; d++)
            {
                // deep copy of X values
                range[d] = X[d].ToArray();

                // enforce ascending order of elements
                Array.Sort<double>(range[d]);

                // number of values for each dimension
                rangeCount[d] = range[d].Length;
            }

            // Populate the grid with tuples and N-nodes
            Populate();

            this.n = tuples.Count;
        }

        /// <summary>
        /// 
        /// combinatorial generation of grid nodes :
        /// 
        /// n = 0
        /// node[n + 0]     = [x{0}_0       , x{1}_0        , ..., x{D-1}_0]
        /// node[n + 1]     = [x{0}_1       , x{1}_0        , ..., x{D-1}_0]
        /// ...             ...
        /// node[n + N1-1]  = [x{0}_(N1-1)  , x{1}_0        , ..., x{D-1}_0]
        ///
        /// n = N1 * 1
        /// node[n + 0]     = [x{0}_0       , x{1}_1        , ..., x{D-1}_0]
        /// node[n + 1]     = [x{0}_1       , x{1}_1        , ..., x{D-1}_0]
        /// ...             ...
        /// node[n + N1-1]  = [x{0}_(N1-1)  , x{1}_1        , ..., x{D-1}_0]
        ///
        /// n = N1 * 2
        /// node[n + 0]     = [x{0}_0       , x{1}_2        , ..., x{D-1}_0]
        /// node[n + 1]     = [x{0}_1       , x{1}_2        , ..., x{D-1}_0]
        /// ...             ...
        /// node[n + N-1]  = [x{0}_(N1-1)  , x{1}_2         , ..., x{D-1}_0]
        /// 
        /// ...
        /// 
        /// n = N1 * (N2-1)
        /// node[n + 0]     = [x{0}_0       , x{1}_(N2-1)   , ..., x{D-1}_0]
        /// node[n + 1]     = [x{0}_1       , x{1}_(N2-1)   , ..., x{D-1}_0]
        /// ...             ...
        /// node[n + N1-1]  = [x{0}_(N1-1)  , x{1}_(N2-1)   , ..., x{D-1}_0]
        /// 
        /// ...
        /// 
        /// n = N1 * N2 * ... * N{D-1} x 1
        /// node[n + 0]     = [x{0}_0       , x{1}_0        , ..., x{D-1}_1]
        /// node[n + 1]     = [x{0}_1       , x{1}_0        , ..., x{D-1}_1]
        /// ...             ...
        /// node[n + N1-1]  = [x{0}_(N1-1)  , x{1}_0        , ..., x{D-1}_1]
        /// 
        /// ...
        /// 
        /// n = N1 * N2 * ... * (N{D-1}-1)
        /// node[n + 0]     = [x{0}_0       , x{1}_0        , ..., x{D-1}_(N{D}-1)]
        /// node[n + 1]     = [x{0}_1       , x{1}_0        , ..., x{D-1}_(N{D}-1)]
        /// ...             ...
        /// node[n + N1-1]  = [x{0}_(N1-1)  , x{1}_0        , ..., x{D-1}_(N{D}-1)]
        /// 
        /// ...
        /// 
        /// The nuplet  : [i{0}, i{1}, ..., i{D-1}]
        /// The node    : [x{0}[i{0}], x{1}[i{1}], ..., x{D-1}[i{D-1}]]
        /// Is at index : i{0} + N{0}*i{1} + N{0}*N{1}*i{2} + ... + N{0}*N{1}*..*N{D-2}*i{D-1}
        /// 
        /// </summary>
        /// <param name="X"></param>
        private void Populate()
        {
            nodes = new List<Node>();
            tuples = new List<int[]>();
            DoTupleRecursion(this.rangeCount, new List<int>(), ref tuples, ref nodes, ref this.range);
        }
        private void DoTupleRecursion(int[] range, List<int> indices, ref List<int[]> tuples, ref List<Node> nodes, ref double[][]X)
        {
            int n = range.Length;
            int rank = n - indices.Count;

            if (rank == 1)
            {
                // compute base index
                int nk = 0;
                int nprod = 1;
                for (int i = 0; i < indices.Count; i++)
                {
                    nprod *= range[i];
                    nk += nprod * indices[i];
                }

                // append tuples
                for (int k = 0; k < range[0]; k++)
                {
                    var index = k + nk;
                    var tuple = new List<int>();
                    tuple.Add(k);
                    tuple.AddRange(indices);
                    tuples.Add(tuple.ToArray());

                    // do what-so ever with the given (index, tuple)
                    var node = new Node(dim);
                    for (int d = 0; d < tuple.Count; d++)
                    {
                        node[d] = X[d][tuple[d]];
                    }
                    nodes.Add(node);
                }
            }
            else
            {
                for (int l = 0; l < range[rank - 1]; l++)
                {
                    var indices_l = indices.ToList<int>();
                    indices_l.Insert(0, l);
                    DoTupleRecursion(range, indices_l, ref tuples, ref nodes, ref X);
                }
            }
        }

        /// <summary>
        /// String representation of a tuple.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <returns>A string representing the tuple (i0,i1,...,in-1).</returns>
        public static string TupleToString(IList<int> tuple)
        {
            var s = "(" + tuple[0];
            for (int i = 1; i < tuple.Count; i++)
            {
                s += ", " + tuple[i];
            }
            return s += ")";
        }

        /// <summary>
        /// Contigous index for tuples.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <param name="range">The array of range int.</param>
        /// <returns>The tuple corresponding to the given index.</returns>
        public static int TupleToIndex(IList<int> tuple, IList<int> range)
        {
            int index = tuple[0];
            int nprod = 1;
            for (int i = 0; i < tuple.Count - 1; i++)
            {
                nprod *= range[i];
                index += nprod * tuple[i + 1];
            }
            return index;
        }
        public int TupleToIndex(IList<int> tuple)
        {
            return TupleToIndex(tuple, this.rangeCount);
        }

        /// <summary>
        /// Get a tuple from its contiguous index.
        /// </summary>
        /// <param name="index">The index to convert.</param>
        /// <param name="range">The int array of range.</param>
        /// <returns>The tuple corresponding to the given index.</returns>
        public static IList<int> IndexToTuple(int index, IList<int> range)
        {
            int n = range.Count;
            var tuple = new int[n];

            int q = index;
            int r;
            for (int i = 0; i < n - 1; i++)
            {
                q = Math.DivRem(q, range[i], out r);
                tuple[i] = r;
            }
            tuple[n - 1] = q;
            return tuple;
        }
        public int[] IndexToTuple(int index)
        {
            return IndexToTuple(index, this.rangeCount).ToArray<int>();
        }

        public override string ToString()
        {
            var s = new List<string>();

            s.Add(String.Format("GRID (dim = {0} | nodeCount = {1}", Dim, NodeCount));
            s.Add("==========================");
            for (int i = 0; i < Dim; i++)
            {
                s.Add(String.Format("D{0} = {1}", i + 1, ToString(range[i])));
            }
            s.Add("");
            s.Add("==========================");
            for (int i = 0; i < NodeCount; i++)
            {
                s.Add(String.Format("{0} = {1} | {2}", i + 1, TupleToString(tuples[i]), nodes[i].ToString()));
            }

            return String.Join(Environment.NewLine, s);
        }

        public static string ToString(double[] array)
        {
            var s = "(" + array[0];
            for (int i = 1; i < array.Length; i++)
            {
                s += ", " + array[i];
            }
            return s += ")";
        }
    }
}
