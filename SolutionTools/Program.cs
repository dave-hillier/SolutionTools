using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
                    PrintAllDependencies(subOption);
                }
                else if (Path.GetExtension(subOption) == ".sln")
                {
                    PrintSlnProjects(subOption);
                }
                else if (IsDirectory(subOption))
                {
                    PrintAllDependenciesInDirectory(subOption);
                }
            }
            else if (verb == "sln" && args.Length > 1)
            {
                var outputSlnPath = args[1];
                WriteSlnFromStdIn(outputSlnPath);
            }
            else if (verb == "auto" && args.Length > 2)
            {
                var sln = args[1];
                var input = args[2];
                GenerateSolution(input, sln);
            }
            else if (verb == "dot" && args.Length > 1)
            {
                
            }
        }

        private static void PrintSlnProjects(string subOption)
        {
            using (var f = File.OpenText(subOption))
            {
                var lines = new List<string>();
                while (!f.EndOfStream)
                    lines.Add(f.ReadLine());
                var re = new Regex(@"Project\("".+?""\) = ""(.+?)"", ""(.+?)"", "".+?""");
                var projects = from line in lines
                               let matches = re.Match(line)
                               where
                                   matches.Success && matches.Groups.Count == 3 &&
                                   matches.Groups[1].Value != matches.Groups[2].Value
                               select matches.Groups[2].Value;
                foreach (var project in projects)
                {
                    Console.WriteLine("{0}", project);
                }
            }
        }

        private static void GenerateSolution(string input, string sln)
        {
            var projects = GetProjects(input, Path.GetDirectoryName(sln));
            // TODO: exclude regex?
            SolutionWriter.WriteSolution(projects, sln, GetSlnFolder, IsTestProject);
        }

        private static IEnumerable<string> GetProjects(string inputPath, string searchDirectory)
        {
            if (IsDirectory(inputPath))
            {
                return ProjectListBuilder.FindProjects(searchDirectory);
            }
            if (ProjectListBuilder.HasProjectExtension(inputPath))
            {
                return ProjectListBuilder.FindAllDependencies(inputPath).Concat(new[] {inputPath});
            }
            throw new NotSupportedException();
        }

        private static void PrintAllDependenciesInDirectory(string subOption)
        {
            var projects = ProjectListBuilder.FindProjects(subOption).ToArray();
            var deps = from f in projects
                       from dep in ProjectListBuilder.FindAllDependencies(f)
                       select dep;

            deps = deps.Concat(projects);
            
            foreach (var dependency in deps)
            {
                Console.WriteLine("{0}", dependency);
            }
        }

        private static void PrintAllDependencies(string project)
        {
            Console.WriteLine("{0}", project); // Should this be optional?
            var dependencies = ProjectListBuilder.FindAllDependencies(project);
            foreach (var dependency in dependencies)
            {
                Console.WriteLine("{0}", dependency);
            }
        }

        private static void WriteSlnFromStdIn(string outputSlnPath)
        {
            var stdin = Console.In.ReadToEnd();
            var lines = stdin.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            SolutionWriter.WriteSolution(lines, outputSlnPath, GetSlnFolder, IsTestProject);
        }

        private static bool IsTestProject(string fn)
        {
            return fn.ToLowerInvariant().Contains("test");
        }

        private static string GetSlnFolder(string project)
        {
            return "MyFolder"; // TODO: two directories above the project?
        }

        private static bool IsDirectory(string input)
        {
            return (File.GetAttributes(input) & FileAttributes.Directory) != 0;
        }
    }
}
