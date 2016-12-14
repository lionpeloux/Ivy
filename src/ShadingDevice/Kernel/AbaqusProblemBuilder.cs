using Grasshopper;
using IvyCore.Parametric;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShadingDevice.Kernel
{
    public class AbaqusProblemBuilder
    {
        public double[][] ShellVertices { get; private set; }
        public int[][] ShellFaces { get; private set; }
        public AbaqusShellElementProperty[] ShellProperties { get; private set; }

        public Actuator A1 { get; private set; }
        public Actuator A2 { get; private set; }
        public BoundaryCondition[] BC { get; private set; }

        public AbaqusProblemBuilder(double[][] shellVertices, int[][] shellFaces, IList<int> a1, IList<int> a2, IList<int> bc)
        {
            ShellVertices = shellVertices;
            ShellFaces = shellFaces;

            ShellProperties = new AbaqusShellElementProperty[ShellFaces.Length];
            var shellProp = new AbaqusShellElementProperty(0.01, 5e9, 1, 0);
            for (int i = 0; i < ShellFaces.Length; i++)
            {
                ShellProperties[i] = shellProp;
            }

            int nodeNum = shellVertices.Length;


            // actuator length (in m)
            double[] pta_0, pta_1;
            double sec, l, alpha, E, density;
            AbaqusTrussElementProperty trussProp;

            // A1
            pta_0 = shellVertices[a1[0]];
            pta_1 = shellVertices[a1[1]];
            l = Math.Sqrt(
                (pta_1[0] - pta_0[0]) * (pta_1[0] - pta_0[0]) +
                (pta_1[1] - pta_0[1]) * (pta_1[1] - pta_0[1]) +
                (pta_1[2] - pta_0[2]) * (pta_1[2] - pta_0[2])
                );

            E = 210e9;
            density = 8;
            sec = Math.PI * Math.Pow(0.1, 2);

            alpha = 1e-3 / l;
            trussProp = new AbaqusTrussElementProperty(sec, E, density, alpha);
            A1 = new Actuator(a1[0], a1[1], nodeNum, nodeNum + 1, trussProp);

            // A1
            pta_0 = shellVertices[a2[0]];
            pta_1 = shellVertices[a2[1]];
            l = Math.Sqrt(
                (pta_1[0] - pta_0[0]) * (pta_1[0] - pta_0[0]) +
                (pta_1[1] - pta_0[1]) * (pta_1[1] - pta_0[1]) +
                (pta_1[2] - pta_0[2]) * (pta_1[2] - pta_0[2])
                );


            alpha = 1e-3 / l;
            trussProp = new AbaqusTrussElementProperty(sec, E, density, alpha);
            A2 = new Actuator(a2[0], a2[1], nodeNum+2, nodeNum + 3, trussProp);

            // BC
            BC = new BoundaryCondition[bc.Count];
            for (int i = 0; i < bc.Count; i++)
            {
                BC[i] = BoundaryCondition.CreateEncastreBC(bc[i]);
            }
        }

        #region PY BUILDER

        public static List<string> GetPy(string dir, Grid actuationGrid)
        {
            List<string> s = new List<String>();
            var heading = GetHeading();
            var functions = GetFunctions();
            var stepProp = GetStepProperties(true, 200, 0.01, 1e-6, 0.05);
            var actuNodes = GetActuation(actuationGrid);

            s.AddRange(heading);
            s.Add("  ");

            s.AddRange(functions);
            s.Add("  ");

            s.Add("# ACTUATION NODES");
            s.AddRange(actuNodes);
            s.Add("  ");

            s.Add("# STEP PROPERTIES");
            s.AddRange(stepProp);
            s.Add("  ");

            s.Add("# WORKING DIRECTORY");
            s.Add("wkdir = '" + dir + "'");
            s.Add("  ");

            s.Add("# GENERATE ABAQUS MODELS FROM INP FILES");
            s.Add("for subdir in GetSubdirectories(wkdir):");
            s.Add("  ");

            s.Add("  " + "# LOAD INP FILE");
            s.Add("  " + "path = wkdir + '\\\\' + subdir");
            s.Add("  " + "mdb.ModelFromInputFile(inputFileName = path + '\\\\' + 'model.inp', name = 'model')");
            s.Add("  ");

            s.Add("  " + "# DELETE ALL OTHER EXISTING MODELS");
            s.Add("  " + "for k in mdb.models.keys():");
            s.Add("    " + "if k <> 'model': del mdb.models[k]");
            s.Add("  ");

            s.Add("  " + "# CREATE FIELDS");
            s.Add("  " + "# T1 : actuation field 1 | +1K => 1mm expansion | -1K => 1mm skrinkage");
            s.Add("  " + "# T2 : actuation field 2 | +1K => 1mm expansion | -1K => 1mm skrinkage");
            s.Add("  " + "mdb.models['model'].Temperature(createStepName = 'Initial', crossSectionDistribution = CONSTANT_THROUGH_THICKNESS, distributionType = UNIFORM, magnitudes = (0.0, ), name = 'T1', region = mdb.models['model'].rootAssembly.instances['Instance A'].sets['A1'])");
            s.Add("  " + "mdb.models['model'].Temperature(createStepName = 'Initial', crossSectionDistribution = CONSTANT_THROUGH_THICKNESS, distributionType = UNIFORM, magnitudes = (0.0, ), name = 'T2', region = mdb.models['model'].rootAssembly.instances['Instance A'].sets['A2'])");
            s.Add("  ");

            s.Add("  " + "# CREATE STEPS");
            s.Add("  " + "stepName_prev = 'Initial'");
            s.Add("  " + "stepName = ''");
            s.Add("  " + "for i in range(len(actu)):");
            s.Add("    " + "a = actu[i]");
            s.Add("    " + "if i > 0: stepName_prev = stepName");
            s.Add("    " + "stepName = '(' + str(int(a[0])) + ',' + str(int(a[1])) + ')'");
            s.Add("    " + "mdb.models['model'].StaticStep(nlgeom = nlgeom, maxNumInc = maxNumInc, initialInc = initialInc, maxInc = maxInc, minInc = minInc, name = stepName, previous = stepName_prev)");
            s.Add("    " + "mdb.models['model'].predefinedFields['T1'].setValuesInStep(magnitudes = (a[0], ), stepName = stepName)");
            s.Add("    " + "mdb.models['model'].predefinedFields['T2'].setValuesInStep(magnitudes = (a[1], ), stepName = stepName)");
            s.Add("  ");

            s.Add("  " + "# CREATE NEW JOB");
            s.Add("  " +    "mdb.Job(atTime = None, contactPrint = OFF, description = '', echoPrint = OFF, " +
                            "explicitPrecision = SINGLE, getMemoryFromAnalysis = True, historyPrint = OFF, memory = 90, " +
                            "memoryUnits = PERCENTAGE, model = 'model', modelPrint = OFF, multiprocessingMode = DEFAULT, name = 'job', " +
                            "nodalOutputPrecision = SINGLE, numCpus = 1, numGPUs = 0, queue = None, scratch = '', type = ANALYSIS, userSubroutine = '', " +
                            "waitHours = 0, waitMinutes = 0)"
                            );
            s.Add(" ");
            s.Add("  " + "# SAVE CAE MODEL");
            s.Add("  " + "mdb.saveAs(pathName = path + '\\\\' + 'model.cae')");
            return s;
        }
        private static string[] GetHeading()
        {
            List<string> s = new List<string>();

            // imports
            s.Add("from part import *");
            s.Add("from material import *");
            s.Add("from section import *");
            s.Add("from assembly import *");
            s.Add("from step import *");
            s.Add("from interaction import *");
            s.Add("from load import *");
            s.Add("from mesh import *");
            s.Add("from optimization import *");
            s.Add("from job import *");
            s.Add("from sketch import *");
            s.Add("from visualization import *");
            s.Add("from connectorBehavior import *");

            return s.ToArray();
        }
        private static List<string> GetFunctions()
        {
            List<string> s = new List<string>();

            // imports
            s.Add("def GetSubdirectories(dir):");
            s.Add("\t return [name for name in os.listdir(dir)");
            s.Add("\t\t if os.path.isdir(os.path.join(dir, name))]");
            s.Add("");


            return s;
        }
        private static List<string> GetStepProperties(
            bool nlgeom, 
            int maxNumInc,
            double initialInc,
            double minInc,
            double maxInc
            )
        {
            List<string> s = new List<string>();

            if (nlgeom)
                s.Add("nlgeom = ON");
            else
                s.Add("nlgeom = OFF");

            s.Add("maxNumInc = " + maxNumInc);
            s.Add("initialInc = " + initialInc);
            s.Add("minInc = " + minInc);
            s.Add("maxInc = " + maxInc);

            return s;
        }
        private static List<string> GetActuation(Grid grid)
        {
            List<string> s = new List<string>();

            // imports
            s.Add("actu = [");
            for (int i = 0; i < grid.NodeCount; i++)
            {
                var node = grid.Nodes[i];
                s.Add("\t" + String.Format("({0:F3}, {1:F3}), ", node.Coord[0], node.Coord[1]));
            }
            s.Add("\t]");


            return s;
        }

        #endregion

        #region INP BUILDER
        public List<string> GetInp(IList<string> header)
        {
            List<string> s = new List<String>();

            var heading = GetHeading(header);
            var nodes_shell = GetShellNodes();
            var nodes_actuator = GetActuatorNodes();
            var elements_shell = GetShellElements();
            var elements_actuator = GetActuatorElements();
            var sections = GetSections();
            var sets = GetSets();
            var ties = GetTies();
            var materials = GetMaterials(2, 2);
            var bc = GetBoudaryConditions();

            s.AddRange(heading);

            s.Add("**");
            s.Add("** =============================================================================");
            s.Add("** PART");
            s.Add("** =============================================================================");
            s.Add("**");
            s.Add("*Part, name=\"Part A\"");
            s.AddRange(nodes_shell);
            s.AddRange(nodes_actuator);
            s.AddRange(elements_shell);
            s.AddRange(elements_actuator);
            s.AddRange(sets);
            s.AddRange(sections);
            s.Add("*End Part");
            s.Add("**");
            s.Add("** =============================================================================");
            s.Add("** ASSEMBLY");
            s.Add("** =============================================================================");
            s.Add("**");
            s.Add("*Assembly, name=Assembly");
            s.Add("**");
            s.Add("*Instance, name=\"Instance A\", part=\"Part A\"");
            s.Add("*End Instance");
            s.Add("**");
            s.AddRange(ties);
            s.Add("*End Assembly");
            s.Add("**");
            s.Add("** =============================================================================");
            s.Add("** MATERIALS");
            s.Add("** =============================================================================");
            s.Add("**");
            s.AddRange(materials);
            s.Add("**");
            s.Add("** =============================================================================");
            s.Add("** BOUDARY CONDITIONS");
            s.Add("** =============================================================================");
            s.Add("**");
            s.AddRange(bc);

            return s;

        }

        private List<string> GetHeading(IList<string> header)
        {
            List<string> s = new List<string>();
            s.Add("* Heading");

            for (int i = 0; i < header.Count; i++)
            {
                s.Add("**" + header[i]);
            }

            s.Add("**");
            s.Add("*Preprint, echo=NO, model=NO, history=NO, contact=NO");

            return s;
        }
        private List<string> GetShellNodes(string format="F6")
        {
            List<string> s = new List<string>();

            s.Add("*Node, nset=\"shell\"");
            for (int i = 0; i < ShellVertices.Length; i++)
            {
                var vertex = ShellVertices[i];
                s.Add(
                  String.Format("{0,4},{1,12},{2,12},{3,12}",
                  i + 1,
                  vertex[0].ToString(format),
                  vertex[1].ToString(format),
                  vertex[2].ToString(format))
                  );
            }

            return s;
        }
        private List<string> GetShellElements()
        {
            List<string> s = new List<string>();

            s.Add("*Element, type=S4R, elset=\"shell\"");

            for (int i = 0; i < ShellFaces.Length; i++)
            {
                var face = ShellFaces[i];
                s.Add(
                    String.Format("{0},{1},{2},{3},{4}",
                        i + 1,
                        face[0] + 1,
                        face[1] + 1,
                        face[2] + 1,
                        face[3] + 1
                    )
                );
            }
            return s;
        }
        private List<string> GetActuatorNodes(string format = "F6")
        {
            List<string> s = new List<string>();
            double[] pt;

            var A = new Actuator[2] { A1, A2 };
            for (int i = 0; i < A.Length; i++)
            {
                s.Add("*Node, nset=\"A" + (i + 1) + "\"");
                pt = ShellVertices[A[i].ShellNode_0];
                s.Add(
                    String.Format("{0,6},{1,12},{2,12},{3,12}",
                    A[i].TrussNode_0 + 1,
                    pt[0].ToString(format),
                    pt[1].ToString(format),
                    pt[2].ToString(format))
                );
                pt = ShellVertices[A[i].ShellNode_1];
                s.Add(
                    String.Format("{0,6},{1,12},{2,12},{3,12}",
                    A[i].TrussNode_1 + 1,
                    pt[0].ToString(format),
                    pt[1].ToString(format),
                    pt[2].ToString(format))
                );
            }
            
            return s;
        }
        private List<string> GetActuatorElements()
        {
            List<string> s = new List<string>();

            int elNum = ShellFaces.Length;

            var A = new Actuator[2] { A1, A2 };
            for (int i = 0; i < A.Length; i++)
            {
                elNum += 1;
                s.Add("*Element, type=T3D2H, elset=\"A" + (i + 1) + "\"");
                s.Add(String.Format("{0,6},{1,6},{2,6}",
                  elNum,
                  A[i].TrussNode_0 + 1,
                  A[i].TrussNode_1 + 1)
                  );
            }

            return s;
        }
        private List<string> GetSections()
        {
            List<string> s = new List<string>();

            // SHELL
            for (int i = 0; i < ShellProperties.Length; i++)
            {
                var prop = ShellProperties[i];
                s.Add("*Shell Section, elset=\"shell-" + (i + 1) + "\", material=\"shell-" + (i + 1) + "\"");
                s.Add(String.Format("{0:e2}, {1}", prop.AbaqusShellThickness, 5)); // thickness, thickness integration points
            }

            // ACTUATORS
            var A = new Actuator[2] { A1, A2 };
            for (int i = 0; i < A.Length; i++)
            {
                s.Add("*Solid Section, elset=\"A" + (i + 1) + "\", material=\"actuator-" + (i + 1) + "\"");
                s.Add(String.Format("{0:e2}", A[i].Property.AbaqusTrussSectionArea)); // section
            }

            return s;
        }
        private List<string> GetSets()
        {
            List<string> s = new List<string>();

            // SHELL
            for (int i = 0; i < ShellFaces.Length; i++)
            {
                s.Add("*Elset, elset=\"shell-" + (i + 1) + "\", internal");
                s.Add(String.Format("{0}", i + 1));
            }

            // ACTUATORS
            var A = new Actuator[2] { A1, A2 };
            for (int i = 0; i < A.Length; i++)
            {
                s.Add("*Nset, nset=\"A" + (i + 1) + "-1-shell\"");
                s.Add(String.Format("{0}", A[i].ShellNode_0 + 1));
                s.Add("*Nset, nset=\"A" + (i + 1) + "-1-beam\"");
                s.Add(String.Format("{0}", A[i].TrussNode_0 + 1));

                s.Add("*Nset, nset=\"A" + (i + 1) + "-2-shell\"");
                s.Add(String.Format("{0}", A[i].ShellNode_1 + 1));
                s.Add("*Nset, nset=\"A" + (i + 1) + "-2-beam\"");
                s.Add(String.Format("{0}", A[i].TrussNode_1 + 1));
            }

            // BOUNDARY CONDITIONS
            var bc = new int[BC.Length];
            for (int i = 0; i < BC.Length; i++)
            {
                bc[i] = BC[i].ShellNode + 1;
            }
            s.Add("*Nset, nset=\"BC\"");
            s.Add(String.Join(", ", bc));

            return s;

        }
        private List<string> GetTies()
        {
            List<string> s = new List<string>();

            // ACTUATOR 1
            ///*
            s.Add("*Surface, type=NODE, name=\"A1-1-shell\", internal");
            s.Add(String.Format("\"Instance A\".\"{0}\", 1.", "A1-1-shell"));
            s.Add("*Surface, type=NODE, name=\"A1-1-beam\", internal");
            s.Add(String.Format("\"Instance A\".\"{0}\", 1.", "A1-1-beam"));
            //*/
            s.Add("*Tie, name=\"A1-1\", adjust=no, no rotation");
            s.Add("\"A1-1-shell\", \"A1-1-beam\"");

            ///*
            s.Add("*Surface, type=NODE, name=\"A1-2-shell\", internal");
            s.Add(String.Format("\"Instance A\".\"{0}\", 1.", "A1-2-shell"));
            s.Add("*Surface, type=NODE, name=\"A1-2-beam\", internal");
            s.Add(String.Format("\"Instance A\".\"{0}\", 1.", "A1-2-beam"));
            //*/
            s.Add("*Tie, name=\"A1-2\", adjust=no, no rotation");
            s.Add("\"A1-2-shell\", \"A1-2-beam\"");

            // ACTUATOR 2
            ///*
            s.Add("*Surface, type=NODE, name=\"A2-1-shell\", internal");
            s.Add(String.Format("\"Instance A\".\"{0}\", 1.", "A2-1-shell"));
            s.Add("*Surface, type=NODE, name=\"A2-1-beam\", internal");
            s.Add(String.Format("\"Instance A\".\"{0}\", 1.", "A2-1-beam"));
            //*/
            s.Add("*Tie, name=\"A2-1\", adjust=no, no rotation");
            s.Add("\"A2-1-shell\", \"A2-1-beam\"");

            ///*
            s.Add("*Surface, type=NODE, name=\"A2-2-shell\", internal");
            s.Add(String.Format("\"Instance A\".\"{0}\", 1.", "A2-2-shell"));
            s.Add("*Surface, type=NODE, name=\"A2-2-beam\", internal");
            s.Add(String.Format("\"Instance A\".\"{0}\", 1.", "A2-2-beam"));
            //*/
            s.Add("*Tie, name=\"A2-2\", adjust=no, no rotation");
            s.Add("\"A2-2-shell\", \"A2-2-beam\"");

            return s;

        }
        private List<string> GetMaterials(double aT1, double aT2)
        {
            List<string> s = new List<string>();

            // SHELL
            for (int i = 0; i < ShellProperties.Length; i++)
            {
                var prop = ShellProperties[i];
                s.Add("*Material, name=\"shell-" + (i + 1) + "\"");
                s.Add("*Density");
                s.Add(String.Format("{0:F2},", prop.Density));
                s.Add("*Elastic");
                s.Add(String.Format("{0:e3}, {1:F2}", prop.E, 0.3));
            }

            // ACTUATORS
            var A = new Actuator[2] { A1, A2 };
            for (int i = 0; i < A.Length; i++)
            {
                s.Add("*Material, name=\"actuator-" + (i + 1) + "\"");
                s.Add("*Density");
                s.Add(String.Format("{0:F2},", A[i].Property.Density));
                s.Add("*Elastic");
                s.Add(String.Format("{0:e3}, {1:F2}", A[i].Property.E, 0.3));
                s.Add("*Expansion");
                s.Add(String.Format("{0:e3},", A[i].Property.Alpha));
            }

            return s;
        }
        private List<string> GetBoudaryConditions()
        {
            List<string> s = new List<string>();

            s.Add("*Boundary");
            s.Add("\"Instance A\".\"BC\", ENCASTRE");

            return s;
        }
        #endregion

        #region STATIC METHODS
        public static double[][] MeshVerticesToArray(Mesh mesh)
        {
            int n = mesh.Vertices.Count;
            var array = new double[n][];

            for (int i = 0; i < n; i++)
            {
                var pt = mesh.Vertices[i];
                array[i] = new double[3] { pt.X, pt.Y, pt.Z };
            }
            return array;
        }
        public static int[][] MeshFacesToArray(Mesh mesh)
        {
            int n = mesh.Faces.Count;
            var array = new int[n][];

            for (int i = 0; i < n; i++)
            {
                var face = mesh.Faces[i];

                if (face.IsQuad)
                {
                    array[i] = new int[4] { face[0], face[1], face[2], face[3] };
                }
                else
                {
                    array[i] = new int[3] { face[0], face[1], face[2] };
                }

            }
            return array;
        }
        #endregion

    }
}
