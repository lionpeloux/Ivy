﻿using System;
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
        private double[][] data;    
        private int[] nodeCount;
        private int[] cellCount;
        private int[] nodeIndexBasis;
        private int[] cellIndexBasis;
        private int[][] permutations;
        private Interval[] intervals;
        private Node[] nodes;
        private Cell[] cells;

        /// <summary>
        /// Grid dimension.
        /// </summary>
        public int Dim
        {
            get;
            protected set;
        }

        public double[][] Data { get { return data; } }

        /// <summary>
        /// Number of Nodes in the grid.
        /// </summary>
        public int NodeCount
        {
            get { return Nodes.Length; }
        }

        /// <summary>
        /// Coefficient Basis for the fast computation of Node index.
        /// i{0} ∈ [0,nn{0}-1] | i{1} ∈ [0,nn{1}-1] | ... | i{D-1} ∈ [0,nn{D-1}-1]
        /// NodeCount = [nn{0}, nn{1}, ..., nn{D-1}]
        /// NodeIndexBasis = [1, nn{0}, nn{0}*nn{1}, ..., nn{0}*nn{1}*...*nn{D-2}]
        /// index = i0 + nn0 * (i1 + nn1 * (i2 + nn2 * (i3 + ... + nn{D-2} * i{D-1}))).
        /// index = 1*i0 + nn0*i1 + nn0*nn1*i2 + ... + nn0*nn1*...*nn{D-2}*i{D-1}.
        /// </summary>
        public int[] NodeIndexBasis
        {
            get { return nodeIndexBasis; }
        }

        /// <summary>
        /// Coefficient Basis for the fast computation of Cell index.
        /// i{0} ∈ [0,nc{0}-1] | i{1} ∈ [0,nc{1}-1] | ... | i{D-1} ∈ [0,nc{D-1}-1]
        /// CellCount = [nc{0}, nc{1}, ..., nc{D-1}] 
        /// Recall that : nc{i} = nn{i} - 1
        /// NodeIndexBasis = [1, nc{0}, nc{0}*nc{1}, ..., nc{0}*nc{1}*...*nc{D-2}]
        /// index = i0 + nc0 * (i1 + nc1 * (i2 + n2 * (i3 + ... + nc{D-2} * i{D-1}))).
        /// index = 1*i0 + nc0*i1 + nc0*nc1*i2 + ... + nc0*nc1*...*nc{D-2}*i{D-1}.
        /// </summary>
        public int[] CellIndexBasis
        {
            get { return cellIndexBasis; }
        }

        /// <summary>
        /// Number of Cells in the grid.
        /// </summary>
        public int CellCount
        {
            get { return Cells.Length; }
        }

        /// <summary>
        /// An array that holds for each dimension the number of node coordinates in the grid.
        /// For instance, if a tuple represents a node adress belonging to a grid generated by [[0,1],[-5,10,3]],
        /// NodeDimCount will be [2,3].
        /// 
        /// i{0} ∈ [0,nn{0}-1] | i{1} ∈ [0,nn{1}-1] | ... | i{D-1} ∈ [0,nn{D-1}-1]
        /// </summary>
        public int[] NodeDimCount { get { return nodeCount; } }

        /// <summary>
        /// An array that holds for each dimension the number of node coordinates in the grid
        /// For instance, if a tuple represents a node adress belonging to a grid generated by [[0,1],[-5,10,3]],
        /// CellDimCount will be [1,2].
        /// 
        /// i{0} ∈ [0,ne{0}-1] | i{1} ∈ [0,ne{1}-1] | ... | i{D-1} ∈ [0,ne{D-1}-1]
        /// </summary>
        public int[] CellDimCount { get { return cellCount; } }

        /// <summary>
        /// Get 2^D, the number of permutations nedded to compute cell bounds.
        /// </summary>
        public int PermutationCount
        {
            get { return Permutations.Length; }
        }

        /// <summary>
        /// Get the 2^D permutations nedded to compute cell bounding nodes.
        /// </summary>
        public int[][] Permutations { get { return permutations; } }

        /// <summary>
        /// Ordered list of Intervals in the Grid.
        /// Each dimension has its own Interval object to store its bounds.
        /// </summary>
        public Interval[] Intervals { get { return intervals; } }

        /// <summary>
        /// Ordered list of Nodes in the Grid.
        /// </summary>
        public Node[] Nodes { get { return nodes; } }

        /// <summary>
        /// Ordered list of Cells in the Grid.
        /// </summary>
        public Cell[] Cells { get { return cells; } }

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
            this.Dim = X.Length;
            
            nodeIndexBasis = new int[this.Dim];
            cellIndexBasis = new int[this.Dim];

            data = new double[this.Dim][];
            nodeCount = new int[this.Dim];
            cellCount = new int[this.Dim];
            
            intervals = new Interval[Dim];

            int nperm = (int)Math.Pow(2, this.Dim);
            int nn = 1;
            int nc = 1;
            
            for (int d = 0; d < this.Dim; d++)
            {
                // deep copy of X values
                this.data[d] = X[d].ToArray();

                // enforce ascending order of elements
                Array.Sort<double>(data[d]);

                // number of values for each dimension
                nodeCount[d] = data[d].Length;
                cellCount[d] = data[d].Length - 1;

                // total number of nodes and cells
                nn *= nodeCount[d];
                nc *= cellCount[d];

                // check consistency
                if (nodeCount[d] < 2)
                {
                    throw new System.ArgumentOutOfRangeException(
                        "DEGENERATED DIMENSION : the dimension " + (d + 1) + " must be of size > 1"
                        );
                }

                // populate intervals
                intervals[d] = new Interval(data[d].First<double>(), data[d].Last<double>());
            }

            // fast index computation (precompute arrays)
            nodeIndexBasis[0] = 1;
            cellIndexBasis[0] = 1;
            for (int d = 1; d < Dim; d++)
            {
                var n = nodeCount[d - 1];
                nodeIndexBasis[d] = nodeIndexBasis[d - 1] * n;
                cellIndexBasis[d] = cellIndexBasis[d - 1] * (n - 1);
            }

            // Populate the grid with Nodes and Cells
            nodes = new Node[nn];
            cells = new Cell[nc];
            permutations = new int[nperm][];
            Populate();
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
            int index;

            // Precompute CELL PERMUTAIONS
            index = 0;
            var dimCount = new int[Dim];
            for (int i = 0; i < Dim; i++) dimCount[i] = 2;
            DoTupleRecursion(ref index, dimCount, new List<int>(), AddPermutation);

            // Populate NODES        
            index = 0;
            DoTupleRecursion(ref index, NodeDimCount, new List<int>(), AddNode);

            // Populate CELLS     
            index = 0;
            DoTupleRecursion(ref index, CellDimCount, new List<int>(), AddCell);        
        }
        private void DoTupleRecursion(ref int index, int[] dimCount, List<int> indices, Action<Grid, IList<int>, int> f)
        {
            int rank = Dim - indices.Count;

            if (rank == 1)
            {
                var tuple = new List<int>();
                tuple.Add(0);
                tuple.AddRange(indices);

                // append tuples
                for (int k = 0; k < dimCount[0]; k++)
                {
                    tuple[0] = k;
                    f(this, tuple, index);
                    index += 1;
                }
            }
            else
            {
                for (int l = 0; l < dimCount[rank - 1]; l++)
                {
                    var indices_l = indices.ToList<int>();
                    indices_l.Insert(0, l);
                    DoTupleRecursion(ref index, dimCount, indices_l, f);
                }
            }
        }

        // Delegates that do specific work in DoTupleRecursion()
        private static void AddNode(Grid grid, IList<int> tuple, int index)
        {
            var node = new Node(grid, tuple);
            for (int d = 0; d < tuple.Count; d++)
            {
                node[d] = grid.data[d][tuple[d]];
            }
            //nodes.Add(new Node(this, tuple, coordinates));
            grid.nodes[index] = node;
        }
        private static void AddCell(Grid grid, IList<int> tuple, int index)
        {
            var cell = new Cell(grid, tuple);
            grid.cells[index] = cell;
        }
        private static void AddPermutation(Grid grid, IList<int> tuple, int index)
        {
            var perm = tuple.ToArray<int>();
            grid.permutations[index] = perm;
        }

        // Lerp


        // info
        public override string ToString()
        {
            return String.Format("GRID (dim = {0} | nodes = {1} | cells = {2}", Dim, NodeCount, CellCount);
        }
        public string Info()
        {
            var s = new List<string>();

            s.Add(String.Format("GRID (dim = {0} | nodes = {1} | cells = {2}", Dim, NodeCount, CellCount));
            s.Add("==========================");
            for (int i = 0; i < Dim; i++)
            {
                s.Add(String.Format("DATA[{0}] = {1}", i, ITuple.ToString(data[i])));
            }
            s.Add("");
            s.Add("NODES");
            s.Add("==========================");
            for (int i = 0; i < NodeCount; i++)
            {
                s.Add(Nodes[i].Info());
            }
            s.Add("");
            s.Add("CELLS");
            s.Add("==========================");
            for (int i = 0; i < CellCount; i++)
            {
                s.Add(Cells[i].Info());
            }
            s.Add("");
            s.Add("PERMUTATIONS");
            s.Add("==========================");
            for (int i = 0; i < PermutationCount; i++)
            {
                s.Add("PERM[" + i + "] = " + Tuple.ToString(Permutations[i]));
            }

            return String.Join(Environment.NewLine, s);
        }

        // Index Helper
        public int NodeIndex(IList<int> tuple)
        {
            return Tuple.FastIndexFromTuple(this.NodeIndexBasis, tuple);
        }
        public int CellIndex(IList<int> tuple)
        {
            return Tuple.FastIndexFromTuple(this.CellIndexBasis, tuple);
        }
    }
}
