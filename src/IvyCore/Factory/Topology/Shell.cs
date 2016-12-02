using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Factory
{
    public class Shell
    {
        private List<double[]> vertices;
        private List<int[]> faces;
        private List<AbaqusShellElementProperty> properties;

        public int NodeCount
        {
            get
            {
                return vertices.Count;
            }
        }
        public int ElementCount
        {
            get
            {
                return faces.Count;
            }
        }

        public List<double[]> Vertices
        {
            get
            {
                return vertices;
            }
        }
        public List<int[]> Faces
        {
            get
            {
                return faces;
            }
        }
        public List<AbaqusShellElementProperty> Properties { get; protected set; }

        public Shell(List<double[]> vertices, List<int[]> faces, List<AbaqusShellElementProperty> properties)
        {
            this.vertices = vertices;
            this.faces = faces;
            this.properties = properties;
        }

        public override string ToString()
        {
            var s = new List<string>();

            s.Add("NODES (" + NodeCount + ")");
            s.Add("==========================");
            for (int i = 0; i < vertices.Count; i++)
            {
                var node = vertices[i];
                s.Add(String.Format("NODE[{0}] : ({1:F3},{2:F3},{3:F3})", i + 1, node[0], node[1], node[2]));
            }

            s.Add("");
            s.Add("ELEMENTS (" + ElementCount + ")");
            s.Add("==========================");
            for (int i = 0; i < faces.Count; i++)
            {
                var face = faces[i];
                s.Add(String.Format("FACE[{0}] : ({1},{2},{3},{4}) | thikness = {5:F3}", i + 1, face[0], face[1], face[2], face[3], properties[i].AbaqusShellThickness));
            }

            return String.Join(Environment.NewLine, s);
        }
    }
}
