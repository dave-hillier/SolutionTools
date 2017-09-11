using Xunit;

namespace SolutionTools.Tests
{
    public class FolderSelectorTests
    {
        [Fact]
        public void GetSlnFolder()
        {
            var result = FolderSelector.GetSlnFolder(@"c:\project\project.sln".ToPlatformPath(), @"c:\project\folder\a\a.csproj".ToPlatformPath());
            Assert.Equal("folder", result);
        }

        [Fact]
        public void GetNoSlnFolder()
        {
            var result = FolderSelector.GetSlnFolder(@"c:\project\project.sln".ToPlatformPath(), @"c:\project\a\a.csproj".ToPlatformPath());
            Assert.Equal("", result);
        }

        [Fact]
        public void GetSlnFolderByNamespace()
        {
            var result = FolderSelector.GetSlnFolderByNamespace(@"c:\project\project.sln".ToPlatformPath(), @"c:\project\a.b.c.csproj".ToPlatformPath());
            Assert.Equal("a.b", result); // TODO: or just b?
        }


        [Fact]
        public void GetSlnFolderByNamespaceIgnoreSubfolder()
        {
            var result = FolderSelector.GetSlnFolderByNamespace(@"c:\project\project.sln".ToPlatformPath(), @"c:\project\c\a.b.c.csproj".ToPlatformPath());
            Assert.Equal("a.b", result); 
        }
    }
}