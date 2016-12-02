using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Factory
{
    interface IAbaqusMaterial
    {
        /// <summary>
        /// Young Modulus (Pa).
        /// </summary>
        double E { get; }
        /// <summary>
        /// Density.
        /// </summary>
        double Density { get; }
        /// <summary>
        /// Coefficient of Thermal expansion (K^-1).
        /// </summary>
        double Alpha { get; }
    }
}
