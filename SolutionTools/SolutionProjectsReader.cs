using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionTools
{
    class SolutionProjectsReader : IProjectReader
    {
        private readonly string _slnPath;

        public SolutionProjectsReader(string slnPath)
        {
            _slnPath = slnPath;
        }

        public IEnumerable<string> GetProjects()
        {
            var directory = Path.GetDirectoryName(_slnPath);
            return SolutionReader.ReadAllProjects(_slnPath).
                                  Select(fn => directory != null ? Path.Combine(directory, fn) : null).Where(fn => fn != null);
        }
    }
}