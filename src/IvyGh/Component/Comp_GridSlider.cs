using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using IvyCore.MultiDimGrid;
using Grasshopper.Kernel.Special;
using System.Drawing;
using Grasshopper.Kernel.Parameters;
using System.Windows.Forms;
using IvyGh.Type;
using IvyGh.Properties;

namespace IvyGh
{
    public class Comp_GridSlider : GH_Component, IGH_VariableParameterComponent
    {
        private GH_Grid ghGrid, ghGridCache;
        private IvyCore.MultiDimGrid.Point point;
        private bool hasValidControls = false;


        public Comp_GridSlider()
          : base("MultiDimensional Slider", "Slider",
              "Explore a MuliDimGrid seamlessly. Select sliding options from the rigth-clic menu.",
              "Ivy", "Grid")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{479434ce-3e14-4f0b-abec-2bc675c40eb4}"); }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.tertiary;
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.grid_slider;
            }
        }
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "grid", "The grid to explore.", GH_ParamAccess.item);
            pManager[0].Optional = false;

            //this.Params.ParameterSourcesChanged += OnParameterSourcesChanged;
        }

        private void OnParameterSourcesChanged(object sender, GH_ParamServerEventArgs e)
        {
            //// It's happening on Params.Input
            //if (e.ParameterSide == GH_ParameterSide.Input)
            //{
            //    // it's happening on Params.Input[0]
            //    if (e.ParameterIndex == 0)
            //    {
            //        int n = Params.Input[0].SourceCount;

            //        if (n == 0)
            //        {
            //            // no more source is connected to Inpur[0]
            //            hasValidPoint = false;
            //            return;
            //        }

            //        //  one source is connected
            //        if (n == 1)
            //        {
            //            // thus the grid may have changed ...
            //            // we request refresh from the user
            //            hasValidControls = false;
            //            hasValidPoint = false;

                        
            //            Params.Input[0]..SolutionExpired += OnGridExpired;

            //            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Grid Has Changed. Reset Controls via Menu.");
            //            this.Message = "Select Controls in Menu";
            //        }

            //        //  more than one source is not allowed : disconnect them
            //        for (int i = 1; i < n; i++)
            //        {
            //            Params.Input[0].RemoveSource(Params.Input[0].Sources[i]);
            //        }
            //    }
            //}
        }
        private void OnGridExpired(IGH_DocumentObject sender, GH_SolutionExpiredEventArgs e)
        {
           // throw new NotImplementedException();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "info", "Grid info.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Active Point Coordinates", "ptCoord", "Selected Point coordinates in the Grid.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Active Cell Index", "cellIndex", "Active Cell index.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ghGrid = new GH_Grid();

            if (!DA.GetData(0,ref ghGrid)) return;

            if(GridHasChanged())
            {
                hasValidControls = false;
                point = new IvyCore.MultiDimGrid.Point(ghGrid.Value);
                // cache a DeepCopy
                ghGridCache = ghGrid.DeepCopy();
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Grid Has Changed. Reset Controls via Menu.");
                this.Message = "Select Controls in Menu";
            }

            // If controls are valid, then compute the point
            if (hasValidControls)
            {
                UpdatePoint(DA);
                DA.SetDataList(1, point.Coord);
                DA.SetData(2, point.GetCellIndex());
            }
            else
            {          
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Grid Has Changed. Reset Controls via Menu.");
                this.Message = "Select Controls in Menu";
            }

            DA.SetData(0, ghGrid.Value.Info());
        }

        private void UpdatePoint(IGH_DataAccess DA)
        {
            for (int d = 0; d < ghGrid.Value.Dim; d++)
            {
                DA.GetData<double>(1 + d, ref point.Coord[d]);
            }
        }
        private bool GridHasChanged()
        {
            if (ghGridCache == null)
            {
                // cache a DeepCopy
                ghGridCache = ghGrid.DeepCopy();
                return true;
            }

            var b1 = Grid.IsEqualData(ghGrid.Value, ghGridCache.Value);
            var b2 = Grid.IsEqualLabels(ghGrid.Value, ghGridCache.Value);

            return !(b1 && b2);
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
                for (int d = 0; d < ghGrid.Value.Dim; d++)
                {
                    IGH_Param control = createControl(d);

                    // description
                    control.NickName = "D" + d + " | " + ghGrid.Value.Labels[d];
                   
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
            var t0 = ghGrid.Value.Intervals[d].T0;
            var t1 = ghGrid.Value.Intervals[d].T1;
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
            for (int i = 0; i < ghGrid.Value.NodeDimCount[d]; i++)
            {
                var val = String.Format("{0:F3}", ghGrid.Value.Data[d][i]);
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
            for (int i = 0; i < ghGrid.Value.NodeDimCount[d]; i++)
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
            var t0 = ghGrid.Value.Intervals[d].T0;
            var t1 = ghGrid.Value.Intervals[d].T1;
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
            param.Access = GH_ParamAccess.item;
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