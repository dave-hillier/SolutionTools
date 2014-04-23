using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                var projects = GetProjectsAndDependencies(subOption);
                foreach (var project in projects)
                {
                    Console.WriteLine("{0}",project);
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
                var subOption = args[1];
                var projects = GetProjectsAndDependencies(subOption);

                Console.WriteLine("digraph dependencies {");
                foreach (var project in projects)
                {
                    foreach (var reference in ProjectReader.GetProjectReferences(project))
                    {
                        Console.WriteLine("\t\"{0}\"->\"{1}\";", ProjectReader.GetName(project), ProjectReader.GetName(reference));
                    }
                }
                Console.WriteLine("}");
            }
        }

        private static IEnumerable<string> GetProjectsAndDependencies(string subOption)
        {
            // TODO: Ensure that as fully qualified path
            if (ProjectListBuilder.HasProjectExtension(subOption))
            {
                return ProjectListBuilder.FindAllDependencies(subOption).Concat(new[] {subOption});
            }
            if (Path.GetExtension(subOption) == ".sln")
            {
                var directory = Path.GetDirectoryName(subOption);
                return SolutionReader.GetProjects(subOption).Select(fn => Path.Combine(directory, fn));
            }
            if (IsDirectory(subOption))
            {
                return GetProjectsAndDependenciesInDirectory(subOption);
            }
            throw new NotSupportedException();
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


        private static IEnumerable<string> GetProjectsAndDependenciesInDirectory(string directory)
        {
            var projects = ProjectListBuilder.FindProjects(directory).ToArray();
            var deps = from f in projects
                       from dep in ProjectListBuilder.FindAllDependencies(f)
                       select dep;

            deps = deps.Concat(projects);
            return deps;
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
