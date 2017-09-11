using Xunit;

namespace SolutionTools.Tests
{
    public class ProjectListBuilderTests
    {
        [Fact]
        public void HasExtension()
        {
            const string csproj = @"c:\project\project.csproj";
            const string fsproj = @"project.fsproj";
            const string vbproj = @"project.vbproj";

            Assert.True(ProjectListBuilder.HasProjectExtension(csproj));
            Assert.True(ProjectListBuilder.HasProjectExtension(fsproj));
            Assert.True(ProjectListBuilder.HasProjectExtension(vbproj));
        }


        [Fact]
        public void NotExt()
        {
            const string sln = @"c:\project\project.sln";
            Assert.False(ProjectListBuilder.HasProjectExtension(sln));
        }
    }
}