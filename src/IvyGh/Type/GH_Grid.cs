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
    public class GH_Grid : GH_Goo<Grid>
    {
        
        #region FIELDS

        public override bool IsValid { get { return true; } }

        public override string TypeDescription { get { return "A N-dimensional Grid of numbers."; } }

        public override string TypeName { get { return "Grid"; } }
        
        #endregion

        #region CONSTRUCTOR
        public GH_Grid() {}
        public GH_Grid(GH_Grid ghGrid)
        {
            // no deep copy ??
            this.Value = ghGrid.Value;
        }
        public GH_Grid(Grid grid)
        {
            this.Value = grid;
        }
        public GH_Grid(double[][] X)
        {
            this.Value = new Grid(X);
        }
        public GH_Grid(double[][] X, IList<string> labels)
        {
            this.Value = new Grid(X, labels);
        }
        #endregion

        #region INSTANCE METHODS

        public override IGH_Goo Duplicate()
        {
            // should I make a deep copy or not ?
            return new GH_Grid(this.Value.DeepCopy());
        }

        public GH_Grid DeepCopy()
        {
            // should I make a deep copy or not ?
            return new GH_Grid(this.Value.DeepCopy());
        }

        public override object ScriptVariable()
        {
            return this.Value;
        }

        //public override bool CastTo<Q>(ref Q target)
        //{
        //    // Cast data array
        //    if (typeof(Q).IsAssignableFrom(typeof(double)))
        //    {
        //        var tree = new DataTree<double>();
        //        for (int d = 0; d < Value.Dim; d++)
        //        {
        //            var path = new GH_Path(d);
        //            tree.AddRange(Value.Data[d], path);
        //        }
        //        target = (Q)(object)tree;
        //        return true;
        //    }
        //    if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
        //    {
        //        var tree = new DataTree<double>();
        //        for (int d = 0; d < Value.Dim; d++)
        //        {
        //            var path = new GH_Path(d);
        //            tree.AddRange(Value.Data[d], path);
        //        }
        //        target = (Q)(object)tree;
        //        return true;
        //    }

        //    return base.CastTo<Q>(ref target);
        //}

        //public override bool CastFrom(object source)
        //{
        //    //Abort immediately on bogus data.
        //    if (source == null) { return false; }

        //    // create a Grid from a compatible DataTree<double>
        //    // or GH_Structure<GH_Number>

        //    return base.CastFrom(source);
        //}

        public override string ToString()
        {
            return this.Value.ToString();
        }

        #endregion

        #region SERIALIZATION
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("Dim", Value.Dim);
            for (int d = 0; d < Value.Dim; d++)
            {
                int Nd = Value.Data[d].Length;
                writer.SetInt32("NodeCount", d);
                writer.SetString("Label", d, Value.Labels[d]);

                string dimString = "Data[" + d + "]";
                for (int i = 0; i < Value.Data[d].Length; i++)
                {
                    writer.SetDouble(dimString, i, Value.Data[d][i]);
                }
            }
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            int Dim = 0;
            reader.TryGetInt32("Dim", ref Dim);

            var data = new double[Dim][];
            var labels = new string[Dim];

            string label = "";
            int Nd = 0;

            for (int d = 0; d < Dim; d++)
            {
                reader.TryGetString("NodeCount", d, ref label);
                reader.TryGetInt32("NodeCount", d, ref Nd);

                labels[d] = label;
                data[d] = new double[Nd];

                string dimString = "Data[" + d + "]";
                for (int i = 0; i < Nd; i++)
                {
                    reader.TryGetDouble(dimString, i, ref data[d][i]);
                }

                this.Value = new Grid(data, labels);
            }
            return base.Read(reader);
        }
        #endregion
    }
}
