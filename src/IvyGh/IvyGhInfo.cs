using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace IvyGh
{
    public class IvyGhInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "IvyGh";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("bd7d9427-3369-45a8-b8c6-fbd626d519f7");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
