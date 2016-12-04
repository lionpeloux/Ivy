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

namespace IvyGh
{
    public class GHComponentGridSlider : GH_Component, IGH_VariableParameterComponent
    {
        Grid grid;
        IvyCore.Parametric.Point point;
        GH_Structure<GH_Number> range;

        bool hasValidGrid = false;
        bool hasValidControls = false;

        public override Guid ComponentGuid
        {
            get { return new Guid("{479434ce-3e14-4f0b-abec-2bc675c40eb4}"); }
        }

        public GHComponentGridSlider()
          : base("Grid Slider", "Slider",
              "Construct a multidimensional grid from a tree. Gives controls to explore the grid (see right clic menu).",
              "Ivy", "Test")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter(
                "Range",
                "range",
                "Grid definition as a tree. Branch i correspond to dimension i with ni values.",
                GH_ParamAccess.tree
                );
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "info", "Grid info.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Active Point", "C", "Selected point on the grid.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Active Cell", "P", "Active cell index.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Grid Nodes", "N", "All the nodes from the grid.", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetDataTree(0, out range)) {
                return;
            }
            var X = GridFromTree(range);

            // test if a valid grid exists
            if (!hasValidGrid)
            {             
                CreateGrid(X);
                DA.SetData(0, grid.Info());
                return;
            }

            // Test if the grid has changed
            if (GridHasChanged(X))
            {
                CreateGrid(X);
                DA.SetData(0, grid.Info());
                return;
            }

