namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;

    public interface IMsTestWrapper
    {
        IEnumerable<MethodDefinition> ReadTestMethodsFromAssembly(string assembly);

        XDocument RunMsTest(IEnumerable<string> assemblies);
    }

    public class MsTestWrapper : IMsTestWrapper
    {
        private readonly IVisualStudioConnection _visualStudio;

        public MsTestWrapper(IVisualStudioConnection visualStudio)
        {
            _visualStudio = visualStudio;
        }

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

        
        public XDocument RunMsTest(IEnumerable<string> assemblies)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(Path.Combine(_visualStudio.InstallPath, @"Common7\IDE\MSTest.exe"));
        

            var arguments = new StringBuilder();
            foreach (string assembly in assemblies)
            {
                arguments.Append(@"-testcontainer:" + assembly.InQuotes() + " ");
            }

            string resultsFile = Path.GetTempFileName();
            arguments.Append(@"-resultsfile:" + resultsFile.InQuotes());

            p.StartInfo.Arguments = arguments.ToString();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;


            File.Delete(resultsFile);

            p.Start();
            p.WaitForExit();

            return XDocument.Load(resultsFile);
            
        }



    }
}