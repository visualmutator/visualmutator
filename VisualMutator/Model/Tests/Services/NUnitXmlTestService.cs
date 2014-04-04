namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using Infrastructure;
    using log4net;
    using NUnit.Core;
    using RunProcessAsTask;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    #endregion

    public class NUnitXmlTestService : NUnitTestService
    {
        private readonly INUnitExternal _nUnitExternal;
        private readonly IFactory<NUnitTester> _nUnitTesterFactory;
        private readonly CommonServices _svc;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string _assemblyPath;
        private string _nunitConsolePath;


        public string NUnitConsoleAltPath { get; set; }
        public string NUnitConsolePath { get; set; }

        public NUnitXmlTestService(
            INUnitWrapper nUnitWrapper, 
            INUnitExternal nUnitExternal,
            IFactory<NUnitTester> nUnitTesterFactory,
            CommonServices svc)
            : base(nUnitWrapper, svc.Logging)
        {
            _nUnitExternal = nUnitExternal;
            _nUnitTesterFactory = nUnitTesterFactory;
            _svc = svc;

            var localPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            NUnitConsoleAltPath = Path.Combine(Path.GetDirectoryName(localPath), "nunit-console.exe");
            NUnitConsolePath = Path.Combine(Path.GetDirectoryName(localPath), "nunit-console-x86.exe");

            _nunitConsolePath = FindConsolePath();
            _log.Info("Set NUnit Console path: " + _nunitConsolePath);
        }


        public override May<TestsLoadContext> LoadTests(string assemblyPath)
        {
            _assemblyPath = assemblyPath;
            May<TestsLoadContext> loadTests = base.LoadTests(assemblyPath);

            UnloadTests();
            return loadTests;
        }
        private string FindConsolePath()
        {
            string runPath;
            if (_svc.FileSystem.File.Exists(NUnitConsolePath))
            {
                runPath = NUnitConsolePath;
            }
            else if (_svc.FileSystem.File.Exists(NUnitConsoleAltPath))
            {
                runPath = NUnitConsoleAltPath;
            }
            else
            {
                throw new FileNotFoundException(NUnitConsolePath + " nor " + NUnitConsoleAltPath + " file was found.");
            }
            return runPath;
        }

        public NUnitTester SpawnTester(TestsRunContext arg)
        {
            return _nUnitTesterFactory.CreateWithParams(_nunitConsolePath, arg);
        }
    }
}