            // If controls are valid, the compute the point
            if (hasValidControls)
            {
                UpdatePoint(DA);
                DA.SetData(0, grid.Info());
                DA.SetDataList(1, point.Coord);
                DA.SetData(2, point.CellIndex());
                return;
            }        
        }

        private void UpdatePoint(IGH_DataAccess DA)
        {
            for (int d = 0; d < grid.Dim; d++)
            {
                DA.GetData<double>(1 + d, ref point.Coord[d]);
            }
        }
        private bool GridHasChanged(double[][] X)
        {
            if (grid.Data.Length != X.Length)
                return true;

            for (int i = 0; i < grid.Data.Length; i++)
            {
                if (grid.Data[i].Length != X[i].Length)
                    return true;

                for (int j = 0; j < grid.Data[i].Length; j++)
                {
                    if (grid.Data[i][j] != X[i][j])
                        return true;
                }
            }
            return false;
        }
        private double[][] GridFromTree(GH_Structure<GH_Number> range)
        {
            int dim = range.Branches.Count;
            var X = new double[dim][];
            for (int d = 0; d < dim; d++)
            {
                var branche = range.Branches[d];
                X[d] = new double[branche.Count];

                for (int i = 0; i < X[d].Length; i++)
                {
                    X[d][i] = branche[i].Value;
                }
                Array.Sort<double>(X[d]);
            }
            return X;
        }
        private void CreateGrid(double[][] X)
        {
            grid = new Grid(X);
            point = new IvyCore.Parametric.Point(grid);

            hasValidGrid = true;
            hasValidControls = false;

            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Grid Has Changed. Reset Controls via Menu.");
            this.Message = "Select Controls in Menu";
        }

        
        #region DYNAMIC GENERATION OF CONTROLS
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Set Continuous controls", ResetContinuousControls);
            Menu_AppendItem(menu, "Set Discret controls", ResetDiscretControls);
            Menu_AppendItem(menu, "Set Tuple controls", ResetTupleControls);

        }
        private void ResetControls(Func<int, IGH_Param> createControl)
        {
            // destroy existing sliders and parameters
            DeleteExistingControls();

            var ghdoc = this.OnPingDocument();

            if (ghdoc != null)
            {
                // Create a new slider for each dimension
                for (int d = 0; d < grid.Dim; d++)
                {
                    IGH_Param control = createControl(d);

                    // description
                    control.NickName = "D" + d;
                   
                    // position the new control on the caneva, relative to this component
                    var x = (float)this.Attributes.DocObject.Attributes.Bounds.Left - control.Attributes.Bounds.Width - 30;
                    var y = (float)this.Attributes.DocObject.Attributes.Bounds.Top + (d + 1) * 30;
                    control.Attributes.Pivot = new PointF(x, y);

                    // add the slider to the canevas
                    ghdoc.AddObject(control, false);

                    // add a new parameter
                    var param = CreateParameter(GH_ParameterSide.Input, d + 1);

                    // wire the newly created input and slider
                    param.AddSource(control);
                }

                hasValidControls = true;

                // call expire to redraw the solution;
                this.ExpireSolution(true);
            }
        }
        private void DeleteExistingControls()
        {
            // destroy existing sliders and parameters (from 1 to N) at index = 1
            // this leaves only the first input at index = 0
            int n = Params.Input.Count;
            for (int i = 1; i < n; i++)
            {
                this.DestroyParameter(GH_ParameterSide.Input, 1);
            }
        }

        private void ResetContinuousControls(object sender, EventArgs e)
        {
            ResetControls(CreateNumberSlider);
            this.Message = "Continuous";
        }
        private IGH_Param CreateNumberSlider(int d)
        {
            var slider = new GH_NumberSlider();
            slider.CreateAttributes();
            
            // setup the slider according to the grid
            // each slider account for one grid dimension
            var t0 = grid.Intervals[d].T0;
            var t1 = grid.Intervals[d].T1;
            slider.Slider.Type = Grasshopper.GUI.Base.GH_SliderAccuracy.Float;
            slider.Slider.DecimalPlaces = 2;
            slider.Slider.Minimum = (decimal)t0;
            slider.Slider.Maximum = (decimal)t1;
            slider.SetSliderValue((decimal)(0.5 * (t0 + t1)));

            return slider;
        }

        private void ResetDiscretControls(object sender, EventArgs e)
        {
            ResetControls(CreateNumberValueList);
            this.Message = "Discret";
        }
        private IGH_Param CreateNumberValueList(int d)
        {
            //instantiate  new value list
            var valueList = new GH_ValueList();
            valueList.CreateAttributes();
            valueList.ListMode = GH_ValueListMode.Cycle;

            //populate value list with our own data
            valueList.ListItems.Clear();
            for (int i = 0; i < grid.NodeDimCount[d]; i++)
            {
                var val = String.Format("{0:F3}", grid.Data[d][i]);
                var item = new GH_ValueListItem(val, val);
                valueList.ListItems.Add(item);
            }

            return valueList;
        }

        private void ResetTupleControls(object sender, EventArgs e)
        {
            ResetControls(CreateTupleValueList);
            this.Message = "Tuple";
        }
        private IGH_Param CreateTupleValueList(int d)
        {
            //instantiate  new value list
            var valueList = new GH_ValueList();
            valueList.CreateAttributes();
            valueList.ListMode = GH_ValueListMode.Sequence;

            //populate value list with our own data
            valueList.ListItems.Clear();
            for (int i = 0; i < grid.NodeDimCount[d]; i++)
            {
                var val = "" + i;
                var item = new GH_ValueListItem(val, val);
                valueList.ListItems.Add(item);
            }

            return valueList;
        }
        private IGH_Param CreateIntegerSlider(int d)
        {
            var slider = new GH_NumberSlider();
            slider.CreateAttributes();

            // setup the slider according to the grid
            // each slider account for one grid dimension
            var t0 = grid.Intervals[d].T0;
            var t1 = grid.Intervals[d].T1;
            slider.Slider.Type = Grasshopper.GUI.Base.GH_SliderAccuracy.Integer;
            slider.Slider.Minimum = (decimal)t0;
            slider.Slider.Maximum = (decimal)t1 - 1;
            slider.SetSliderValue((decimal)(0.5 * (t0 + t1)));

            return slider;
        }
        #endregion

        #region IGH_VariableParameterComponent
        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            // add a number parameter to the component
            var param = new Param_Number();
            param.NickName = "D" + (index - 1);
            this.Params.RegisterInputParam(param);
            return param;
        }
        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            var ghdoc = this.OnPingDocument();

            var param = this.Params.Input[index];
            var sources = param.Sources;

            // stack all components linked to this parameter
            var components = new List<IGH_DocumentObject>();
            for (int i = 0; i < param.Sources.Count; i++)
            {
                var source = param.Sources[i];
                var component = source.Attributes.GetTopLevel.DocObject;
                components.Add(component);
            }

            // disconnect all components linked to this input
            param.RemoveAllSources();

            // delete all the previously connected components
            for (int i = 0; i < components.Count; i++)
            {
                ghdoc.RemoveObject(components[i].Attributes, false);
            }
          
            // unregister the input parameter
            var b = this.Params.UnregisterInputParameter(param);
           
            return true;
        }
        public void VariableParameterMaintenance()
        {
            return;
        }
        #endregion

    }
}