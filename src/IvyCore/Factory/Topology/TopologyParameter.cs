using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Factory
{
    public class TopologyParameter
    {
        public string SQLType { get; protected set; }
        public string Name { get; protected set; }
        public double Value { get; set; }

        public TopologyParameter(string name, double value)
        {
            this.SQLType = "REAL";
            this.Value = value;
        }
    }
}
