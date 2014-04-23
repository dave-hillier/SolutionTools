using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionTools
{
    internal class ProjectListBuilder
    {
        private static readonly string[] Ext = new[] { ".csproj", ".fsproj", ".vbproj" };

        public static bool HasProjectExtension(string fileName)
        {
            return (from ext in Ext select Path.GetExtension(fileName) == ext).Any(b => b);
        }

        public static IEnumerable<string> FindProjects(string root)
        {
            return from ext in Ext
                   from projPath in Directory.GetFiles(root, "*" + ext, SearchOption.AllDirectories)
                   select projPath;
        }

        public static IEnumerable<string> FindAllDependencies(string proj)
        {
            Func<string, string[]> tmp = a => ProjectReader.GetProjectReferences(a).ToArray();
            var getReferencesMemoized = tmp.Memoize();
            return GetAllDependenciesRecursively(proj, getReferencesMemoized);
        }

        private static IEnumerable<string> GetAllDependenciesRecursively(string proj, Func<string, string[]> getReferencesMemoized)
        {
            var projectReferences = getReferencesMemoized(proj);

            var more = from reference in projectReferences
                       from another in GetAllDependenciesRecursively(reference, getReferencesMemoized)
                       select another;

            return projectReferences.Concat(more).OrderByDescending(a => a).Distinct();
        }
    }
}