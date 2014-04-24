using System.IO;
using System.Linq;

namespace SolutionTools
{
    public class FolderSelector
    {
        public static string GetSlnFolderByNamespace(string sln, string project)
        {
            var fn = Path.GetFileName(project);
            var s = fn.Split('.');
            return string.Join(".", s.Take(2));
        }

        public static string GetSlnFolder(string sln, string project)
        {

            var slnDir = Path.GetDirectoryName(sln);
            var projectDir = Path.GetDirectoryName(project);
            var next = Path.GetDirectoryName(projectDir);
            if (next == slnDir)
                return "";
            // TODO: null checks

            while (slnDir != next)
            {
                projectDir = next; 
                next = Path.GetDirectoryName(projectDir);
            }

            return projectDir.Substring(slnDir.Length).Trim('\\');
        }
    }
}