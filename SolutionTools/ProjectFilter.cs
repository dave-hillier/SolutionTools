using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SolutionTools
{
    public class ProjectFilter
    {
        private readonly Regex _includeRegex;
        private readonly Regex _excludeRegex;

        public ProjectFilter(string include, string exclude)
        {
            _includeRegex = include != null ? new Regex(include) : null;
            _excludeRegex = exclude != null ? new Regex(exclude) : null;
        }

        public IEnumerable<string> ApplyFilters(IEnumerable<string> projects)
        {
            if (_includeRegex != null)
            {
                projects = projects.Where(p => _includeRegex.IsMatch(p));
            }
            if (_excludeRegex != null)
            {
                projects = projects.Where(p => !_excludeRegex.IsMatch(p));
            }
            return projects;
        }
    }
}