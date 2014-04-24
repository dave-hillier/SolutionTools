using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SolutionTools
{
    public class ProjectFilter
    {
        private readonly string _include;
        private readonly string _exclude;

        public ProjectFilter(string include, string exclude)
        {
            _include = include;
            _exclude = exclude;
        }

        public IEnumerable<string> ApplyFilters(IEnumerable<string> projects)
        {
            if (_include != null)
            {
                var re = new Regex(_include);
                projects = projects.Where(p => re.IsMatch(p));
            }
            if (_exclude != null)
            {
                var re1 = new Regex(_exclude);
                projects = projects.Where(p => !re1.IsMatch(p));
            }
            return projects;
        }
    }
}