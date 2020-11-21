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
            //Console.WriteLine("Hello World!");
            //PowerShell script = PowerShell.Create();
            //script.AddScript($"cd {args[0]}");
            //script.AddScript("git config --get remote.origin.url");

            //Collection<PSObject> psOutput = script.Invoke();

            Structure s = new Structure();
            DirectoryNode root = new DirectoryNode();
            s.CreateStructure(args[0], root);

            var n = root;
            File.WriteAllText(@"dirstruct.json", JsonConvert.SerializeObject(root)); // TODO check out microsoft json
        }
    }
}
