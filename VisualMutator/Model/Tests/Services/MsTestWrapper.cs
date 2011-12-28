namespace VisualMutator.Model.Tests.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Exceptions;

    using log4net;

    #endregion

    public interface IMsTestWrapper
    {
        //  IEnumerable<MethodDefinition> ReadTestMethodsFromAssembly(string assembly);

        void Cancel();
        XDocument RunMsTest(IEnumerable<string> assemblies);
    }

    public class MsTestWrapper : IMsTestWrapper
    {
        private readonly IVisualStudioConnection _visualStudio;

        private readonly CommonServices _svc;

        private Process _proc;

        private object _locker = new object();

        private bool _isCancelled;
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public MsTestWrapper(
            IVisualStudioConnection visualStudio,
            CommonServices svc)
        {
            _visualStudio = visualStudio;
            _svc = svc;
        }

        public XDocument RunMsTest(IEnumerable<string> assemblies)
        {
            string settingsPath = TestSettings();
            string resultsFile = Path.GetTempFileName();

            File.Delete(resultsFile);

            var arguments = new StringBuilder();
            arguments.Append(@"-noisolation" + " ");
            arguments.Append(@"-testsettings:" + settingsPath.InQuotes() + " ");

            foreach (string assembly in assemblies)
            {
                arguments.Append(@"-testcontainer:" + assembly.InQuotes() + " ");
            }

            arguments.Append(@"-resultsfile:" + resultsFile.InQuotes());

            
            var startInfo = new ProcessStartInfo(Path.Combine(_visualStudio.InstallPath, @"Common7\IDE\MSTest.exe"))
            {   
                Arguments = arguments.ToString(), 
                WindowStyle = ProcessWindowStyle.Hidden, 
             //   CreateNoWindow = true, 
                UseShellExecute = false, 
                RedirectStandardOutput = true 
            };

            lock (_locker)
            {
                if (_isCancelled)
                {
                    _isCancelled = false;
                    throw new TestingCancelledException();
                }
                _proc = new Process();
                _proc.StartInfo = startInfo;
            }

            StringBuilder sb = new StringBuilder();

    
       //     var asyncOut = Observable.FromEvent<DataReceivedEventArgs>(_proc, "OutputDataReceived")
        //        .Subscribe((e) => sb.Append(e.EventArgs.Data));

            _proc.Start();

          //  StreamReader sr = _proc.StandardOutput;
        //    _proc.BeginOutputReadLine();
         //   string consoleOutput = sr.ReadToEnd();
          //  string consoleOutput = "";
            _proc.WaitForExit();

            lock (_locker)
            {
             //   asyncOut.Dispose();
                _proc = null;
                if (_isCancelled)
                {
                    _isCancelled = false;
                    throw new TestingCancelledException();
                }
            }
           

            try
            {
                return XDocument.Load(resultsFile);
            }
            catch (FileNotFoundException e)
            {
                throw new MsTestException(sb.ToString(), e);
            }
        }

      
        public void Cancel()
        {
            lock (_locker)
            {
                if (!_isCancelled)
                {
                    _isCancelled = true;
                    if (_proc != null)
                    {
                        try
                        {
                            _proc.Kill();
                        }
                        catch (Exception e)
                        {
                            _log.Warn(e);
                        }

                    }
                }
                
            }
            
        }

        private string TestSettings()
        {
            string content =
@"
<?xml version=""1.0"" encoding=""UTF-8""?>
<TestSettings
  id=""906380a7-c058-43a6-8f36-966013e5a9eb""
  name=""UnitTests""
  enableDefaultDataCollectors=""false""
  xmlns=""http://microsoft.com/schemas/VisualStudio/TeamTest/2010"">
  <Deployment enabled=""false"" />
  <Description>This test run configuration is used for running the unit tests</Description>
</TestSettings>
".Trim();
            string path = Path.GetTempFileName();

            File.WriteAllText(path, content);

            return path;
        }
    }
}