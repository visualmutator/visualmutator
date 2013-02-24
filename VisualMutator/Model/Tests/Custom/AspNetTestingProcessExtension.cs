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

    [Export(typeof(ITestingProcessExtension))]
    public class AspNetTestingProcessExtension : ITestingProcessExtension
    {
        private DirectoryPathAbsolute _projectPath;

        private string _port;

        private string cmdPath;

  

        public void OnSessionStarting(string parameter, IList<string> projectPaths)
        {

            _projectPath = projectPaths.Select(p => new DirectoryPathAbsolute(p))
             .First(ass => ass.ChildrenFilesPath.Where(p => p.FileName == "Web.config").Any());


            _port = parameter;
            string content =
@"taskkill /F /IM WebDev.WebServer40.EXE
START /D ""C:\Program Files (x86)\Common Files\microsoft shared\DevServer\10.0\"" /B WebDev.WebServer40.EXE /port:" 
+ _port + @" /path:" + _projectPath.Path.InQuotes() + @" /vpath:""/""";

             cmdPath = Path.GetTempFileName();
            

            cmdPath = Path.ChangeExtension(cmdPath, "cmd");
            File.WriteAllText(cmdPath, content, Encoding.ASCII);
       


        }

        public void OnTestingOfMutantStarting(string mutantDestination, IList<string> mutantFilePaths)
        {
            string src = _projectPath.Concat("chromedriver.exe").Path;

            string dest = Path.Combine(mutantDestination, "chromedriver.exe");
            if (!File.Exists(dest))
            {
                File.Copy(src, dest, true);
            }

            foreach (var p in mutantFilePaths.Select(path=>new FilePathAbsolute(path)))
            {
                var mutdest = _projectPath.Concat("bin");//.Concat(p.FileName).Path;
             /*   if(File.Exists(mutdest.Concat(p.FileName).Path))
                {
                    File.Move(mutdest.Concat(p.FileName).Path, mutdest.Concat(p.FileNameWithoutExtension).Concat(".dlltmp").Path);
                }*/

                File.Copy(p.Path, mutdest.Concat(p.FileName).Path, true);
            }


            Process.Start(cmdPath);

        }

        /// <summary>
        /// Run on cancellation of current test run or whole mutantion testing operation.
        /// </summary>
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
                return "ASP.NET Development Server Support";
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
                catch (Win32Exception )
                {
                    // process was terminating or can't be terminated - deal with it
                }
                catch (InvalidOperationException )
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