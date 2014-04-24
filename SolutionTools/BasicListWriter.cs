using System.Collections.Generic;
using System.IO;

namespace SolutionTools
{
    internal class BasicListWriter : IProjectListWriter
    {
        public void Write(IEnumerable<string> projects, TextWriter textWriter)
        {
            foreach (var project in projects)
            {
                textWriter.WriteLine("{0}", project);
            }
        }
    }
}