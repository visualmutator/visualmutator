namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using System;

    using Ninject;
    using Ninject.Syntax;

    using VisualMutator.Infrastructure.Factories;

    #endregion

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

    public static class Utility
    {

        public static void InjectFuncFactory<T>(this IKernel kernel, Func<T> func)
        {
            kernel.Bind<IFactory<T>>().ToConstant(new FuncFactory<T>(func));
        }

        public static void AndFromFactory<T>(this IBindingWhenInNamedWithOrOnSyntax<T> binding)
        {

            binding.Kernel.Bind<IFactory<T>>().ToConstant(new NinjectFactory<T>(binding.Kernel));
        }
    }
}