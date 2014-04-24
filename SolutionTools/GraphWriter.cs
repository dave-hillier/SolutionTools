using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionTools
{
    public class GraphWriter : IProjectListWriter
    {
        private readonly bool _drawAssemblyReferences;

        public GraphWriter(bool drawAssemblyReferences)
        {
            _drawAssemblyReferences = drawAssemblyReferences;
        }

        public void Write(IEnumerable<string> projects, TextWriter textWriter)
        {
            //http://stamm-wilbrandt.de/GraphvizFiddle/
            projects = projects.ToArray();

            textWriter.WriteLine("digraph dependencies {");
            WriteProjectNodes(projects, textWriter);

            if (_drawAssemblyReferences)
            {
                WriteAssemblyNodes(projects, textWriter);
            }

            textWriter.WriteLine();
            foreach (var project in projects)
            {
                WriteProjectReferences(textWriter, project);
                if (_drawAssemblyReferences)
                {
                    WriteAssemblyReferences(textWriter, project);
                }
            }
            textWriter.WriteLine("}");
        }

        private static void WriteProjectNodes(IEnumerable<string> projects, TextWriter textWriter)
        {
            textWriter.Write("\tnode[shape = box]; ");
            foreach (var project in projects)
            {
                textWriter.Write("\"{0}\";", ProjectParser.GetName(project));
            }
            textWriter.WriteLine();
        }

        private static void WriteAssemblyNodes(IEnumerable<string> projects, TextWriter textWriter)
        {
            var refs = from project in projects
                       from aref in ProjectParser.GetAssemblyReferences(project)
                       select aref;
            textWriter.Write("\tnode[shape = ellipse]; ");
            foreach (var aref in refs)
            {
                textWriter.Write("\"{0}\";", aref);
            }
        }

        private static void WriteAssemblyReferences(TextWriter textWriter, string project)
        {
            foreach (var reference in ProjectParser.GetAssemblyReferences(project))
            {
                textWriter.WriteLine("\t\"{0}\"->\"{1}\";", ProjectParser.GetName(project), reference);
            }
        }

        private static void WriteProjectReferences(TextWriter textWriter, string project)
        {
            foreach (var reference in ProjectParser.GetProjectReferences(project))
            {
                textWriter.WriteLine("\t\"{0}\"->\"{1}\";", ProjectParser.GetName(project), ProjectParser.GetName(reference));
            }
        }
    }
}