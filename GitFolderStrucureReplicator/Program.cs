using GitFolderStructureReplicator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            Console.Write("Extract or replicate (e/r): ");
            bool extract = Console.ReadLine().ToLower().StartsWith('e');
            Console.WriteLine();

            Console.Write("Root Directory: ");
            string rootDirectory = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Folder for generated files (leave empty to place in program folder): ");
            string folderPath = Console.ReadLine();
            Console.WriteLine();

            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                Console.WriteLine("Root Directory cannot be empty!");
                return;
            }

            Structure s = new Structure(rootDirectory, folderPath);

            if (extract)
            {
                s.ExtractFolderStructure();
            }
            else
            {
                s.ReplicateFolderStructure();
            }
        }
    }
}
