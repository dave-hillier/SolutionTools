using NUnit.Framework;

namespace SolutionTools.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        [Test]
        public void GetSlnFolder()
        {
            var result = Program.GetSlnFolder(@"c:\project\project.sln", @"c:\project\folder\a\a.csproj");
            Assert.AreEqual("folder", result);
        }

        [Test]
        public void GetNoSlnFolder()
        {
            var result = Program.GetSlnFolder(@"c:\project\project.sln", @"c:\project\a\a.csproj");
            Assert.AreEqual("", result);
        }
    }
}