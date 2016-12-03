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

        /// <summary>
        /// Return the index of the Nodes that bound the Cell.
        /// This array is ordered according to the permutation scheme.
        /// </summary>
        public int[] VerticesIndex { get; protected set; }

        /// <summary>
        /// Cell Volume for Lerp.
        /// </summary>
        public double Volume { get; protected set; }

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

            var nperm = Grid.PermutationCount;
            var nodes = new Node[nperm];

            // Compute the Cell Volume
            double V = 1;
            for (int d = 0; d < Dim; d++)
            {
                int i = this.Tuple[d];
                double x0 = Grid.Data[d][i];
                double x1 = Grid.Data[d][i + 1];
                V *= (x1 - x0);
            }
            this.Volume = V;

            // Compute Cell's Bounding Nodes Index
            var nodesIndex = new int[Grid.PermutationCount];
            for (int i = 0; i < nodesIndex.Length; i++)
            {
                var index = Grid.NodeIndex(this.Tuple.Add(Grid.Permutations[i]));
                nodesIndex[i] = index;
            }
            this.VerticesIndex = nodesIndex;
        }

        /// <summary>
        /// Multi-Linear interpolation inside the Cell.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double[] LerpCoefficients(Point p)
        {
            // assume that p is in the cell.

            /// Une cellule est un polytope (ou hypercube) à 2^D sommets
            /// Chaque sommet est obtenu par addition des permutations élémentaires
            /// sur le tuple de la cellule considérée.
            /// 
            /// Examples :
            /// 
            ///     D=1             D=2             D=3
            /// 
            ///     base:  (i)      base:  (i,j)    base:  (i,j,k)
            ///     p=0 : +(0)      p=0 : +(0,0)    p=0 : +(0,0,0)
            ///     p=1 : +(1)      p=1 : +(1,0)    p=1 : +(1,0,0)
            ///                     p=2 : +(0,1)    p=2 : +(0,1,0)
            ///                     p=3 : +(1,1)    p=3 : +(1,1,0)
            ///                                     p=4 : +(0,0,1)
            ///                                     p=5 : +(1,0,1)
            ///                                     p=6 : +(0,1,1)
            ///                                     p=7 : +(1,1,1)
            /// 
            /// On note :
            ///     
            ///     i       l'indice de la permutation i ∈ [0,2^D - 1]
            ///             peut également être reprenté par un tuple de dimension D 
            ///             
            ///     tup     la représentation de la permutation sous forme de tuple
            ///             tup = (σ{0}, σ{1}, ..., σ{D-1}) avec σ = [0,1]
            /// 
            ///     X{i}    le noeud i de coordoonées [x0{i}, x1{i}, ..., x{D-1}{i}] ∈ R^D
            ///             en un noeud chaque coordonnée ne prend que l'une des 2 valeurs
            ///             x{k}{i} in [x{k}_0, x{k}_1] k ∈ [0,D-1]
            ///             
            ///     X       le point à interpoler dans l'espace du polytope et de coordonnées
            ///             [x0, x1, ..., x{D-1}] ∈ R^D
            ///     
            ///     Y{i}    la valeur du champs à interpoler (Y ∈ R^n) au noeud X{i}
            ///             [y0{i}, y1{i}, ..., y{n-1}{i}] ∈ R^n
            ///             Y{i} = f(X{i}) avec f : R^D -> R^n 
            ///     
            ///     Y       le champ interpolé au point X
            ///             [y0, y1, ..., y{n-1}] ∈ R^n
            ///             
            ///     V       le volume du polytope
            ///             V = (x0_1 - x0_0) * (x1_1 - x1_0) * ... * (x{D-1}_1 - x{D-1}_0)
            ///             
            ///     V{i}    le volume de la partition associée à la permutation i
            ///             V{i} = PROD[ 
            ///                          x{k}_1 - x{k}     | si σ{k} = 0
            ///                          x{k}   - x{k}_0)  | si σ{k} = 1
            ///                         ] , k ∈ [0, D-1]
            ///                         
            ///     N{i}    le volume normalisé N{i} = V{i}/V associé à la permutation i
            ///     
            /// On calcul :
            /// 
            ///     Y = Y{0} * N{0} +  Y{1} * N{1} + ... + Y{2^D -1} * N{2^D -1}
            ///

            var nperm = Grid.PermutationCount;
            var N = new double[nperm];

            // Precompute quantities
            // TODO : cell volume could be done only once
            var v0 = new double[Dim];
            var v1 = new double[Dim];

            for (int d = 0; d < Dim; d++)
            {
                int i = this.Tuple[d];

                double x = p[d];
                double x0 = Grid.Data[d][i];
                double x1 = Grid.Data[d][i + 1];

                v0[d] = x - x0;
                v1[d] = x1 - x;
            }

            for (int i = 0; i < nperm; i++)
            {
                double Vi = 1;
                int[] perm = Grid.Permutations[i];

                for (int d = 0; d < Dim; d++)
                {
                    var σ = perm[d];
                    Vi *= v0[d] * σ + (1 - σ) * v1[d];
                    //Vi *= (v0[d]- v1[d]) * σ + v1[d];
                }
                N[i] = Vi / Volume;
            }
            return N;
        }

        public override string ToString()
        {
            return this.Tuple.ToString();
        }
        public string Info()
        {
            return "CELL[" + this.Index + "|" + this.Tuple + "] : volume = " + Volume;
        }
    }
}
