using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadingDevice.Kernel
{
    /// <summary>
    /// Property object for a truss element in Abaqus. 
    /// </summary>
    public class AbaqusTrussElementProperty : IAbaqusMaterial
    {
        public string AbaqusElementType { get; protected set; }
        public double AbaqusTrussSectionArea { get; protected set; }

        public double E { get; protected set; }
        public double Density { get; protected set; }
        public double Alpha { get; protected set; }

        public AbaqusTrussElementProperty(double S, double E, double d, double α)
        {
            this.AbaqusElementType = "T3D2H";
            this.AbaqusTrussSectionArea = S;
            this.E = E;
            this.Density = d;
            this.Alpha = α;
        }
    }
}
