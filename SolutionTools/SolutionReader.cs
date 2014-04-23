using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SolutionTools
{
    internal class SolutionReader
    {
        public static IEnumerable<string> GetProjects(string subOption)
        {
            using (var f = File.OpenText(subOption))
            {
                var lines = ReadLines(f);
                return GetProjects(lines);
            }
        }

        private static IEnumerable<string> ReadLines(StreamReader f)
        {
            var lines = new List<string>();
            while (!f.EndOfStream)
                lines.Add(f.ReadLine());
            return lines;
        }

        private static IEnumerable<string> GetProjects(IEnumerable<string> lines)
        {
            var re = new Regex(@"Project\("".+?""\) = ""(.+?)"", ""(.+?)"", "".+?""");
            return from line in lines
                   let matches = re.Match(line)
                   where
                       matches.Success && matches.Groups.Count == 3 &&
                       matches.Groups[1].Value != matches.Groups[2].Value
                   select matches.Groups[2].Value;
        }
    }
}