using System.Collections.Generic;
using System.Linq;

namespace SolutionTools
{
    class ProjectDependenciesReader : IProjectReader
    {
        private readonly string _projectPath;

        public ProjectDependenciesReader(string projectPath)
        {
            _projectPath = projectPath;
        }

        public IEnumerable<string> GetProjects()
        {
            return ProjectListBuilder.GetAllDependenciesRecursively(_projectPath).Concat(new[] {_projectPath});

        }
    }
}