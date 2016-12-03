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
            var nperm = Grid.PermutationCount;
            var nodes = new Node[nperm];

            /// Une cellule est un polytope (ou hypercube) à 2^D sommets
            /// Chaque sommet est obtenu par addition des permutations élémentaires
            /// sur le tuple de la cellule considérée.
            /// 
            /// Examples :
            /// 
            ///     D=1             D=2             D=3
            /// 
            ///     base:  (i)      base:  (i,j)    base:  (i,j,k)
            ///     i=0 : +(0)      i=0 : +(0,0)    i=0 : +(0,0,0)
            ///     i=1 : +(1)      i=1 : +(1,0)    i=1 : +(1,0,0)
            ///                     i=2 : +(0,1)    i=2 : +(0,1,0)
            ///                     i=3 : +(1,1)    i=3 : +(1,1,0)
            ///                                     i=4 : +(0,0,1)
            ///                                     i=5 : +(1,0,1)
            ///                                     i=6 : +(0,1,1)
            ///                                     i=7 : +(1,1,1)
            /// 
            /// On note :
            ///     
            ///     i   :   l'indice de la permutation i ∈ [0,2^D - 1]
            ///             peut également être reprenté par un tuple de dimension D 
            ///             avec des 0 ou 1 (cf example)
            /// 
            ///     X{i}:   le noeud i de coordoonées [x{i}_1, x{i}_2, ..., x{i}_{D-1}] ∈ R^D
            ///     
            ///     Y{i}:   la valeur du champs ∈ R^n (n > 0) à interpoler au noeud X{i}
            ///             Y{i} = f(X{i}) avec f : R^D -> R^n 
        
            ///     V   :   le volume du polytope = 


            for (int i = 0; i < nperm; i++)
            {
                var perm = Grid.Permutations[i];
                var tuple = this.Tuple.Add(perm);

                var index = Grid.NodeIndex(tuple);
                var node = Grid.Nodes[index];
            }

            return nodes;
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
