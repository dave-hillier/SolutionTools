using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionTools
{

    internal class SolutionWriter : IProjectListWriter
    {
        private readonly TextWriter _textWriter;
        private readonly string _outputSlnPath;
        private readonly Func<string, string> _folderNameSelector;
        private readonly Func<string, bool> _isTestFolder;

        public SolutionWriter(TextWriter textWriter, string outputSlnPath, 
            Func<string, string> folderNameSelector, Func<string, bool> isTestFolder)
        {
            _textWriter = textWriter;
            _outputSlnPath = outputSlnPath;
            _folderNameSelector = folderNameSelector;
            _isTestFolder = isTestFolder;
        }

        public void Write(IEnumerable<string> projects)
        {
            string slnDirectory = Path.GetDirectoryName(_outputSlnPath) + Path.DirectorySeparatorChar;
            var grouped = projects.GroupBy(_folderNameSelector);

            WriteHeader(_textWriter);

            var seenElements = new HashSet<string>();
            var nestedFolderGuids = new List<string>();

            foreach (var @group in grouped)
            {
                var projectFolder = Guid.NewGuid();

                if (@group.Key != "")
                    WriteProject(_textWriter, projectFolder, @group.Key);

                var testProjects = false;
                var testFolderGuid = Guid.NewGuid();
                foreach (var file in @group)
                {
                    var relative = PathHelper.GetRelativePath(slnDirectory, file);
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName != null && seenElements.Add(relative))
                    {
                        var projectEntryGuid = Guid.NewGuid();
                        WriteProject(_textWriter, fileName, relative, projectEntryGuid);

                        if (!string.IsNullOrEmpty(@group.Key))
                        {
                            testProjects = AddToNestedFolders(_isTestFolder, fileName, nestedFolderGuids, projectEntryGuid, testFolderGuid, testProjects, projectFolder);
                        }
                    }
                    // TODO: error?
                }
                if (testProjects)
                {
                    WriteProject(_textWriter, testFolderGuid, "Tests");
                    nestedFolderGuids.Add(String.Format("{{{0}}} = {{{1}}}", testFolderGuid, projectFolder));
                }
            }

            _textWriter.WriteLine("Global");
            WriteNestedProjects(_textWriter, nestedFolderGuids);
            // HACK: omit the build configurations - visual studio does this
            _textWriter.WriteLine("EndGlobal");
        }

        private static void WriteHeader(TextWriter writer)
        {
            //Microsoft Visual Studio Solution File, Format Version 11.00
            //# Visual Studio 2010

            writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00"); // TODO: parameterize
            writer.WriteLine("# Visual Studio 2012");
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