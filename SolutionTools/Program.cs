using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SolutionTools
{
    public class Program
    {
        private static readonly Regex TestFolderRegex = new Regex("[Tt]est");

        private static void Main(string[] args)
        {
            if (args.Length < 1 || args[0].ToLowerInvariant() == "help")
            {
                DisplayHelp();
                return;
            }

            if (args.Length == 2 && args[0].ToLowerInvariant() == "help")
            {
                DisplayHelp(args[1]);                
                return;
            }

            try
            {
                var projectReader = CreateProjectReader(args);
                var filter = CreateFilter(args);

                var textWriter = GetOutputTextWriter(args);
                var writer = CreateProjectWriter(args, textWriter);

                var projects = projectReader.GetProjects();
                projects = filter.ApplyFilters(projects);
                writer.Write(projects);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: {0}", ex.Message);
                DisplayHelp();
                throw;
            }
        }

        private static void DisplayHelp(string command)
        {
            throw new NotImplementedException();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine(@"{0} - {1} - {2}\n",
                              Assembly.GetExecutingAssembly().GetName().Name,
                              Assembly.GetExecutingAssembly().GetName().Version,
                              "https://github.com/dave-hillier/SolutionTools");

            Console.WriteLine(@"Usage:
    {0} <command> <project|solution|directory> [command options|options]

Commands:
    auto    Create a solution based on the inputs
    graph   Generate a graphviz compatible chart
    list    Prints the result to the specified output
    help    Display this help or help for a command

Options:
    --out <file>      File to save the result to 
    --consolein       Read the input from the console
    --include <regex> Only include projects matching the expression
    --exclude <regex> Exclude projects matching the expression

",
             Assembly.GetExecutingAssembly().GetName().Name);
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
