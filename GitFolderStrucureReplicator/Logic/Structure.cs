
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


        public List<string> Logs { get; } = new List<string>();
        public string RootPath { get; private set; }

        private Serializer serializer;
        private GitInterface gitInterface;

        public Structure()
        {
            this.serializer = new Serializer();
            this.gitInterface = new GitInterface();
        }

        public void CreateAndWriteFolderStructure(string rootPath)
        {
            this.RootPath = rootPath;

            DirectoryNode root = GetStructureRoot(rootPath);
            root.Path = "root";

            serializer.WriteToFile(root, Logs);
        }

        private DirectoryNode GetStructureRoot(string rootPath)
        {
            DirectoryNode root = new DirectoryNode();
            CreateStructure(rootPath, root);
            return root;
        }

        private bool CreateStructure(string path, DirectoryNode node)
        {
            node.Path = GetRelativePath(path);

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

        private string GetRelativePath(string path)
        {
            return path.Replace(RootPath, string.Empty).TrimStart('\\');
        }

        private bool SearchInSubDirectories(DirectoryNode node, string[] dirs)
        {
            bool hasGitChild = false;
            node.Children = new List<DirectoryNode>();
            foreach (var dir in dirs)
            {
                DirectoryNode n = new DirectoryNode();

                if (CreateStructure(dir, n))
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
