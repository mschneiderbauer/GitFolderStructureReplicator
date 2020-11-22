using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GitFolderStructureReplicator
{
    public class GitInterface
    {
        private const string GIT_EXE = "git.exe";
        private ProcessStartInfo psi;

        public GitInterface()
        {
            this.psi = CreateGitProcessStartInfo();
        }

        private ProcessStartInfo CreateGitProcessStartInfo()
        {
            ProcessStartInfo psi = new ProcessStartInfo(GIT_EXE);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            return psi;
        }

        public string CallGitCommand(string path, string argument, List<string> logs)
        {
            psi.Arguments = argument;
            psi.WorkingDirectory = path;

            Process p = Process.Start(psi);

            string result = p.StandardOutput.ReadToEnd();
            string resulterr = p.StandardError.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(resulterr))
            {
                logs.Add(resulterr);
            }

            p.WaitForExit();
            p.Close();
            return result;
        }

        public string GetUrl(string path, List<string> logs)
        {
            return CallGitCommand(path, "config --get remote.origin.url", logs);
        }
        
        public string GetCurrentBranch(string path, List<string> logs)
        {
            return CallGitCommand(path, "symbolic-ref --short HEAD", logs);
        }
    }
}
