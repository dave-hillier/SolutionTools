using System;
using System.Linq;
using System.IO;
using Xunit;

namespace SolutionTools.Tests
{
  public class SolutionWriterTests
  {
    [Fact]
    public void EmptySln()
    {
      var stringWriter = new StringWriter();
      var sw = new SolutionWriter(stringWriter, @"c:\output.sln", g => g, t => false);
      sw.Write(new string[] { });

      var actual = stringWriter.ToString();
      Assert.Equal(@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 2012
Global
	GlobalSection(NestedProjects) = preSolution
	EndGlobalSection
EndGlobal
", actual);
    }

    [Fact]
    public void OneProject()
    {
      var stringWriter = new StringWriter();
      var grouped = new[] { @"c:\MySln\MyProject\MyProject.csproj".ToPlatformPath() };
      var sw = new SolutionWriter(stringWriter, @"c:\MySln\".ToPlatformPath(), f => "Folder", t => false);
      sw.Write(grouped);

      var actual = stringWriter.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

      Assert.Contains(@"Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""Folder"", ""Folder"",", actual[2]);
      Assert.Equal(actual[3], "EndProject");
      Assert.Contains(@"Project(""0"") = ""MyProject"", ""MyProject\MyProject.csproj"",".ToPlatformPath(), actual[4]);
      Assert.Equal(actual[5], "EndProject");
    }

    [Fact]
    public void TestFolder()
    {
      var stringWriter = new StringWriter();
      var grouped = new[] { @"c:\MySln\MyProject\MyProject.Test.csproj" };
      var sw = new SolutionWriter(stringWriter, @"c:\MySln\", f => "Folder", t => true);
      sw.Write(grouped);

      var actual = stringWriter.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

      Assert.Contains(@"Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""Tests"", ""Tests"",", actual[6]);
      Assert.Equal(actual[7], "EndProject");
    }

    // TODO: nesting test
  }
}