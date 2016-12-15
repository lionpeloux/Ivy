using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    public class Interpolant
    {
        /// <summary>
        ///  Lerp Coefficients
        /// </summary>
        public double[] LERP { get; protected set; }

        /// <summary>
        /// Index of cell vertices
        /// </summary>
        public int[] VerticesIndex { get; protected set; }

        public Interpolant(double[] LERP, int[] index)
        {
            this.LERP = LERP;
            this.VerticesIndex = index;
        }

        /// <summary>
        /// Interpolate a scalar field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public double Lerp(IList<double> field)
        {
            int nperm = VerticesIndex.Length;
            double v = 0;
            for (int i = 0; i < nperm; i++)
            {
                v += LERP[i] * field[VerticesIndex[i]];
            }
            return v;
        }

        /// <summary>
        /// Interpolate a vector field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public double[] Lerp(IList<double[]> field)
        {
            int nperm = VerticesIndex.Length;
            int dim = field[0].Length;
            var v = new double[dim];
            for (int i = 0; i < nperm; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    v[j] += LERP[i] * field[VerticesIndex[i]][j];
                }
            }
            return v;
        }

        /// <summary>
        /// Interpolate a vector field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public double[] Lerp(double[,] field)
        {
            int nperm = VerticesIndex.Length;
            int dim = field.GetLength(1);
            var v = new double[dim];
            for (int i = 0; i < nperm; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    v[j] += LERP[i] * field[VerticesIndex[i], j];
                }
            }
            return v;
        }
    }
}
