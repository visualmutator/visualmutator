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
        private readonly IFactory<NUnitTester> _nUnitTesterFactory;
        private readonly ISettingsManager _settingsManager;
        private readonly CommonServices _svc;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _nunitConsolePath;

        public NUnitXmlTestService(
            INUnitWrapper nUnitWrapper, 
            IFactory<NUnitTester> nUnitTesterFactory,
            ISettingsManager settingsManager,
            CommonServices svc)
            : base(nUnitWrapper)
        {
            _nUnitTesterFactory = nUnitTesterFactory;
            _settingsManager = settingsManager;
            _svc = svc;

            _nunitConsolePath = FindConsolePath();
            _log.Info("Set NUnit Console path: " + _nunitConsolePath);
        }


        public override May<TestsLoadContext> LoadTests(string assemblyPath)
        {
            May<TestsLoadContext> loadTests = base.LoadTests(assemblyPath);

            UnloadTests();
            return loadTests;
        }

        public override void Cancel()
        {
            
        }

        private string FindConsolePath()
        {
            var nUnitDirPath = _settingsManager["NUnitConsoleDirPath"];
            var nUnitConsolePath = Path.Combine(nUnitDirPath, "nunit-console-x86.exe");
            
            if (!_svc.FileSystem.File.Exists(nUnitConsolePath))
            {
                throw new FileNotFoundException(nUnitConsolePath + " file was found.");
            }
            return nUnitConsolePath;
        }

        public NUnitTester SpawnTester(TestsRunContext arg)
        {
            return _nUnitTesterFactory.CreateWithParams(_nunitConsolePath, arg);
        }
    }
}