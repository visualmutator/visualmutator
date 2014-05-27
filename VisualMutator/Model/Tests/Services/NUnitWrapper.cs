namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using log4net;
    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Framework;
    using NUnit.Util;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using UsefulTools.Threading;

    #endregion

    public interface INUnitWrapper
    {
        TestFilter NameFilter { get; }
        ITest LoadTests(IEnumerable<string> assemblies);
        void UnloadProject();
    }

    public class NUnitWrapper : INUnitWrapper
    {

        private readonly TestRunner _testRunner;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
     
        public TestFilter NameFilter
        {
            get { return null; }
        }

        public NUnitWrapper()
        {
            InternalTrace.Initialize("nunit-visual-mutator.log", InternalTraceLevel.Verbose);
         
            CoreExtensions.Host.InitializeService();
            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new RecentFilesService());
            ServiceManager.Services.AddService(new ProjectService());
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());

           
        }

       
        public ITest LoadTests(IEnumerable<string> assemblies)
        {

            try
            {
                var testRunner = new SimpleTestRunner();
                
                var enumerable = assemblies as IList<string> ?? assemblies.ToList();
                _log.Debug("Creating NUnit package for files " + string.Join(", ", enumerable));
                var package = new TestPackage("", enumerable.ToList());
                package.Settings["RuntimeFramework"] = new RuntimeFramework(RuntimeType.Net, Environment.Version);
                package.Settings["UseThreadedRunner"] = false;

//                lock (this)
//                {
                    _log.Debug("Loading NUnit package: " + package);
                    bool load = testRunner.Load(package);
                    if (!load)
                    {
                        throw new Exception("Tests load result: false.");
                    }
                    var t = testRunner.Test;
                    testRunner.Unload();
                    return t;
//                }
               
            }
            catch (Exception e)
            {
                throw new TestsLoadingException("Exception while loading tests.",e);
            }

        }

       
        public void UnloadProject()
        {
            _testRunner.Unload();
        }

     

    }
}