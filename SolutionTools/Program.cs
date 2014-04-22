using System;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace SolutionTools
{
    public class Program
    {
        public class ListSubOptions
        {
            [Option('a', "all", HelpText = "Help!")]
            public bool All { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        public class Options 
        {
            [VerbOption("list", HelpText = "List all dependencies for a project.")]
            public ListSubOptions ListVerb { get; set; }

            [VerbOption("sln", HelpText = "Create a solution.")]
            public ListSubOptions SlnVerb { get; set; }

            // TODO: auto generate

            [HelpVerbOption]
            public string GetUsage(string verb)
            {
                return HelpText.AutoBuild(this, verb);
            }
        }

        static void Main(string[] args)
        {
            var options = new Options();
            string invokedVerb;
            object invokedVerbInstance;

            if (CommandLine.Parser.Default.ParseArguments(args, options,
              (verb, subOptions) =>
              {
                  // if parsing succeeds the verb name and correct instance
                  // will be passed to onVerbCommand delegate (string,object)
                  invokedVerb = verb;
                  invokedVerbInstance = subOptions;
              }))
            {
            }
        }
    }
}
