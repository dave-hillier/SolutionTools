﻿using System;
using System.IO;

namespace SolutionTools
{
    internal static class PathHelper
    {
        public static string GetRelativePath(string destPath, string path)
        {
            var relativeUrl = new Uri(destPath).MakeRelativeUri(new Uri(path)).ToString();
            var stringToUnescape = relativeUrl.Replace('/', Path.DirectorySeparatorChar);
            return Uri.UnescapeDataString(stringToUnescape);
        }

        public static bool IsDirectory(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) != 0;
        }

        public static string ToPlatformPath(this string path) 
        {
            return path.Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}