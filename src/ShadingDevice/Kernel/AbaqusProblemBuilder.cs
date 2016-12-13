using Grasshopper;
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
            var shellProp = new AbaqusShellElementProperty(0.01, 210e9, 8, 1e-4);
            for (int i = 0; i < ShellFaces.Length; i++)
            {
                ShellProperties[i] = shellProp;
            }

            int nodeNum = shellVertices.Length;
            var trussProp = new AbaqusTrussElementProperty(1e-2, 210e9, 8, 1e-4);

            A1 = new Actuator(a1[0], a1[1], nodeNum, nodeNum + 1, trussProp);
            A2 = new Actuator(a2[0], a2[1], nodeNum+2, nodeNum + 3, trussProp);

            BC = new BoundaryCondition[bc.Count];
            for (int i = 0; i < bc.Count; i++)
            {
                BC[i] = BoundaryCondition.CreateEncastreBC(bc[i]);
            }
        }

        #region PY BUILDER
        public List<string> GetPy() { return new List<string>(); }
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

            s.AddRange(header);

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
            
            s.Add("*Node, nset=\"A1\"");
            foreach (int shellIndex in new int[2] { A1.ShellNode_0, A1.ShellNode_1 })
            {
                var pt = ShellVertices[shellIndex];
                s.Add(
                    String.Format("{0,6},{1,12},{2,12},{3,12}",
                    shellIndex + 1,
                    pt[0].ToString(format),
                    pt[1].ToString(format),
                    pt[2].ToString(format))
                );
            }

            s.Add("*Node, nset=\"A2\"");
            foreach (int shellIndex in new int[2] { A2.ShellNode_0, A2.ShellNode_1 })
            {
                var pt = ShellVertices[shellIndex];
                s.Add(
                    String.Format("{0,6},{1,12},{2,12},{3,12}",
                    shellIndex + 1,
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

            s.Add("*Element, type=T3D2H, elset=\"A1\"");
            s.Add(String.Format("{0,6},{1,6},{2,6}",
              elNum + 1,
              A1.TrussNode_0,
              A1.TrussNode_1)
              );

            s.Add("*Element, type=T3D2H, elset=\"A2\"");
            s.Add(String.Format("{0,6},{1,6},{2,6}",
              elNum + 1,
              A2.TrussNode_0,
              A2.TrussNode_1)
              );

            return s;
        }
        private List<string> GetSections()
        {
            List<string> s = new List<string>();

            // SHELL
            //for (int i = 0; i < ShellFaces.Length; i++)
            //{
                s.Add("*Shell Section, elset=\"shell\", material=\"shell\"");
                s.Add("0.01, 5"); // thickness, thickness integration points
                s.Add("");
            //}
            

            // ACTUATOR 1
            s.Add("*Solid Section, elset=\"A1\", material=\"actuator\"");
            s.Add(String.Format("{0:e2}", A1.Property.AbaqusTrussSectionArea)); // section

            // ACTUATOR 2
            s.Add("*Solid Section, elset=\"A2\", material=\"actuator\"");
            s.Add(String.Format("{0:e2}", A2.Property.AbaqusTrussSectionArea)); // section

            return s;
        }
        private List<string> GetSets()
        {
            List<string> s = new List<string>();

            // ACTUATOR 1
            s.Add("*Nset, nset=\"A1-1-shell\"");
            s.Add(String.Format("{0}", A1.ShellNode_0 + 1));
            s.Add("*Nset, nset=\"A1-1-beam\"");
            s.Add(String.Format("{0}", A1.TrussNode_0 + 1));

            s.Add("*Nset, nset=\"A1-2-shell\"");
            s.Add(String.Format("{0}", A1.ShellNode_1 + 1));
            s.Add("*Nset, nset=\"A1-2-beam\"");
            s.Add(String.Format("{0}", A1.TrussNode_1 + 1));

            // ACTUATOR 2
            s.Add("*Nset, nset=\"A2-1-shell\"");
            s.Add(String.Format("{0}", A2.ShellNode_0 + 1));
            s.Add("*Nset, nset=\"A2-1-beam\"");
            s.Add(String.Format("{0}", A2.TrussNode_0 + 1));

            s.Add("*Nset, nset=\"A2-2-shell\"");
            s.Add(String.Format("{0}", A2.ShellNode_1 + 1));
            s.Add("*Nset, nset=\"A2-2-beam\"");
            s.Add(String.Format("{0}", A2.TrussNode_1 + 1));

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
            s.Add("*Material, name=\"shell\"");
            s.Add("*Density");
            s.Add(String.Format("{0:F2},", 1));
            s.Add("*Elastic");
            s.Add(String.Format("{0:e3}, {1:F2}", 5e+09, 0.3));

            // ACTUATORS
            s.Add("*Material, name=\"actuator\"");
            s.Add("*Density");
            s.Add(String.Format("{0:F2},", 8));
            s.Add("*Elastic");
            s.Add(String.Format("{0:e3}, {1:F2}", 210e+09, 0.3));
            s.Add("*Expansion");
            s.Add(String.Format("{0:e3},", aT1));

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
