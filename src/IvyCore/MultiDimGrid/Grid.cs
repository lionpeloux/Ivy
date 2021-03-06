﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.MultiDimGrid
{

    /// <summary>
    /// A N-grid composed by a set of N-nodes each of dimension D.
    /// Each dimension (di) in [1,D] is composed of a finite set of Ni values.
    /// Thus, the grid has 'Count = N1 x N2 x ... x ND' of N-nodes.
    /// </summary>
    public class Grid
    {

        #region FIELDS
        private double[][] data;    
        private int[] nodeCount;
        private int[] cellCount;
        private int[] nodeIndexBasis;
        private int[] cellIndexBasis;
        private int[][] permutations;
        private string[] labels;
        private Interval[] intervals;
        private Node[] nodes;
        private Cell[] cells;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Grid dimension.
        /// </summary>
        public int Dim
        {
            get;
            protected set;
        }

        /// <summary>
        /// Internal data as a sorted double[][] array.
        /// </summary>
        public double[][] Data { get { return data; } }

        /// <summary>
        /// A list of labels, one for each dimension. Optional.
        /// Can be used for some UI functionalities.
        /// </summary>
        public string[] Labels { get { return labels; } }

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

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a N-Grid of dimension D. With string labels for each dimension.
        /// Does a deep copy of input parameters.
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
        /// <param name="labels">
        /// A list of string labels, one for each dimension.
        /// </param>
        public Grid(double[][] X, IList<string> labels)
        {
            this.Dim = X.Length;

            if (labels.Count != X.Length)
            {
                throw new System.ArgumentOutOfRangeException("Lables must be of size " + Dim);
            }
            
            nodeIndexBasis = new int[this.Dim];
            cellIndexBasis = new int[this.Dim];

            this.data = new double[this.Dim][]; // DeepCopy of data
            this.nodeCount = new int[this.Dim];
            this.cellCount = new int[this.Dim];
            this.labels = labels.ToArray<string>(); // DeepCopy of labels
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
        /// Initialize a N-Grid of dimension D. With empty string labels for each dimension.
        /// Does a deep copy of input parameters.
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
        public Grid(double[][] X):this(X, new string[X.Length])
        {
            for (int d = 0; d < Dim; d++)
            {
                labels[d] = "";
            }
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
        #endregion

        #region INSTANCE METHODS
        /// <summary>
        /// Get a normalized version of this grid.
        /// </summary>
        /// <returns>A normalized grid.</returns>
        public Grid Normalize()
        {
            // create a new double[][] representation
            var data = new double[Dim][];
            var labels = new string[Dim];

            for (int d = 0; d < data.Length; d++)
            {
                int nd = Data[d].Length;
                data[d] = new double[nd];
                labels[d] = this.labels[d];
                for (int i = 0; i < data[d].Length; i++)
                {
                    data[d][i] = Intervals[d].Normalize(Data[d][i]);
                }
            }

            return new Grid(data);
        }

        /// <summary>
        /// Deep copy. 
        /// </summary>
        /// <returns>An identic grid as an independent object.</returns>
        public Grid DeepCopy()
        {
            // as far as Grid is itself deep copying data & labels, this should work.
            return new Grid(this.data, this.labels);
        }

        /// <summary>
        /// Shallow copy. The new instance will hold a reference to the data and the labels of the same grid. 
        /// </summary>
        /// <returns>An identic grid as an independent object.</returns>
        public Grid ShallowCopy()
        {
            // Do I need to provide something like this ??
            throw new NotImplementedException();
        }

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
                if(Labels[i] != "")
                {
                    s.Add(String.Format("DATA[{0}|{1}] = {2}", i, Labels[i], IAddress.ToString(data[i])));
                }
                else
                {
                    s.Add(String.Format("DATA[{0}] = {1}", i, IAddress.ToString(data[i])));
                }
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
                s.Add("PERM[" + i + "] = " + Address.ToString(Permutations[i]));
            }

            return String.Join(Environment.NewLine, s);
        }
        #endregion

        #region OPERATOR
        public static Grid operator *(Grid grid1, Grid grid2)
        {
            return CartesianProduct(grid1, grid2);
        }
        #endregion

        #region STATIC METHODS

        /// <summary>
        /// Cartesian product of 2 grids I1 x I2 = I
        /// (I1{0}xI1{1}x...xI1{N1-1}) x (I2{0}xI2{1}x...xI2{N2-1})
        /// = I1{0}xI1{1}x...xI1{N1-1}xI2{0}xI2{1}x...xI2{N2-1}
        /// </summary>
        /// <param name="grid1">First Grid I1{0}xI1{1}x...xI1{N1-1}.</param>
        /// <param name="grid2">Second Grid I2{0}xI2{1}x...xI2{N2-1}.</param>
        /// <returns>The cartesian product as a new Grid.</returns>    
        public static Grid CartesianProduct(Grid grid1, Grid grid2)
        {
            int dim1 = grid1.Dim;
            int dim2 = grid2.Dim;

            var data = new double[dim1 + dim2][];
            var labels = new string[dim1 + dim2];

            for (int d = 0; d < dim1; d++)
            {
                data[d] = grid1.Data[d].ToArray<double>();
                labels[d] = grid1.Labels[d];
            }
            for (int d = 0; d < dim2; d++)
            {
                data[dim1 + d] = grid2.Data[d].ToArray<double>();
                labels[dim1 + d] = grid2.Labels[d];
            }
            return new Grid(data, labels);
        }

        /// <summary>
        /// Cartesian product of 2 or more Grids.
        /// </summary>
        /// <param name="grids">2 or more Grids : I0, I1, ..., I{N-1}</param>
        /// <returns>The cartesian product as a Grid :  I0xI1x...xI{N-1}.</returns>
        public static Grid CartesianProduct(params Grid[] grids)
        {
            return CartesianProduct(grids);
        }

        /// <summary>
        /// Cartesian product of 2 or more Grids.
        /// </summary>
        /// <param name="grids">2 or more Grids : I0, I1, ..., I{N-1}</param>
        /// <returns>The cartesian product as a Grid :  I0xI1x...xI{N-1}.</returns>
        public static Grid CartesianProduct(IList<Grid> grids)
        {
            int n = grids.Count;

            if (n < 2)
            {
                throw new System.ArgumentOutOfRangeException("The number of input grids must be > 1.");
            }

            int dim = 0;

            for (int i = 0; i < n; i++)
            {
                dim += grids[i].Dim;
            }

            var data = new double[dim][];
            var labels = new string[dim];
            int count = 0;

            for (int i = 0; i < n; i++)
            {
                var di = grids[i].Dim;
                for (int d = 0; d < di; d++)
                {
                    data[count + d] = grids[i].Data[d].ToArray<double>();
                    labels[count + d] = grids[i].Labels[d];
                }
                count += di;
            }

            return new Grid(data, labels);
        }

        /// <summary>
        /// Test if the data is equal in the two grids.
        /// </summary>
        /// <param name="grid1">First Grid.</param>
        /// <param name="grid2">Second Grid.</param>
        /// <returns>True if data are equal. False otherwise.</returns>
        public static bool IsEqualData(Grid grid1, Grid grid2)
        {
            if (grid1.Data.Length != grid2.Data.Length)
                return false;

            for (int i = 0; i < grid1.Data.Length; i++)
            {
                if (grid1.Data[i].Length != grid2.Data[i].Length)
                    return false;

                for (int j = 0; j < grid1.Data[i].Length; j++)
                {
                    if (grid1.Data[i][j] != grid2.Data[i][j])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Test if the labels are equal in the two grids.
        /// </summary>
        /// <param name="grid1">First Grid.</param>
        /// <param name="grid2">Second Grid.</param>
        /// <returns>True if all labels are equal. False otherwise.</returns>
        public static bool IsEqualLabels(Grid grid1, Grid grid2)
        {
            if (grid1.Dim != grid2.Dim)
                return false;

            for (int d = 0; d < grid1.Data.Length; d++)
            {
                if (grid1.Labels[d] != grid2.Labels[d])
                    return false;
            }
            return true;
        }
        #endregion

        #region INDEX HELPER
        public int NodeAddressToIndex(IList<int> address)
        {
            return Address.FastAddressToIndex(address, this.NodeIndexBasis);
        }
        public int CellAddressToIndex(IList<int> address)
        {
            return Address.FastAddressToIndex(address, this.CellIndexBasis);
        }
        #endregion
    }
}
