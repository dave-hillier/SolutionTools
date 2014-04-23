using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace SolutionTools
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var verb = args[0];
            if (verb == "help" && args.Length > 1)
            {
                var subOption = args[1];

            }
            else if (verb == "list" && args.Length > 1)
            {
                var subOption = args[1];
                if (ProjectListBuilder.HasProjectExtension(subOption))
                {
                    Console.WriteLine("{0}", subOption);
                    var dependencies = ProjectListBuilder.FindAllDependencies(subOption);
                    foreach (var dependency in dependencies)
                    {
                        Console.WriteLine("{0}", dependency);
                    }
                }
                else if (Path.GetExtension(subOption) == "sln")
                {
                    // TODO: implement sln parser
                    // TODO: missing refs?
                }
                else if (IsDirectory(subOption))
                {
                    var projects = ProjectListBuilder.FindProjects(subOption);
                    // TODO: dependencies?
                    foreach (var dependency in projects)
                    {
                        Console.WriteLine("{0}", dependency);
                    }
                }
            }
            else if (verb == "sln" && args.Length > 1)
            {
                // TODO: folder strategy
                var stdin = Console.In.ReadToEnd();
                var lines = stdin.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                SolutionWriter.WriteSolution(lines, args[1], fn => "Projects",
                                             fn => fn.ToLowerInvariant().Contains("test"));
            }
            else if (verb == "auto" && args.Length > 2)
            {
                var sln = args[1];
                var input = args[2];

                IEnumerable<string> projects = new string[] {};
                if (IsDirectory(input))
                {
                    projects = ProjectListBuilder.FindProjects(Path.GetDirectoryName(sln));
                    // TODO: get dependencies for each
                }
                else if (ProjectListBuilder.HasProjectExtension(input))
                {
                    projects = ProjectListBuilder.FindAllDependencies(input).Concat(new[] {input});
                }
                SolutionWriter.WriteSolution(projects, sln, fn => "fn", fn => fn.ToLowerInvariant().Contains("test"));
            }
        }

        private static bool IsDirectory(string input)
        {
            return (File.GetAttributes(input) & FileAttributes.Directory) != 0;
        }
    }
}
