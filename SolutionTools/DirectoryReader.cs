using System.Collections.Generic;
using System.Linq;

namespace SolutionTools
{
    class DirectoryReader : IProjectReader
    {
        private readonly string _path;

        public DirectoryReader(string path)
        {
            _path = path;
        }

        public IEnumerable<string> GetProjects()
        {
            var projects = ProjectListBuilder.FindProjects(_path).ToArray();
            var deps = from f in projects from dep in ProjectListBuilder.GetAllDependenciesRecursively(f) select dep;
            return deps.Concat(projects).OrderByDescending(a => a).Distinct();
        }
    }
}