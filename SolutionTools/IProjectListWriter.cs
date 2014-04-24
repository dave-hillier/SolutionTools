using System.Collections.Generic;
using System.IO;

namespace SolutionTools
{
    public interface IProjectListWriter
    {
        void Write(IEnumerable<string> projects, TextWriter textWriter);
    }
}