using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    /// <summary>
    /// A Cell element of 2^D connected Nodes.
    /// </summary>
    public class Cell : ISortedGridElement<Cell>
    {
        private Node[][] nodes;

        /// <summary>
        /// The Grid this Cell belongs to.
        /// </summary>
        public Grid Grid { get; protected set; }

        /// <summary>
        /// The index in the Grid list of cells.
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// The Tuple representing the position of the Cell in the Grid.
        /// </summary>
        public Tuple Tuple { get; protected set; }

        /// <summary>
        /// Cell dimension.
        /// </summary>
        public int Dim { get { return this.Grid.Dim; } }

        public IList<Cell> List
        {
            get
            {
                return this.Grid.Cells;
            }
        }

        public Cell(Grid grid, IList<int> tuple)
        {
            this.Grid = grid;
            this.Tuple = Tuple.CreateCellTuple(grid, tuple);
            this.Index = this.Tuple.Index;
        }

        /// <summary>
        /// Returns the nodes that bounds the given cell in the given dimension.
        /// </summary>
        /// <returns>The 2^D Nodes bounding the cell.</returns>
        public Node[] Bounds()
        {
            var nperm = (int)Math.Pow(2, Dim);
            var nodes = new Node[nperm];
            var permutations = new List<int[]>();

            /// Compute the perumtation tuples to construct the bounding nodes
            /// 
            /// For Dim = 2 :
            /// (0,0)
            /// (1,0)
            /// (0,1)
            /// (1,1)
            ///
            /// For Dim = 3
            /// (0,0,0)
            /// (1,0,0)
            /// (0,1,0)
            /// (1,1,0)
            /// (0,0,1)
            /// (1,0,1)
            /// (0,1,1)
            /// (1,1,1)
            /// 
            /// For Dim = D, this leads to 2^D permutations
            /// 
            var dimCount = new int[Dim];
            for (int i = 0; i < Dim; i++) dimCount[i] = 1;
            DoTupleRecursion(dimCount, new List<int>(), ref permutations);

            return nodes;
        }
        private void DoTupleRecursion(int[] dimCount, List<int> indices, ref List<int[]> permutations)
        {
            int rank = dimCount.Length - indices.Count;

            if (rank == 1)
            {
                var tuple = new List<int>();
                tuple.Add(0);
                tuple.AddRange(indices);

                // append tuples
                for (int k = 0; k < dimCount[0]; k++)
                {
                    tuple[0] = k;
                    permutations.Add(tuple.ToArray<int>());
                }
            }
            else
            {
                for (int l = 0; l < dimCount[rank - 1]; l++)
                {
                    var indices_l = indices.ToList<int>();
                    indices_l.Insert(0, l);
                    DoTupleRecursion(dimCount, indices_l, ref permutations);
                }
            }
        }


        public override string ToString()
        {
            return this.Tuple.ToString();
        }
        public string Info()
        {
            return "CELL[" + this.Index + "|" + this.Tuple + "]";
        }
    }
}
