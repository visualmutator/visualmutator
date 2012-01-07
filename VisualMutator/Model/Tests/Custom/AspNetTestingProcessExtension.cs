namespace VisualMutator.Model.Tests.Custom
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Text;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Paths;

    using VisualMutator.Model.Mutations.Structure;

    [Export(typeof(ITestingProcessExtension))]
    public class AspNetTestingProcessExtension : ITestingProcessExtension
    {
        private string _projectPath;

        private string _port;

        private string cmdPath;

        public AspNetTestingProcessExtension()
        {
            
        }

        public void Initialize(string parameter, IList<DirectoryPathAbsolute> projectPaths)
        {

            _projectPath = projectPaths
             .First(ass => ass.ChildrenFilesPath.Where(p => p.FileName == "Web.config").Any()).Path;


            _port = parameter;
            string content =
@"taskkill /F /IM WebDev.WebServer40.EXE
START /D ""C:\Program Files (x86)\Common Files\microsoft shared\DevServer\10.0\"" /B WebDev.WebServer40.EXE /port:" 
+ _port + @" /path:" + _projectPath.InQuotes() + @" /vpath:""/""";

             cmdPath = Path.GetTempFileName();
            

            cmdPath = Path.ChangeExtension(cmdPath, "cmd");
            File.WriteAllText(cmdPath, content, Encoding.ASCII);
            //_projectPath = @"D:\PLIKI\Programowanie\C#\Source Code'y\ASP.NET MVC\MvcMusicStore 3\MvcMusicStore";




         
  

           //// 

        }

        public void PrepareForMutant(string mutantDestination, List<string> mutantFilePaths)
        {
            string src = @"D:\PLIKI\Programowanie\C#\Source Code'y\ASP.NET MVC\MvcMusicStore 3\chromedriver.exe";

            string dest = Path.Combine(mutantDestination, "chromedriver.exe");
            if (!File.Exists(dest))
            {
                File.Copy(src, dest, true);
            }

            
        //    copyTo();

            foreach (var p in mutantFilePaths)
            {
                File.Copy(p, Path.Combine(Path.Combine(_projectPath, "bin"), Path.GetFileName(p)), true);
            }


            Process.Start(cmdPath);

        }
        public void OnTestingCancelled()
        {
            Kill("chromedriver");
            Kill("WebDev.WebServer40");
        }

        public void OnSessionFinished()
        {
            
        }



        public string Name
        {
            get
            {
                return "ASP.NET Devevelopment Server Support";
            }
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