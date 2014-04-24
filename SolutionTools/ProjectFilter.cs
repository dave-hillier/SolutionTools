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
            projects = InclusiveFilter(_include, projects);
            projects = ExclusiveFilter(_exclude, projects);
            return projects;
        }

        private static IEnumerable<string> ExclusiveFilter(string excludeRegex, IEnumerable<string> projects)
        {
            if (excludeRegex != null)
            {
                var re = new Regex(excludeRegex);
                projects = projects.Where(p => !re.IsMatch(p));
            }
            return projects;
        }

        private static IEnumerable<string> InclusiveFilter(string includeRegex, IEnumerable<string> projects)
        {
            if (includeRegex != null)
            {
                var re = new Regex(includeRegex);
                projects = projects.Where(p => re.IsMatch(p));
            }
            return projects;
        }
    }
}