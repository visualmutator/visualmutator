namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure;

    using VisualMutator.Infrastructure;

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
        /*
        public IEnumerable<MethodDefinition> ReadTestMethodsFromAssembly(string assembly)
        {
            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(assembly);
            IEnumerable<TypeDefinition> types =
                ad.MainModule.Types.Where(
                    t =>
                    t.CustomAttributes.Any(
                        a =>
                        a.AttributeType.FullName ==
                        @"Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute")).ToList();

            return types.SelectMany(t => t.Methods).Where(
                m => m.CustomAttributes.Any(
                    a =>
                    a.AttributeType.FullName ==
                    @"Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute"));
        }
        */
        
        public XDocument RunMsTest(IEnumerable<string> assemblies)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(Path.Combine(_visualStudio.InstallPath, @"Common7\IDE\MSTest.exe"));


            string settingsPath = TestSettings();
            string resultsFile = Path.GetTempFileName();

        
             File.Delete(resultsFile);



            var arguments = new StringBuilder();
            arguments.Append(@"-noisolation" + " ");
            arguments.Append(@"-testsettings:" +settingsPath.InQuotes()+ " ");

            foreach (string assembly in assemblies)
            {
                arguments.Append(@"-testcontainer:" + assembly.InQuotes() + " ");
            }

            
            arguments.Append(@"-resultsfile:" + resultsFile.InQuotes());


            p.StartInfo.Arguments = arguments.ToString();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
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
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<TestSettings
  id=""906380a7-c058-43a6-8f36-966013e5a9eb""
  name=""UnitTests""
  enableDefaultDataCollectors=""false""
  xmlns=""http://microsoft.com/schemas/VisualStudio/TeamTest/2010"">
  <Deployment enabled=""false"" />
  <Description>This test run configuration is used for running the unit tests</Description>
</TestSettings>
";
            string path = Path.GetTempFileName();

            File.WriteAllText(path, content);

     
            return path;
        }


    }
}