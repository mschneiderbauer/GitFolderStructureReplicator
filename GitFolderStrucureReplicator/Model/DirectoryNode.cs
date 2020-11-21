using System;
using System.Collections.Generic;
using System.Text;

namespace GitFolderStructureReplicator
{
    public class DirectoryNode
    {
        public string Path { get; set; }
        public Git Git { get; set; }

        public List<DirectoryNode> Children { get; set; }
    }
}
