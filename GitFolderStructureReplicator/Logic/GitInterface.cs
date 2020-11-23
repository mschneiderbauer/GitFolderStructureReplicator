using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public string CallGitCommand(string path, string argument, List<Log> logs)
        {
            psi.Arguments = argument;
            psi.WorkingDirectory = path;

            try
            {
                Process p = Process.Start(psi);

                string result = p.StandardOutput.ReadToEnd();
                string resulterr = p.StandardError.ReadToEnd();

                p.WaitForExit();
                p.Close();

                if (!string.IsNullOrWhiteSpace(resulterr))
                {
                    logs.Add(new Log { DateTime = DateTime.Now, Message = resulterr, Path = path });
                }
                return result;
            }
            catch (Exception ex)
            {
                logs.Add(new Log { DateTime = DateTime.Now, Message = ex.Message, Path = path });
                return string.Empty;
            }
        }

        public bool CallGitActionCommand(string path, string argument, List<Log> logs)
        {
            psi.Arguments = argument;
            psi.WorkingDirectory = path;

            try
            {
                Process p = new Process();
                StringBuilder result = new StringBuilder();
                p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        result.Append(e.Data).Append(Environment.NewLine);
                        Console.WriteLine(e.Data);
                    }
                });

                StringBuilder error = new StringBuilder();
                p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        error.Append(e.Data).Append(Environment.NewLine);
                        Console.WriteLine(e.Data);
                    }
                });

                p.StartInfo = psi;
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
                p.Close();

                if (error.Length > 0)
                {
                    logs.Add(new Log { DateTime = DateTime.Now, Message = error.ToString(), Path = path });
                }
                if(result.Length > 0)
                {
                    logs.Add(new Log { DateTime = DateTime.Now, Message = result.ToString(), Path = path });
                }
                return true;
            }
            catch (Exception ex)
            {
                logs.Add(new Log { DateTime = DateTime.Now, Message = ex.Message, Path = path });
                return false;
            }
        }

        public string GetUrl(string path, List<Log> logs)
        {
            return CallGitCommand(path, "config --get remote.origin.url", logs);
        }

        public string GetCurrentBranch(string path, List<Log> logs)
        {
            return CallGitCommand(path, "symbolic-ref --short HEAD", logs);
        }

        public void Clone(string path, DirectoryNode node, List<Log> logs)
        {
            string dirName = node.Name;
            Console.WriteLine($"Cloning into {path} from {node.Git.Url}...");
            CallGitActionCommand(path, $"clone {node.Git.Url} {dirName}", logs);
            Console.WriteLine();

            if (Directory.Exists($"{path}\\{dirName}"))
            {
                Console.WriteLine($"Trying to checkout {node.Git.CurrentBranch}...");
                CallGitActionCommand($"{path}\\{dirName}", $"checkout {node.Git.CurrentBranch}", logs);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cloning error, directory doesn't exist");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
        }
    }
}
