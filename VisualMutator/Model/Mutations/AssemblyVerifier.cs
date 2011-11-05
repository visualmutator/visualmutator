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
        bool Verify( string assemblyPath);
    }

    public class AssemblyVerifier : IAssemblyVerifier
    {


        public AssemblyVerifier()
        {
          
        }

  
        public bool Verify( string assemblyPath)
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

            return p.ExitCode == 0;
        }

    }
}