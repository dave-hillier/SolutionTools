using NUnit.Framework;

namespace SolutionTools.Tests
{
    [TestFixture]
    public class PathHelperTests
    {
        [Test]
        public void FileSameDirectory()
        {
            const string a = @"c:\test\test1.txt";
            const string b = @"c:\test\test2.txt";
            var c = PathHelper.GetRelativePath(a, b);
            Assert.AreEqual(c,  "test2.txt");
        }

        [Test]
        public void FileToDirectory()
        {
            const string a = @"c:\test\";
            const string b = @"c:\test\test2.txt";
            var c = PathHelper.GetRelativePath(a, b);
            Assert.AreEqual(c, "test2.txt");
        }

        [Test]
        public void FileToParentDirectory()
        {
            const string a = @"c:\";
            const string b = @"c:\test\test2.txt";
            var c = PathHelper.GetRelativePath(a, b);
            Assert.AreEqual(c, @"test\test2.txt");
        }

        [Test]
        public void FileToHigherDirectory()
        {
            const string a = @"c:\test1\test2\";
            const string b = @"c:\test\test2.txt";
            var c = PathHelper.GetRelativePath(a, b);
            Assert.AreEqual(c, @"..\..\test\test2.txt");
        }
    }
}
