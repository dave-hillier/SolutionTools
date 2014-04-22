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
    }
}