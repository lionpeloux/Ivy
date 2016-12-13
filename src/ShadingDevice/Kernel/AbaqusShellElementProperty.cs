using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadingDevice.Kernel
{
    /// <summary>
    /// Property object for a shell element in Abaqus. 
    /// </summary>
    public class AbaqusShellElementProperty : IAbaqusMaterial
    {
        public string AbaqusElementType { get; protected set; }
        public double AbaqusShellThickness { get; protected set; }

        public double E { get; protected set; }
        public double Density { get; protected set; }
        public double Alpha { get; protected set; }

        public AbaqusShellElementProperty(double e, double E, double d, double α)
        {
            this.AbaqusElementType = "S4R";
            this.AbaqusShellThickness = e;
            this.E = E;
            this.Density = d;
            this.Alpha = α;
        }
    }
}
