using Xunit;

namespace SolutionTools.Tests
{
    public class PathHelperTests
    {
        [Fact]
        public void FileSameDirectory()
        {
            const string a = @"c:\test\test1.txt";
            const string b = @"c:\test\test2.txt";
            var c = PathHelper.GetRelativePath(a.ToPlatformPath(), b.ToPlatformPath());
            Assert.Equal(c,  "test2.txt");
        }

        [Fact]
        public void FileToDirectory()
        {
            const string a = @"c:\test\"; // TODO - no trailing slash?
            const string b = @"c:\test\test2.txt";
            var c = PathHelper.GetRelativePath(a.ToPlatformPath(), b.ToPlatformPath());
            Assert.Equal(c, "test2.txt");
        }

        [Fact]
        public void FileToParentDirectory()
        {
            const string a = @"c:\";
            const string b = @"c:\test\test2.txt";
            var c = PathHelper.GetRelativePath(a.ToPlatformPath(), b.ToPlatformPath());
            Assert.Equal(c, @"test\test2.txt".ToPlatformPath());
        }

        [Fact]
        public void FileToHigherDirectory()
        {
            const string a = @"c:\test1\test2\";
            const string b = @"c:\test\test2.txt";
            var c = PathHelper.GetRelativePath(a.ToPlatformPath(), b.ToPlatformPath());
            Assert.Equal(c, @"..\..\test\test2.txt".ToPlatformPath());
        }
    }
}
