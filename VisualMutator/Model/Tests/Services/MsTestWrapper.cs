namespace VisualMutator.Model.Tests.Services
{
    #region Usings

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure;

    using VisualMutator.Infrastructure;

    #endregion

    public interface IMsTestWrapper
    {
        //  IEnumerable<MethodDefinition> ReadTestMethodsFromAssembly(string assembly);

        XDocument RunMsTest(IEnumerable<string> assemblies);
    }

    public class MsTestWrapper : IMsTestWrapper
    {
        private readonly IVisualStudioConnection _visualStudio;

        public MsTestWrapper(IVisualStudioConnection visualStudio)
        {
            _visualStudio = visualStudio;
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

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(Path.Combine(_visualStudio.InstallPath, @"Common7\IDE\MSTest.exe"))
            {   
                Arguments = arguments.ToString(), 
                WindowStyle = ProcessWindowStyle.Hidden, 
                CreateNoWindow = true, 
                UseShellExecute = false, 
                RedirectStandardOutput = true 
            };

            p.Start();

            StreamReader sr = p.StandardOutput;
            string consoleOutput = sr.ReadToEnd();

            p.WaitForExit();

            try
            {
                return XDocument.Load(resultsFile);
            }
            catch (FileNotFoundException e)
            {
                throw new MsTestException(consoleOutput, e);
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