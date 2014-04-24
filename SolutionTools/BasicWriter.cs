using System.Collections.Generic;
using System.IO;

namespace SolutionTools
{
    internal class BasicWriter : IProjectListWriter
    {
        private readonly TextWriter _textWriter;

        public BasicWriter(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        public void Write(IEnumerable<string> projects)
        {
            foreach (var project in projects)
            {
                _textWriter.WriteLine("{0}", project);
            }
        }
    }
}