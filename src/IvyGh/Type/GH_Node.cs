using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GH_IO.Serialization;
using IvyCore.Parametric;
using Grasshopper.Kernel.Data;
using Grasshopper;
using Grasshopper.Kernel;

namespace IvyGh.Type
{
    public class GH_Node : GH_Goo<Node>
    {
        
        #region FIELDS

        public override bool IsValid { get { return true; } }

        public override string TypeDescription { get { return "A Node in a Grid."; } }

        public override string TypeName { get { return "Node"; } }
        
        #endregion

        #region CONSTRUCTOR
        public GH_Node() {}
        public GH_Node(GH_Node ghNode)
        {
            // no deep copy ??
            this.Value = ghNode.Value;
        }
        public GH_Node(Node node)
        {
            this.Value = node;
        }
        #endregion

        #region INSTANCE METHODS

        public override object ScriptVariable()
        {
            return this.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            //Abort immediately on bogus data.
            if (source == null) { return false; }

            //Use the Grasshopper Integer converter. By specifying GH_Conversion.Both 
            //we will get both exact and fuzzy results. You should always try to use the
            //methods available through GH_Convert as they are extensive and consistent.
            int val;
            if (source.GetType() == typeof(Node))
            {
                this.Value = (Node)source;
                return true;
            }
            return false;
        }

            #endregion

            #region SERIALIZATION
            #endregion
        }
}
