using System;
using System.Collections.Generic;
using System.Text;

namespace GitFolderStructureReplicator
{
    public class Git
    {
        public string Url { get; set; }
        public string CurrentBranch { get; set; }
        public string Remote { get; set; }
        public string Merge { get; set; }

        public User User { get; set; }
    }
}
