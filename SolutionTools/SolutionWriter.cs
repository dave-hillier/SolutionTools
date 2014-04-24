using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionTools
{

    internal class SlnWriter : IProjectListWriter
    {
        private readonly string _outputSlnPath;
        private readonly Func<string, string> _folderNameSelector;
        private readonly Func<string, bool> _isTestFolder;

        public SlnWriter(string outputSlnPath,
            Func<string, string> folderNameSelector,
            Func<string, bool> isTestFolder)
        {
            _outputSlnPath = outputSlnPath;
            _folderNameSelector = folderNameSelector;
            _isTestFolder = isTestFolder;
        }

        public void Write(IEnumerable<string> projects, TextWriter textWriter)
        {
            string slnDirectory = Path.GetDirectoryName(_outputSlnPath) + Path.DirectorySeparatorChar;
            var grouped = projects.GroupBy(_folderNameSelector);
            using (var writer = new StreamWriter(_outputSlnPath))
            {
                WriteSolution(slnDirectory, writer, grouped, _isTestFolder);
            }
        }

        private static void WriteHeader(TextWriter writer)
        {
            //Microsoft Visual Studio Solution File, Format Version 11.00
            //# Visual Studio 2010

            writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00"); // TODO: parameterize
            writer.WriteLine("# Visual Studio 2012");
        }

        public static void WriteSolution(string solutionDirectory, TextWriter writer,
            IEnumerable<IGrouping<string, string>> grouped,
            Func<string, bool> isTestFolder)
        {
            WriteHeader(writer);

            var seenElements = new HashSet<string>();
            var nestedFolderGuids = new List<string>();

            foreach (var group in grouped)
            {
                var projectFolder = Guid.NewGuid();

                if (group.Key != "")
                    WriteProject(writer, projectFolder, @group.Key);

                var testProjects = false;
                var testFolderGuid = Guid.NewGuid();
                foreach (var file in @group)
                {
                    var relative = PathHelper.GetRelativePath(solutionDirectory, file);
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName != null && seenElements.Add(relative))
                    {
                        var projectEntryGuid = Guid.NewGuid();
                        WriteProject(writer, fileName, relative, projectEntryGuid);

                        if (!string.IsNullOrEmpty(@group.Key))
                        {
                            testProjects = AddToNestedFolders(isTestFolder, fileName, nestedFolderGuids, projectEntryGuid, testFolderGuid, testProjects, projectFolder);
                        }
                    }
                    // TODO: error?
                }
                if (testProjects)
                {
                    WriteProject(writer, testFolderGuid, "Tests");
                    nestedFolderGuids.Add(String.Format("{{{0}}} = {{{1}}}", testFolderGuid, projectFolder));
                }
            }

            writer.WriteLine("Global");
            WriteNestedProjects(writer, nestedFolderGuids);
            // HACK: omit the build configurations - visual studio does this
            writer.WriteLine("EndGlobal");
        }

        private static bool AddToNestedFolders(Func<string, bool> isTestFolder, string fileName, List<string> nestedFolderGuids, Guid projectEntryGuid,
                                               Guid testFolderGuid, bool testProjects, Guid projectFolder)
        {
            if (isTestFolder(fileName))
            {
                nestedFolderGuids.Add(String.Format("{{{0}}} = {{{1}}}", projectEntryGuid, testFolderGuid));
                testProjects = true;
            }
            else
                nestedFolderGuids.Add(String.Format("{{{0}}} = {{{1}}}", projectEntryGuid, projectFolder));
            return testProjects;
        }

        private static void WriteProject(TextWriter writer, string fileName, string relative, Guid projectEntryGuid)
        {
            // HACK: put a zero in place of the project guid. I dont know what this should be.
            writer.WriteLine(@"Project(""0"") = ""{0}"", ""{1}"",""{{{2}}}""", fileName, relative, projectEntryGuid);
            writer.WriteLine("EndProject");
        }

        private static void WriteNestedProjects(TextWriter writer, IEnumerable<string> nestedGuids)
        {
            writer.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
            foreach (var nestedGuid in nestedGuids)
            {
                writer.WriteLine("\t\t{0}", nestedGuid);
            }
            writer.WriteLine("\tEndGlobalSection");
        }

        private static void WriteProject(TextWriter writer, Guid folderGuid, string folderName)
        {
            writer.WriteLine(@"Project(""{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}"") = ""{1}"", ""{1}"",""{{{0}}}""", folderGuid, folderName);
            writer.WriteLine("EndProject");
        }
    }
}