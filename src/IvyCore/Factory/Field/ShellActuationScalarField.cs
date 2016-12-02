using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Factory
{
    public class ShellActuationScalarField
    {
        /// <summary>
        /// The Shell object this field applies to.
        /// </summary>
        public Shell Shell { get; protected set; }

        /// <summary>
        /// Field Name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Field Abaqus Name.
        /// </summary>
        public string AbaqusName { get; protected set; }

        /// <summary>
        /// The ScalarField.
        /// </summary>
        public List<double> Field { get; protected set; }

        public List<Color> GetColorField(Gradient gradient)
        {
            return new List<Color>();
        }
    }

}
