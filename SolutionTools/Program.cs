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

            var textWriter = GetOutputTextWriter(args);
            var writer = CreateProjectWriter(args, textWriter);

            var projects = projectReader.GetProjects();
            projects = filter.ApplyFilters(projects);
            writer.Write(projects);


        }

        private static TextWriter GetOutputTextWriter(string[] args)
        {
            return IsOptionSet(args, "--out") ? new StreamWriter(File.Open(GetOption(args, "--out"), FileMode.OpenOrCreate)) : Console.Out;
        }

        private static IProjectReader CreateProjectReader(string[] args)
        {
            if (IsOptionSet(args, "--consolein"))
            {
                return new ProjectStreamReader(Console.In);
            }
            var subOption = args[1];
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
            throw new NotSupportedException();
        }

        private static bool IsOptionSet(IEnumerable<string> args, string option)
        {
            return args.Any(a => a.ToLowerInvariant().Contains(option));
        }

        private static IProjectListWriter CreateProjectWriter(string[] args, TextWriter writer)
        {
            var verb = args[0];
            if (verb == "graph")
                return new GraphWriter(writer, IsOptionSet(args, "--assemblyreferences"));
            if (verb == "list")
                return new BasicWriter(writer);
            if (verb == "auto")
            {
                var folderSelector = new FolderSelector(args[2]);
                return new SolutionWriter(writer, args[2], folderSelector.GetFolder, IsTestProject);
            }
            throw new NotSupportedException(string.Format("Unknown option: {0}", verb));
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
