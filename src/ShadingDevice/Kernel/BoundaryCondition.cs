using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadingDevice.Kernel
{
    public class BoundaryCondition
    {
        public int ShellNode { get; private set; }
        public int[] Release { get; private set; }
        public string AbaqusType { get; private set; }

        private BoundaryCondition(int nodeNum, int[] release, string type)
        {
            ShellNode = nodeNum;
            Release = release;
            AbaqusType = type;
        }

        public static BoundaryCondition CreateEncastreBC(int nodeNum)
        {
            return new BoundaryCondition(nodeNum, new int[6] { 0, 0, 0, 0, 0, 0 }, "ENC");
        }

    }
}
