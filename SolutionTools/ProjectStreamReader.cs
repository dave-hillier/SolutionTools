using System;
using System.Collections.Generic;
using System.IO;

namespace SolutionTools
{
    class ProjectStreamReader : IProjectReader
    {
        private readonly TextReader _reader;

        public ProjectStreamReader(TextReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<string> GetProjects()
        {
            string line;
            while ((line = _reader.ReadLine()) != null)
            {
                if (!ProjectListBuilder.HasProjectExtension(line))
                    throw new NotSupportedException("File type unknown");
                // TODO: do we care if this file exists...
                yield return line;
            }
        }
    }
}