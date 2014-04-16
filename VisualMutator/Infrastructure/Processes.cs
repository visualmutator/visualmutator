namespace VisualMutator.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using RunProcessAsTask;

    public interface IProcesses
    {
         Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo);
         Task<ProcessResults> RunAsync(string fileName);
         Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationTokenSource cancellationToken);
         Task<ProcessResults> RunAsync(string fileName, string arguments);
         Task<ProcessResults> RunAsync(string fileName, string userName, SecureString password, string domain);
         Task<ProcessResults> RunAsync(string fileName, string arguments, string userName, SecureString password, string domain);
    }

    public class Processes : IProcesses
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private HashSet<CancellationTokenRegistration> registrations = new HashSet<CancellationTokenRegistration>();

        public Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo)
        {
            return ProcessEx.RunAsync(processStartInfo);
        }

        public Task<ProcessResults> RunAsync(string fileName)
        {
            return ProcessEx.RunAsync(fileName);
        }

        public Task<ProcessResults> RunAsync(string fileName, string arguments)
        {
            return ProcessEx.RunAsync(fileName, arguments);
        }

        public Task<ProcessResults> RunAsync(string fileName, string userName, SecureString password, string domain)
        {
            return ProcessEx.RunAsync(fileName, userName, password, domain);
        }

        public Task<ProcessResults> RunAsync(string fileName, string arguments, string userName, SecureString password, string domain)
        {
            return ProcessEx.RunAsync(fileName, arguments, userName, password, domain);
        }


        public Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationTokenSource cancellationToken)
        {
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            var tcs = new TaskCompletionSource<ProcessResults>();

            var standardOutput = new List<string>();
            var standardError = new List<string>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardOutput.Add(args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardError.Add(args.Data);
                }
            };

            
            cancellationToken.Token.ThrowIfCancellationRequested();

            _log.Debug("Registering cancellation for " + cancellationToken.GetHashCode());

            cancellationToken.Token.Register(() =>
            {
                tcs.TrySetCanceled();
                KillProcessAndChildren(process.Id);
            });


               process.Exited += (sender, args) =>
               {
                   tcs.TrySetResult(new ProcessResults(process, standardOutput, standardError));
               };

            if (process.Start() == false)
            {
                tcs.TrySetException(new InvalidOperationException("Failed to start process"));
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }


        private static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
    }
}