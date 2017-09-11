using System.Collections.Generic;

namespace SolutionTools
{
    interface IProjectReader
    {
        IEnumerable<string> GetProjects();
    }
}