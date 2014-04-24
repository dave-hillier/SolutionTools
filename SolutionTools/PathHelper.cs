using System;
using System.IO;

namespace SolutionTools
{
    internal class PathHelper
    {
        public static string GetRelativePath(string destPath, string path)
        {
            var relativeUrl = new Uri(destPath).MakeRelativeUri(new Uri(path)).ToString();
            var stringToUnescape = relativeUrl.Replace('/', Path.DirectorySeparatorChar);
            return Uri.UnescapeDataString(stringToUnescape);
        }

        public static bool IsDirectory(string input)
        {
            return (File.GetAttributes(input) & FileAttributes.Directory) != 0;
        }
    }
}