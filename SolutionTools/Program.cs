using System;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace SolutionTools
{
    public class Program
    {
        static void Main(string[] args)
        {
            var verb = args[0];
            if (verb == "help" && args.Length > 1)
            {
                var subOption = args[1];

            }
            else if (verb == "list" && args.Length > 1)
            {
                var subOption = args[1];
                if (ProjectListBuilder.HasProjectExtension(subOption))
                {
                    Console.WriteLine("{0}", subOption);
                    var dependencies = ProjectListBuilder.FindAllDependencies(subOption);
                    foreach (var dependency in dependencies)
                    {
                        Console.WriteLine("{0}", dependency);
                    }
                }
                else if (Path.GetExtension(subOption) == "sln")
                {
                    // TODO: implement sln parser
                }
                // TODO: if directory find all proj and dependents
            }
            else if ()
            {
                
            }
        }


    }
}
