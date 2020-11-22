
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GitFolderStructureReplicator
{
    public class Structure
    {
        public List<Log> Logs { get; } = new List<Log>();
        public string RootPath { get; private set; }

        private Serializer serializer;
        private GitInterface gitInterface;

        public Structure(string rootPath, string folderPath)
        {
            this.RootPath = rootPath;
            this.serializer = new Serializer(folderPath);
            this.gitInterface = new GitInterface();
        }

        public void ExtractFolderStructure()
        {
            DirectoryNode root = ExtractStructure(RootPath);
            root.Name = "root";

            if (serializer.WriteToFile(root, Logs))
            {
                System.Console.WriteLine("Extraction successfull");
            }
            else
            {
                System.Console.WriteLine("Error occured during extraction, see logs_extraction.txt for details");
            }
        }

        public void ReplicateFolderStructure()
        {
            DirectoryNode root = serializer.ReadFromFile(Logs);
            if (root != null)
            {
                ReplicateStructure(RootPath, root);
            }
            serializer.WriteLogs(Logs, false);
        }

        private DirectoryNode ExtractStructure(string rootPath)
        {
            DirectoryNode root = new DirectoryNode();
            ExtractStructure(rootPath, root);
            return root;
        }

        private bool ExtractStructure(string path, DirectoryNode node)
        {
            //node.Path = GetRelativePath(path);
            //node.Name = Path.GetFileName(path);
            node.Name = Path.GetFileName(path);

            string[] dirs = Directory.GetDirectories(path);
            dirs = FilterDirectories(dirs);
            if (dirs.Length == 0)
            {
                return false;
            }

            if (dirs.Where(d => d.EndsWith(".git")).Count() > 0)
            {
                string url = gitInterface.GetUrl(path, Logs);
                string currentBranch = gitInterface.GetCurrentBranch(path, Logs);

                node.Git = new Git { Url = url.ToString().Trim(), CurrentBranch = currentBranch.Trim() };
                return true;
            }
            else
            {
                bool hasGitChild = SearchInSubDirectories(node, dirs);
                return hasGitChild;
            }
        }

        private void ReplicateStructure(string path, DirectoryNode node)
        {
            if (node == null)
            {
                return;
            }
            if (node.Git != null)
            {
                gitInterface.Clone(path, node, Logs);
            }
            else
            {
                if (node.Children != null && node.Children.Count > 0)
                {
                    if (node.Name != "root")
                    {
                        path += "\\" + node.Name;
                        Directory.CreateDirectory(path);
                    }
                    foreach (var child in node.Children)
                    {
                        ReplicateStructure(path, child);
                    }
                }
            }
        }

        private bool SearchInSubDirectories(DirectoryNode node, string[] dirs)
        {
            bool hasGitChild = false;
            node.Children = new List<DirectoryNode>();
            foreach (var dir in dirs)
            {
                DirectoryNode n = new DirectoryNode();

                if (ExtractStructure(dir, n))
                {
                    node.Children.Add(n);
                    hasGitChild = true;
                }
            }
            return hasGitChild;
        }

        private string[] FilterDirectories(string[] dirs)
        {
            return dirs.Where(d => !Excludes.FolderNames.Any(e => d.EndsWith(e))).ToArray();
        }

    }
}
