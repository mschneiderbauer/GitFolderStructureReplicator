
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
        public void CreateStructure(string path, DirectoryNode node)
        {
            node.Path = path;

            string[] myDirs = Directory.GetDirectories(path);
            if (myDirs.Length == 0)
            {
                return;
            }

            if (myDirs.Where(d => d.EndsWith(".git")).Count() > 0)
            {
                ProcessStartInfo psi = new ProcessStartInfo("git.exe");
                psi.WorkingDirectory = path;
                psi.UseShellExecute = false;

                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                psi.Arguments = "config --get remote.origin.url";

                Process p = Process.Start(psi);

                string result = p.StandardOutput.ReadToEnd();
                string resulterr = p.StandardError.ReadToEnd();

                p.WaitForExit();
                p.Close();

                node.Git = new Git { Url = result.ToString().Trim('\n') };
            }
            else
            {
                node.Children = new List<DirectoryNode>();
                foreach (var dir in myDirs)
                {
                    DirectoryNode n = new DirectoryNode(node);
                    //n.Parent = node;
                    
                    CreateStructure(dir, n);
                    node.Children.Add(n);
                }
            }

        }
    }
}
