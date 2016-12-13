using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadingDevice.Kernel
{
    public class Actuator
    {
        public int ShellNode_0 { get; private set; }
        public int ShellNode_1 { get; private set; }

        public int TrussNode_0 { get; private set; }
        public int TrussNode_1 { get; private set; }
        
        public AbaqusTrussElementProperty Property { get; private set; }

        public Actuator(int shellNodeNum_0, int shellNodeNum_1, int trussNodeNum_0, int trussNodeNum_1, AbaqusTrussElementProperty prop)
        {
            ShellNode_0 = shellNodeNum_0;
            ShellNode_1 = shellNodeNum_1;

            TrussNode_0 = trussNodeNum_0;
            TrussNode_1 = trussNodeNum_1;

            Property = prop;
        }
    }
}
