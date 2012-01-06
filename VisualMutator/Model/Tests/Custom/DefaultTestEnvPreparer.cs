namespace VisualMutator.Model.Tests.Custom
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Text;

    using CommonUtilityInfrastructure;

    using VisualMutator.Model.Mutations.Structure;

    public class DefaultTestEnvPreparer
    {
        private string _projectPath;

        private string _port;

        private string cmdPath;

        public DefaultTestEnvPreparer()
        {
            
        }

        public void Initialize(string parameter)
        {

            var paramss = parameter.Split('|');

            _projectPath = paramss[0];
            _port = paramss[1];
            string content =
@"taskkill /F /IM WebDev.WebServer40.EXE
START /D ""C:\Program Files (x86)\Common Files\microsoft shared\DevServer\10.0\"" /B WebDev.WebServer40.EXE /port:" + _port + @" /path:" + _projectPath.InQuotes() + @" /vpath:""/""";

             cmdPath = Path.GetTempFileName();
            

            cmdPath = Path.ChangeExtension(cmdPath, "cmd");
            File.WriteAllText(cmdPath, content, Encoding.ASCII);
            //_projectPath = @"D:\PLIKI\Programowanie\C#\Source Code'y\ASP.NET MVC\MvcMusicStore 3\MvcMusicStore";


        }

        public void PrepareForMutant(string mutantDestination, Action<string> copyTo)
        {
            string src = @"D:\PLIKI\Programowanie\C#\Source Code'y\ASP.NET MVC\MvcMusicStore 3\chromedriver.exe";

            string dest = Path.Combine(mutantDestination, "chromedriver.exe");
            if (!File.Exists(dest))
            {
                File.Copy(src, dest, true);
            }

            
            copyTo(Path.Combine(_projectPath, "bin"));

            Process.Start(cmdPath);

        }
        public void OnTestingCancelled()
        {
            Kill("chromedriver");
            Kill("WebDev.WebServer40");
        }


        private void Kill(string name)
        {
            var procs = Process.GetProcessesByName(name);
            foreach (Process p in procs)
            {
                try
                {
                    KillProcessAndChildren(p.Id);
                  //  p.Kill();
                }
                catch (Win32Exception winException)
                {
                    // process was terminating or can't be terminated - deal with it
                }
                catch (InvalidOperationException invalidException)
                {
                    // process has already exited - might be able to let this one go
                }
            }

        }
        private void KillProcessAndChildren(int pid)
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
            { /* process already exited */
            }
           
        }

    }   
}