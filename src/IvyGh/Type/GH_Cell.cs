using Grasshopper.Kernel.Types;
using System;
using IvyCore.MultiDimGrid;

namespace IvyGh.Type
{
    public class GH_Cell : GH_Goo<Cell>
    {
        
        #region FIELDS

        public override bool IsValid { get { return true; } }

        public override string TypeDescription { get { return "A Cell in a Grid."; } }

        public override string TypeName { get { return "Cell"; } }
        
        #endregion

        #region CONSTRUCTOR
        public GH_Cell() {}
        public GH_Cell(GH_Cell ghCell)
        {
            // no deep copy ??
            this.Value = ghCell.Value;
        }
        public GH_Cell(Cell cell)
        {
            this.Value = cell;
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
            if (source.GetType() == typeof(Cell))
            {
                this.Value = (Cell)source;
                return true;
            }
            return false;
        }

            #endregion

            #region SERIALIZATION
            #endregion
        }
}
