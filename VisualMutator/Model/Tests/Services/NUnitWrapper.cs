namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using log4net;
    using NUnit.Core;
    using NUnit.Util;

    #endregion

    public interface INUnitWrapper
    {
        TestFilter NameFilter { get; }
        ITest LoadTests(IEnumerable<string> assemblies);
        void UnloadProject();
    }

    public class NUnitWrapper : INUnitWrapper
    {


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

       
        public void UnloadProject()
        {
        }

     

    }
}