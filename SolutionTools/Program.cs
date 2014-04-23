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
            if (verb == "help")
            {
                var subOption = args.Length > 1 ? args[1] : "";
                if (subOption == "list")
                    Console.WriteLine("list (project|solution|directory)");
                if (subOption == "sln")
                    Console.WriteLine("sln (input sln)");
                else if (subOption == "graph")
                    Console.WriteLine("graph (project|solution|directory)");
                else
                {
                    Console.WriteLine("\tlist");
                    Console.WriteLine("\tsln");
                    Console.WriteLine("\tauto");
                    Console.WriteLine("\tgraph");

                }
            }
            else if (verb == "list" && args.Length > 1)
            {
                var subOption = args[1];
                var projects = GetProjectsAndDependencies(subOption);
                PrintProjects(projects);
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
            else if (verb == "graph" && args.Length > 1)
            {
                var subOption = args[1];
                var projects = GetProjectsAndDependencies(subOption);
                var drawAssemblyReferences = args.Any(a => a.ToLowerInvariant() == "--assemblyreferences");
                PrintDependencyGraph(projects, drawAssemblyReferences, Console.Out);
            }
        }

        private static void PrintProjects(IEnumerable<string> projects)
        {
            foreach (var project in projects)
            {
                Console.WriteLine("{0}", project);
            }
        }

        private static void PrintDependencyGraph(IEnumerable<string> projects, bool drawAssemblyReferences, TextWriter textWriter)
        {
            //http://stamm-wilbrandt.de/GraphvizFiddle/
            projects = projects.ToArray();

            textWriter.WriteLine("digraph dependencies {");
            textWriter.Write("\tnode[shape = box]; ");
            foreach (var project in projects)
            {
                textWriter.Write("\"{0}\";", ProjectReader.GetName(project));
                
            }
            textWriter.WriteLine();

            if (drawAssemblyReferences)
            {
                var refs = from project in projects
                           from aref in ProjectReader.GetAssemblyReferences(project)
                           select aref;
                textWriter.Write("\tnode[shape = ellipse]; ");
                foreach (var aref in refs)
                {
                    textWriter.Write("\"{0}\";", aref);

                }
            }
            textWriter.WriteLine();
            foreach (var project in projects)
            {
                foreach (var reference in ProjectReader.GetProjectReferences(project))
                {
                    textWriter.WriteLine("\t\"{0}\"->\"{1}\";", ProjectReader.GetName(project), ProjectReader.GetName(reference));
                }
                if (drawAssemblyReferences)
                {
                    foreach (var reference in ProjectReader.GetAssemblyReferences(project))
                    {
                        textWriter.WriteLine("\t\"{0}\"->\"{1}\";", ProjectReader.GetName(project), reference);
                    }
                }
            }
            textWriter.WriteLine("}");
        }

        private static IEnumerable<string> GetProjectsAndDependencies(string subOption)
        {
            if (ProjectListBuilder.HasProjectExtension(subOption))
            {
                return ProjectListBuilder.FindAllDependencies(subOption).Concat(new[] { subOption });
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
            var projects = GetProjectsAndDependencies(input);
            // TODO: exclude regex?
            SolutionWriter.WriteSolution(projects, sln, GetSlnFolder, IsTestProject);
        }

        private static IEnumerable<string> GetProjectsAndDependenciesInDirectory(string directory)
        {
            var projects = ProjectListBuilder.FindProjects(directory).ToArray();
            var deps = from f in projects
                       from dep in ProjectListBuilder.FindAllDependencies(f)
                       select dep;

            deps = deps.Concat(projects);
            return deps.OrderByDescending(a => a).Distinct();
        }

        private static void WriteSlnFromStdIn(string outputSlnPath)
        {
            var stdin = Console.In.ReadToEnd();
            var lines = stdin.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
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
