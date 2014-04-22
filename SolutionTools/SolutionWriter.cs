﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionTools
{
    internal class SolutionWriter
    {
        public static void WriteSolution(IEnumerable<string> projects, string outputSlnPath, Func<string, string> folderNameSelector)
        {
            string outputFolder = Path.GetDirectoryName(outputSlnPath) + Path.DirectorySeparatorChar;

            var grouped = projects.GroupBy(folderNameSelector);

            using (var writer = new StreamWriter(outputSlnPath))
            {
                WriteSolution(writer, grouped, outputFolder, fileName => fileName.Contains("Test"));
            }
        }

        private static void WriteHeader(StreamWriter writer)
        {
            writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 11.00"); // TODO: parameterize
            writer.WriteLine("# Visual Studio 2010");
        }

        public static void WriteSolution(StreamWriter writer, IEnumerable<IGrouping<string, string>> grouped, string outputFolder, Func<string, bool> isTestFolder)
        {
            WriteHeader(writer);

            var seenElements = new HashSet<string>();
            var nestedGuids = new List<string>();
            foreach (var group in grouped)
            {
                var projectFolder = Guid.NewGuid();
                writer.WriteLine(
                    @"Project(""{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}"") = ""{0}"", ""{1}"",""{{{2}}}""",
                    @group.Key,
                    @group.Key,
                    projectFolder);

                writer.WriteLine("EndProject");
                var testProjects = false;
                var testFolderGuid = Guid.NewGuid();
                foreach (var file in @group)
                {
                    var relative = PathHelper.GetRelativePath(outputFolder, file);
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName != null && seenElements.Add(relative))
                    {
                        var projectEntryGuid = Guid.NewGuid();
                        writer.WriteLine(@"Project(""0"") = ""{0}"", ""{1}"",""{{{2}}}""", fileName, relative, projectEntryGuid);
                        writer.WriteLine("EndProject");

                        if (isTestFolder(fileName))
                        {
                            nestedGuids.Add(String.Format("{{{0}}} = {{{1}}}", projectEntryGuid, testFolderGuid));
                            testProjects = true;
                        }
                        else
                            nestedGuids.Add(String.Format("{{{0}}} = {{{1}}}", projectEntryGuid, projectFolder));
                    }
                }
                if (testProjects)
                {
                    writer.WriteLine(@"Project(""{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}"") = ""Tests"", ""Tests"",""{{{0}}}""", testFolderGuid);
                    nestedGuids.Add(String.Format("{{{0}}} = {{{1}}}", testFolderGuid, projectFolder));
                }
            }

            writer.WriteLine("Global");
            writer.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
            foreach (var nestedGuid in nestedGuids)
            {
                writer.WriteLine("\t\t{0}", nestedGuid);
            }
            writer.WriteLine("\tEndGlobalSection");
            writer.WriteLine("EndGlobal");
        }

    }
}