using System;
using System.IO;
using System.Linq;

namespace SolutionTools
{
  class FolderSelector : IFolderSelector
  {
    private readonly string _sln;

    public FolderSelector(string sln)
    {
      _sln = sln;
    }

    public static string GetSlnFolderByNamespace(string sln, string project)
    {
      var fn = Path.GetFileName(project);
      var s = fn.Split('.');
      return string.Join(".", s.Take(2));
    }

    public static string GetSlnFolder(string sln, string project)
    {
      if (sln == null)
        throw new ArgumentNullException();
      if (project == null)
        throw new ArgumentNullException();

      var slnDir = Path.GetDirectoryName(sln);
      var projectDir = Path.GetDirectoryName(project); // TODO: must contain sln dir
      var next = Path.GetDirectoryName(projectDir);
      if (next == null)
        throw new Exception();

      if (next == slnDir)
        return "";



      while (slnDir != next)
      {
        projectDir = next;
        next = Path.GetDirectoryName(projectDir);
      }

      return projectDir.Substring(slnDir.Length).Trim('\\').Trim(Path.DirectorySeparatorChar);
    }

    public string GetFolder(string project)
    {
      return GetSlnFolder(_sln, project);
    }
  }
}