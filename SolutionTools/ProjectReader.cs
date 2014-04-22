using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SolutionTools
{
    internal class ProjectReader
    {
        static readonly XNamespace Xs = "http://schemas.microsoft.com/developer/msbuild/2003";

        public static IEnumerable<string> GetProjectReferences(string csproj)
        {
            var xmldoc = XDocument.Load(csproj);
            var directoryName = Path.GetDirectoryName(csproj);

            if (directoryName == null)
                return new string[] {};

            var projectReferences = from projectReferenceElement in xmldoc.Descendants(Xs + "ProjectReference")
                                    let xAttribute = projectReferenceElement.Attribute("Include")
                                    where xAttribute != null
                                    select xAttribute.Value;

            var projectsAndSilverlight = projectReferences.Concat(GetSilverlightApplicationReferences(xmldoc));

            return projectsAndSilverlight.Select(
                p => Path.GetFullPath(Path.Combine(directoryName, p)));
        }

        public static IEnumerable<string> GetSilverlightApplicationReferences(XDocument xmldoc)
        {
            var appList = xmldoc.Descendants(Xs + "SilverlightApplicationList").SingleOrDefault();
            if (appList != null)
            {
                return appList.Value.Split(',').Select(app => app.Split('|')[1]);
            }
            return new string[] { };
        }
    }
}