namespace VisualMutator.Infrastructure
{
    using System.Diagnostics;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using RunProcessAsTask;

    public interface IProcesses
    {
         Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo);
         Task<ProcessResults> RunAsync(string fileName);
         Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken);
         Task<ProcessResults> RunAsync(string fileName, string arguments);
         Task<ProcessResults> RunAsync(string fileName, string userName, SecureString password, string domain);
         Task<ProcessResults> RunAsync(string fileName, string arguments, string userName, SecureString password, string domain);
    }

    class Processes : IProcesses
    {
        public Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo)
        {
            return ProcessEx.RunAsync(processStartInfo);
        }

        public Task<ProcessResults> RunAsync(string fileName)
        {
            return ProcessEx.RunAsync(fileName);
        }

        public Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
        {
            return ProcessEx.RunAsync(processStartInfo, cancellationToken);
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
    }
}