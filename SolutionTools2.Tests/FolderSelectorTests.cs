using NUnit.Framework;

namespace SolutionTools.Tests
{
    [TestFixture]
    public class FolderSelectorTests
    {
        [Test]
        public void GetSlnFolder()
        {
            var result = FolderSelector.GetSlnFolder(@"c:\project\project.sln", @"c:\project\folder\a\a.csproj");
            Assert.AreEqual("folder", result);
        }

        [Test]
        public void GetNoSlnFolder()
        {
            var result = FolderSelector.GetSlnFolder(@"c:\project\project.sln", @"c:\project\a\a.csproj");
            Assert.AreEqual("", result);
        }

        [Test]
        public void GetSlnFolderByNamespace()
        {
            var result = FolderSelector.GetSlnFolderByNamespace(@"c:\project\project.sln", @"c:\project\a.b.c.csproj");
            Assert.AreEqual("a.b", result); // TODO: or just b?
        }


        [Test]
        public void GetSlnFolderByNamespaceIgnoreSubfolder()
        {
            var result = FolderSelector.GetSlnFolderByNamespace(@"c:\project\project.sln", @"c:\project\c\a.b.c.csproj");
            Assert.AreEqual("a.b", result); 
        }
    }
}