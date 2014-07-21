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

    public class NUnitXmlTestService : ITestsService
    {
        private readonly NUnitTestLoader _testsLoader;
        private readonly ISettingsManager _settingsManager;
        private readonly CommonServices _svc;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _nunitConsolePath;

        public string NunitConsolePath
        {
            get { return _nunitConsolePath; }
        }
        
        public NUnitXmlTestService(
            NUnitTestLoader testsLoader, 
            ISettingsManager settingsManager,
            CommonServices svc)
          
        {
            _testsLoader = testsLoader;
            _settingsManager = settingsManager;
            _svc = svc;

            _nunitConsolePath = FindConsolePath();
            _log.Info("Set NUnit Console path: " + _nunitConsolePath);
        }

        public May<TestsLoadContext> LoadTests(string assemblyPath)
        {
            return _testsLoader.LoadTests(assemblyPath);
        }

        public void Cancel()
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

    }
}