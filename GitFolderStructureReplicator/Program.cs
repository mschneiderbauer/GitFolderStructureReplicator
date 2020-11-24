using System;

namespace GitFolderStructureReplicator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Extract or replicate (e/r): ");
            bool extract = Console.ReadLine().ToLower().StartsWith('e');

            Console.Write("Root Directory: ");
            string rootDirectory = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Root Directory cannot be empty!");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.Write("Folder for generated files (leave empty to place in program folder): ");
            string folderPath = Console.ReadLine();
            Console.WriteLine();         

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
