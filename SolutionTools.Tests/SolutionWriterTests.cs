using System;
using System.Linq;
using System.IO;
using NUnit.Framework;

namespace SolutionTools.Tests
{
    [TestFixture]
    public class SolutionWriterTests
    {
        [Test]
        public void EmptySln() // TODO: is this valid?
        {
            var stringWriter = new StringWriter();
            var grouped = (new string[] {}).GroupBy(g => g);
            SolutionWriter.WriteSolution(@"c:\", stringWriter, grouped, t => false);

            var actual = stringWriter.ToString();
            Assert.AreEqual(@"Microsoft Visual Studio Solution File, Format Version 11.00
# Visual Studio 2010
Global
	GlobalSection(NestedProjects) = preSolution
	EndGlobalSection
EndGlobal
", actual);
        }

        [Test]
        public void OneProject()
        {
            var stringWriter = new StringWriter();
            var grouped = (new [] { @"c:\MySln\MyProject\MyProject.csproj" }).GroupBy(g => "Folder");
            SolutionWriter.WriteSolution(@"c:\MySln\", stringWriter, grouped, t => false);

            var actual = stringWriter.ToString().Split(new [] { Environment.NewLine }, StringSplitOptions.None);

            // TODO: bit fragile
            Assert.That(actual[2], Is.StringStarting(@"Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""Folder"", ""Folder"","));
            Assert.That(actual[3], Is.EqualTo("EndProject"));
            Assert.That(actual[4], Is.StringStarting(@"Project(""0"") = ""MyProject"", ""MyProject\MyProject.csproj"","));
            Assert.That(actual[5], Is.EqualTo("EndProject"));

            // TODO: nesting
        }

        [Test]
        public void TestFolder()
        {
            var stringWriter = new StringWriter();
            var grouped = (new[] { @"c:\MySln\MyProject\MyProject.csproj" }).GroupBy(g => "Folder");
            SolutionWriter.WriteSolution(@"c:\MySln\", stringWriter, grouped, t => true);

            var actual = stringWriter.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.That(actual[6], Is.StringStarting(@"Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""Tests"", ""Tests"","));
            Assert.That(actual[7], Is.EqualTo("EndProject"));
        }

        // TODO: check the nesting - its difficult with Guids
    }
}