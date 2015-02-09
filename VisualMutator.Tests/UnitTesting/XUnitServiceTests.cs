namespace VisualMutator.Tests.UnitTesting
{
    #region

    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.Tests;
    using Model.Tests.Services;
    using Ninject;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using Strilanc.Value;
    using UsefulTools.ExtensionMethods;
    using Util;
    using VisualMutator.Infrastructure;

    #endregion

    [TestFixture]
    public class XUnitServiceTests : IntegrationTest
    {


//        [SetUp]
//        public void Setup()
//        {
//            _kernel = new StandardKernel();
//            _kernel.Load(new IntegrationTestModule());
//            _kernel.Bind<XUnitTestsRunContext>().ToSelf().AndFromFactory();
//            _kernel.Bind<XUnitTestService>().ToSelf();
//        }


        [Test]
        public void LoadTests()
        {
            _kernel.Bind<XUnitTestsRunContext>().ToSelf().AndFromFactory();
            _kernel.Bind<XUnitTestService>().ToSelf();
            var service = _kernel.Get<XUnitTestService>();
            var loadCtx = service.LoadTests(TestProjects.AutoMapper).ForceGetValue();
            foreach (var ns in loadCtx.Namespaces)
            {
                ns.IsIncluded = true;
            }
            loadCtx.ClassNodes.Count.ShouldBeGreaterThan(0);

            var runCtx = service.CreateRunContext(loadCtx, TestProjects.AutoMapper);
            var results = runCtx.RunTests().Result;
            results.ResultMethods.Count.ShouldBeGreaterThan(0);
//            foreach (var tmpTestNodeMethod in results.ResultMethods)
//            {
//                _log.Debug("Test: "+ tmpTestNodeMethod.Name+ " state: "+ tmpTestNodeMethod.State);
//            }
        }

        [Test]
        public void Whole()
        {
            string ConsolePath =
                @"C:\USERS\AREGO\APPDATA\LOCAL\MICROSOFT\VISUALSTUDIO\12.0EXP\EXTENSIONS\PIOTRTRZPIL\VISUALMUTATOR\2.0.8\xunitconsole\xunit.console.exe";
            string BasePath = @"C:\PLIKI\VisualMutator\testprojects\Automapper-Integration-Tests\";
            string unitestspath = BasePath + @"AutoMapper.dll";
            string unitestsdestination = BasePath + @"Debug\AutoMapper.dll";
            string unitestsdestinationPdb = BasePath + @"Debug\AutoMapper.pdb";
            string unitests = BasePath + @"Debug\AutoMapper.UnitTests.Net4.dll";
            var cci = new CciModuleSource(unitestspath);

          //  var rewr = new XRewriter(cci.Host);
           // rewr.Rewrite(cci.Module.Module);


            File.Delete(unitestsdestination);
            File.Delete(unitestsdestinationPdb);
            using (var file = File.OpenWrite(unitestsdestination))
            {
                cci.WriteToStream(cci.Module, file, unitestsdestination);
            }

      
            _log.Info("Running: " + ConsolePath.InQuotes() + " " + (unitests).InQuotes());
  
            var procResult = new Processes().RunHiddenAsync(ConsolePath, unitests);

            _log.Debug(procResult.Result.StandardOutput.Concat(procResult.Result.StandardError).Aggregate((a,b)=>a+"\n"+b));


//
//            var cci2 = new CciModuleSource(unitestspath);
//            var rewr = new XRewriter(cci2.Host);
//            
//            rewr.Rewrite(cci2.Module.Module);
//
//            File.Delete(unitestsdestination);
//            File.Delete(unitestsdestinationPdb);
//            using (var file = File.OpenWrite(unitestsdestination))
//            {
//                cci2.WriteToStream(cci2.Module, file, unitestsdestination);
//            }
//
//
//            _log.Info("Running: " + ConsolePath.InQuotes() + " " + arg);
//           
//            var procResult2 = new Processes().RunHiddenAsync(ConsolePath, unitestsdestination);
//            _log.Debug(procResult2.Result.StandardOutput.Concat(procResult2.Result.StandardError).Aggregate((a, b) => a + "\n" + b));

        }
    }

    public class XRewriter : CodeRewriter
    {
        public XRewriter(IMetadataHost host, bool copyAndRewriteImmutableReferences = false)
            : base(host, copyAndRewriteImmutableReferences)
        {
        }

//        public override IExpression Rewrite(IAddition addition)
//        {
//            var sub = new Subtraction();
//            sub.Type = addition.Type;
//            sub.CheckOverflow = addition.CheckOverflow;
//            sub.RightOperand = addition.RightOperand;
//            sub.LeftOperand = addition.LeftOperand;
//            sub.TreatOperandsAsUnsignedIntegers = addition.TreatOperandsAsUnsignedIntegers;
//            sub.Locations = addition.Locations.ToList();
//            sub.ResultIsUnmodifiedLeftOperand = addition.ResultIsUnmodifiedLeftOperand;
//            return sub;
//        }
    }
}