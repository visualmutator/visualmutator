namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.FileSystem;

    using VisualMutator.Model.Tests.Services;

    public interface IAssemblyVerifier
    {
        void Verify(string assemblyPath);
    }

    public class AssemblyVerifier : IAssemblyVerifier
    {


        public AssemblyVerifier()
        {
         
        }

        public void Verify( string assemblyPath)
        {
            var localPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            string path = Path.Combine(Path.GetDirectoryName(localPath), "PEVerify.exe");

            if (!File.Exists(path))
            {
                throw new AssemblyVerificationException("File " + path+" does not exists.");
            }
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(path)
            {
                Arguments = assemblyPath.InQuotes() + " /nologo",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };


            p.Start();

            StreamReader sr = p.StandardOutput;
            string consoleOutput = sr.ReadToEnd();

            p.WaitForExit();

            if(p.ExitCode != 0)
            {
                throw new AssemblyVerificationException(consoleOutput);
            }
        }

    }
}