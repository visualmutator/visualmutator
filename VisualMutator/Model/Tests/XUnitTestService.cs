namespace VisualMutator.Model.Tests
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CoverageFinder;
    using log4net;
    using Microsoft.Cci;
    using Services;
    using Strilanc.Value;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;

    public class XUnitTestService : ITestsService
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISettingsManager _settingsManager;
        private readonly IFactory<XUnitTestsRunContext> _contextFactory;

        public XUnitTestService(
            ISettingsManager settingsManager,
            IFactory<XUnitTestsRunContext> contextFactory)
        {
            _settingsManager = settingsManager;
            _contextFactory = contextFactory;
            XUnitConsolePath = FindConsolePath();
        }

        public string FrameWorkName { get { return "XUnit"; } }
        public string XUnitConsolePath { get; private set; }

        private string FindConsolePath()
        {
            var xUnitDirPath = _settingsManager["XUnitConsoleDirPath"];
            var xUnitConsolePath = Path.Combine(xUnitDirPath, "xunit.console.exe");

            if (!File.Exists(xUnitConsolePath))
            {
                throw new FileNotFoundException(xUnitConsolePath + " file was not found.");
            }
            return xUnitConsolePath;
        }

        public May<TestsLoadContext> LoadTests(string assemblyPath)
        {
            _log.Info("XUnit loading tests...");
            var cci = new CciModuleSource(assemblyPath);

            var visitor = new XUnitTestsVisitor();
            var traverser = new CodeTraverser
            {
                PreorderVisitor = visitor
            };

            traverser.Traverse(cci.Module.Module);

            var classes = visitor.Classes.Where(c => c.Children.Count != 0).ToList();
            if(classes.Count != 0)
            {
                _log.Info("Tests loaded ("+ classes.Count + " classes).");
                return new May<TestsLoadContext>(new TestsLoadContext(FrameWorkName, classes));
            }
            else
            {
                _log.Info("No tests found.");
                return May.NoValue;
            }
        }

        public void Cancel()
        {
           
        }

        public ITestsRunContext CreateRunContext(TestsLoadContext loadContext, string mutatedPath)
        {
            return _contextFactory.CreateWithParams(loadContext, mutatedPath);
        }
    }
}