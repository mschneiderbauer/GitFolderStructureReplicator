using GitFolderStructureReplicator;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;

namespace GitFolderStructureReplicator
{
    class Program
    {
        static void Main(string[] args)
        {
            Structure s = new Structure();

            s.CreateAndWriteFolderStructure(args[0]);

            
        }
    }
}
