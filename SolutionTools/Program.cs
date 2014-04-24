﻿using System;
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
                // TODO: inclusive and exclusive filters
                GenerateSolution(input, sln, null, null);
            }
            else if (verb == "graph" && args.Length > 1)
            {
                var subOption = args[1];
                var projects = GetProjectsAndDependencies(subOption);
                var drawAssemblyReferences = args.Any(a => a.ToLowerInvariant() == "--assemblyreferences");
                GraphPrinter.PrintDependencyGraph(projects, drawAssemblyReferences, Console.Out);
            }
        }

        private static void PrintProjects(IEnumerable<string> projects)
        {
            foreach (var project in projects)
            {
                Console.WriteLine("{0}", project);
            }
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
                return SolutionReader.GetProjects(subOption).Select(fn => directory != null ? Path.Combine(directory, fn) : null).
                    Where(fn => fn != null);
            }
            if (PathHelper.IsDirectory(subOption))
            {
                return GetProjectsAndDependenciesInDirectory(subOption);
            }
            throw new NotSupportedException();
        }

        private static void GenerateSolution(string input, string sln, string excludeRegex, string includeRegex)
        {
            var projects = GetProjectsAndDependencies(input);
            projects = ApplyFilters(excludeRegex, includeRegex, projects);
            SolutionWriter.WriteSolution(projects, sln, path => FolderSelector.GetSlnFolder(sln, path), IsTestProject);
        }

        private static IEnumerable<string> ApplyFilters(string excludeRegex, string includeRegex, IEnumerable<string> projects)
        {
            projects = InclusiveFilter(includeRegex, projects);
            projects = ExclusiveFilter(excludeRegex, projects);
            return projects;
        }

        private static IEnumerable<string> ExclusiveFilter(string excludeRegex, IEnumerable<string> projects)
        {
            if (excludeRegex != null)
            {
                var re = new Regex(excludeRegex);
                projects = projects.Where(p => !re.IsMatch(p));
            }
            return projects;
        }

        private static IEnumerable<string> InclusiveFilter(string includeRegex, IEnumerable<string> projects)
        {
            if (includeRegex != null)
            {
                var re = new Regex(includeRegex);
                projects = projects.Where(p => re.IsMatch(p));
            }
            return projects;
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
            SolutionWriter.WriteSolution(lines, outputSlnPath, path => FolderSelector.GetSlnFolder(outputSlnPath, path), IsTestProject);
        }

        private static bool IsTestProject(string fn)
        {
            return TestFolderRegex.IsMatch(fn);
        }
    }
}
