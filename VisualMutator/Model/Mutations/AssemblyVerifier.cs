namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure;

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
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@".\PEVerify.exe")
            {
                Arguments = assemblyPath.InQuotes(),
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