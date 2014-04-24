using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SolutionTools
{
    class SolutionProjectsReader : IProjectReader
    {
        private readonly string _slnPath;

        public SolutionProjectsReader(string slnPath)
        {
            _slnPath = slnPath;
        }

        public IEnumerable<string> GetProjects()
        {
            var directory = Path.GetDirectoryName(_slnPath);
            using (var f = File.OpenText(_slnPath))
            {
                var lines = ReadLines(f);
                return ReadAllProjects(lines).
                    Select(fn => directory != null ? Path.Combine(directory, fn) : null).
                    Where(fn => fn != null);
            }
        }

        private static IEnumerable<string> ReadLines(StreamReader f)
        {
            var lines = new List<string>();
            while (!f.EndOfStream)
                lines.Add(f.ReadLine());
            return lines;
        }

        private static IEnumerable<string> ReadAllProjects(IEnumerable<string> lines)
        {
            var re = new Regex(@"Project\("".+?""\) = ""(.+?)"", ""(.+?)"", "".+?""");
            return from line in lines
                   let matches = re.Match(line) // TODO: use matches?
                   where matches.Success && matches.Groups.Count == 3 && matches.Groups[1].Value != matches.Groups[2].Value
                   select matches.Groups[2].Value;
        }
    }
}