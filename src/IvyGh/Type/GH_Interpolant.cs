using Grasshopper.Kernel.Types;
using System;
using IvyCore.Interpolation;

namespace IvyGh.Type
{
    public class GH_Interpolant : GH_Goo<Interpolant>
    {
        
        #region FIELDS

        public override bool IsValid { get { return true; } }

        public override string TypeDescription { get { return "An Interpolant object."; } }

        public override string TypeName { get { return "Interpolant"; } }
        
        #endregion

        #region CONSTRUCTOR
        public GH_Interpolant() {}
        public GH_Interpolant(GH_Interpolant ghInterpolant)
        {
            // no deep copy ??
            this.Value = ghInterpolant.Value;
        }
        public GH_Interpolant(Interpolant interpolant)
        {
            this.Value = interpolant;
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
            if (source.GetType() == typeof(Interpolant))
            {
                this.Value = (Interpolant)source;
                return true;
            }
            return false;
        }

            #endregion

            #region SERIALIZATION
            #endregion
        }
}
