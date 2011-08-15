namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Ninject;
    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Injection;
    using Ninject.Modules;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;
    using Ninject.Syntax;


    public class NinjectFactory<TObject> : IFactory<TObject>
    {
        private readonly IKernel _kernel;

        public NinjectFactory(IKernel kernel)
        {
            _kernel = kernel;
         //   _kernel.CanResolve(_kernel.CreateRequest())
        }

        public TObject Create()
        {
            return _kernel.Get<TObject>();
        }

    }
}