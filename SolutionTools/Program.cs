using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SolutionTools
{
    public class Program
    {
        private static readonly Regex TestFolderRegex = new Regex("[Tt]est");
        private static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            var subOption = args[1];
            var projects = GetProjectsAndDependencies(subOption);

            var filter = CreateFilter(args);
            projects = filter.ApplyFilters(projects);

            var writer = CreateWriter(args);
            writer.Write(projects, Console.Out);
        }

        private static IProjectListWriter CreateWriter(string[] args)
        {
            if (args[0] == "graph")
                return new GraphWriter(args.Any(a => a.ToLowerInvariant() == "--assemblyreferences"));
            if (args[0] == "list")
                return new BasicListWriter();
            if (args[0] == "auto")
                return new SlnWriter(args[2], path => FolderSelector.GetSlnFolder(args[2], path), IsTestProject);
            throw new NotSupportedException();
        }

        private static ProjectFilter CreateFilter(string[] args)
        {
            var exclude = args.SkipWhile(a => a != "--exclude").Skip(1).FirstOrDefault();
            var include = args.SkipWhile(a => a != "--include").Skip(1).FirstOrDefault();
            return new ProjectFilter(include, exclude);
        }

        private static IEnumerable<string> GetProjectsAndDependencies(string subOption)
        {
            if (ProjectListBuilder.HasProjectExtension(subOption))
            {
                return ProjectListBuilder.FindAllDependencies(subOption).Concat(new[] { subOption });
            }
            if (Path.GetExtension(subOption) == ".sln")
            {
                return ReadAllProjectsFromSolution(subOption);
            }
            if (PathHelper.IsDirectory(subOption))
            {
                return GetProjectsAndDependenciesInDirectory(subOption);
            }
            throw new NotSupportedException();
        }

        private static IEnumerable<string> ReadAllProjectsFromSolution(string subOption)
        {
            var directory = Path.GetDirectoryName(subOption);
            return SolutionReader.ReadAllProjects(subOption).
                Select(fn => directory != null ? Path.Combine(directory, fn) : null).Where(fn => fn != null);
        }

        private static IEnumerable<string> GetProjectsAndDependenciesInDirectory(string directory)
        {
            var projects = ProjectListBuilder.FindProjects(directory).ToArray();
            var deps = from f in projects
                       from dep in ProjectListBuilder.FindAllDependencies(f)
                       select dep;
            return deps.Concat(projects).OrderByDescending(a => a).Distinct();
        }

        private static bool IsTestProject(string fn)
        {
            return TestFolderRegex.IsMatch(fn);
        }
    }
}
