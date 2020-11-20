using GitFolderStructureReplicator;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            DirectoryNode root = new DirectoryNode(null);
            s.CreateStructure(args[0], root);

            var n = root;
        }
    }
}
