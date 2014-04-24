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
            var projects = filter.ApplyFilters(projectReader.GetProjects());

            var writer = CreateWriter(args);
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
            // TODO: command line reader
            throw new NotSupportedException();
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
            var exclude = args.SkipWhile(a => a.ToLowerInvariant() != "--exclude").Skip(1).FirstOrDefault();
            var include = args.SkipWhile(a => a.ToLowerInvariant() != "--include").Skip(1).FirstOrDefault();
            return new ProjectFilter(include, exclude);
        }

        private static bool IsTestProject(string fn)
        {
            return TestFolderRegex.IsMatch(fn);
        }
    }
}
