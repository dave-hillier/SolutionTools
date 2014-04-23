using NUnit.Framework;

namespace SolutionTools.Tests
{
    [TestFixture]
    public class ProjectListBuilderTests
    {
        [Test]
        public void HasExtension()
        {
            const string csproj = @"c:\project\project.csproj";
            const string fsproj = @"project.fsproj";
            const string vbproj = @"project.vbproj";

            Assert.IsTrue(ProjectListBuilder.HasProjectExtension(csproj));
            Assert.IsTrue(ProjectListBuilder.HasProjectExtension(fsproj));
            Assert.IsTrue(ProjectListBuilder.HasProjectExtension(vbproj));
        }
    }
}