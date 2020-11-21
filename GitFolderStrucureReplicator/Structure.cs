
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
        private const string GIT_EXE = "git.exe";

        public List<string> Logs { get; set; } = new List<string>();

        public void CreateStructure(string path, DirectoryNode node)
        {
            node.Path = path;

            string[] dirs = Directory.GetDirectories(path); // TODO remove excluded folders
            if (dirs.Length == 0)
            {
                return;
            }

            if (dirs.Where(d => d.EndsWith(".git")).Count() > 0)
            {
                string url = CallGitCommand(path, "config --get remote.origin.url");
                string currentBranch = CallGitCommand(path, "symbolic-ref --short HEAD");

                node.Git = new Git { Url = url.ToString().Trim(), CurrentBranch = currentBranch.Trim() };
            }
            else
            {
                node.Children = new List<DirectoryNode>();
                foreach (var dir in dirs)
                {
                    DirectoryNode n = new DirectoryNode();

                    CreateStructure(dir, n);
                    node.Children.Add(n);
                }
            }
        }

        private string CallGitCommand(string path, string argument)
        {
            ProcessStartInfo psi = CreateGitProcessStartInfo(path);
            psi.Arguments = argument;

            Process p = Process.Start(psi);

            string result = p.StandardOutput.ReadToEnd();
            string resulterr = p.StandardError.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(resulterr))
            {
                Logs.Add(resulterr);
            }

            p.WaitForExit();
            p.Close();
            return result;
        }

        private ProcessStartInfo CreateGitProcessStartInfo(string path)
        {
            ProcessStartInfo psi = new ProcessStartInfo(GIT_EXE);
            psi.WorkingDirectory = path;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            return psi;
        }
    }
}
