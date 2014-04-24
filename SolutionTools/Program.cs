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

            var projectReader = CreateProjectReader(args);
            var filter = CreateFilter(args);
            var writer = CreateWriter(args);

            var projects = projectReader.GetProjects();
            projects = filter.ApplyFilters(projects);
            writer.Write(projects, Console.Out);
        }

        private static IProjectReader CreateProjectReader(string[] args)
        {
            var subOption = args[0];
            if (ProjectListBuilder.HasProjectExtension(subOption))
            {
                return new ProjectDependenciesReader(subOption);
            }
            if (Path.GetExtension(subOption) == ".sln")
            {
                return new SolutionProjectsReader(subOption);
            }
            if (PathHelper.IsDirectory(subOption))
            {
                return new DirectoryReader(subOption);
            }
            if (args.Any(a => a.ToLowerInvariant().Contains("--stdin")))
            {
                return new ProjectStreamReader(Console.In);
            }
            throw new NotSupportedException();
        }

        private static IProjectListWriter CreateWriter(string[] args)
        {
            if (args[0] == "graph")
                return new GraphWriter(args.Any(a => a.ToLowerInvariant() == "--assemblyreferences"));
            if (args[0] == "list")
                return new BasicWriter();
            if (args[0] == "auto")
            {
                var folderSelector = new FolderSelector(args[2]);
                return new SolutionWriter(args[2], folderSelector.GetFolder, IsTestProject);
            }
            throw new NotSupportedException();
        }

        private static ProjectFilter CreateFilter(string[] args)
        {
            var exclude = GetOption(args, "--exclude");
            var include = GetOption(args, "--include");
            return new ProjectFilter(include, exclude);
        }

        private static string GetOption(IEnumerable<string> args, string option)
        {
            return args.SkipWhile(a => a.ToLowerInvariant() != option).Skip(1).FirstOrDefault();
        }

        private static bool IsTestProject(string fn)
        {
            return TestFolderRegex.IsMatch(fn);
        }
    }
}
