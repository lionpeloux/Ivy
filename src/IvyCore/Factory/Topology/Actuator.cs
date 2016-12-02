using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Factory
{
    /// <summary>
    /// An actuator. A truss between to nodes of a given shell.
    /// </summary>
    public class Actuator
    {
        public AbaqusTrussElementProperty Property { get; protected set; }
        public double Length { get; protected set; }

        public int NodeNum_0_Actuator { get; protected set; }
        public int NodeNum_0_Shell { get; protected set; }

        public int NodeNum_1_Actuator { get; protected set; }
        public int NodeNum_1_Shell { get; protected set; }

        public Actuator(int nshell_0, int nshell_1, AbaqusTrussElementProperty prop)
        {
            this.NodeNum_0_Shell = nshell_0;
            this.NodeNum_1_Shell = nshell_1; 
            this.Property = prop;
        }

    }
}
