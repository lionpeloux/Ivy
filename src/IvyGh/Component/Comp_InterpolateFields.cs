using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using IvyCore.Parametric;
using Grasshopper.Kernel.Special;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel.Parameters;
using System.Windows.Forms;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using IvyGh.Type;

namespace IvyGh
{
    public class Comp_InterpolateFields : GH_Component
    {
        GH_Grid ghGrid;
        IvyCore.Parametric.Point point;
        double[] coord;

        public override Guid ComponentGuid
        {
            get { return new Guid("{fb7a200f-ce71-4a54-9be1-d9374700b6e5}"); }
        }

        public Comp_InterpolateFields()
          : base("Field Lerp Interpolation", "Lerp",
              "Interpolate a list of fields (as a tree).",
              "Ivy", "Interpolation")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Interpolants", "I", "A list of interpolant.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Fields", "F", "The fields to interpolate.", GH_ParamAccess.tree);
            pManager[0].Optional = false;
            pManager[1].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Interpolated Fields", "IF", "The interpolated fields at the given point in the grid.", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ghInterps = new List<GH_Interpolant>();
            var ghFields = new GH_Structure<GH_Number>();

            if (!DA.GetDataList(0, ghInterps)) { return; }
            if (!DA.GetDataTree(1, out ghFields)) { return; }

            var res = new DataTree<double>();
            

            int n = ghInterps.Count;

            // loop over interpolants
            for (int k = 0; k < n; k++)
            {
                var interpolant = ghInterps[k].Value;

                if (ghFields.Paths[0].Length == 1)
                {
                    // this is a scalar field (dim = 1)
                    // the tree is a list of list (1 level)
                    double fval;
                    for (int i = 0; i < ghFields.Branches.Count; i++)
                    {
                        var path = new GH_Path(k,i);
                        var field = ghFields.Branches[i].ConvertAll<double>(gh => gh.Value);
                        fval = interpolant.Lerp(field);
                        res.Add(fval, path);
                    }
                }
                else
                {
                    // this is a vector field
                    // the tree is 2 level 2 tree
                    int dim = ghFields.Branches[0].Count;
                    int fieldIndex = 0;
                    int nodeIndex = 0;
                    var field = new List<double[]>();
                    double[] fval;

                    for (int i = 0; i < ghFields.PathCount; i++)
                    {
                        var path = ghFields.Paths[i];
                        if (path[0] > fieldIndex)
                        {
                            fval = interpolant.Lerp(field);
                            res.AddRange(fval, new GH_Path(k,fieldIndex));
                            nodeIndex = 0;
                            fieldIndex = path[0];
                            field.Clear();
                        }
                        else
                        {
                            field.Add(ghFields.Branches[i].ConvertAll<double>(gh => gh.Value).ToArray());
                            nodeIndex += 1;
                        }
                    }
                    fval = interpolant.Lerp(field);
                    res.AddRange(fval, new GH_Path(k, fieldIndex));
                }
            }
            

            
            DA.SetDataTree(0, res);
        }



    }
}