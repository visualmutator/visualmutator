namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Windows;
    using log4net;
    using Microsoft.VisualStudio.Shell;
    using Model;
    using Ninject;
    using Ninject.Activation.Strategies;
    using Ninject.Modules;
    using NinjectModules;
    using VisualMutator.Controllers;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class VisualStudioPackageBootstrapper
    {



        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Bootstrapper _boot;

        public Bootstrapper Boot
        {
            get { return _boot; }
        }

        static VisualStudioPackageBootstrapper()
        {
            EnsureApplication();
        }
 

        public VisualStudioPackageBootstrapper(Package package)
        {
            _boot = new Bootstrapper(new List<INinjectModule>() {
                new InfrastructureModule(),
                new ViewsModule(),
                new VisualMutatorModule(),
                new VSNinjectModule(new VisualStudioConnection(package))});
        }

        public static void EnsureApplication()
        {
            if (Application.Current == null)
            {
                new Application();
            }
        }

        public object Shell
        {
            get
            {
                return _boot.Shell;
            }
        }


        public void Initialize()
        {
            _boot.Initialize();
        }
    }
}