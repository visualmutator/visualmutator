namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using System;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.DependencyInjection;

    using Ninject;
    using Ninject.Parameters;
    using Ninject.Syntax;

    #endregion

    public class NinjectFactory<TObject> : IFactory<TObject>
    {
        private readonly IKernel _kernel;

        public NinjectFactory(IKernel kernel)
        {
            _kernel = kernel;
            //   _kernel.CanResolve(_kernel.CreateRequest())
        }

       
        public TObject Create(params object[] parameters)
        {
            if(parameters.Length == 0)
            {
                return _kernel.Get<TObject>();
            }
            else
            {
                var constr = typeof (TObject).GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
                var pars = constr.GetParameters()
                    .Skip(constr.GetParameters().Length - parameters.Length)
                    .Select(param => param.Name);
                var pa = pars.Zip(parameters, (name, obj) => new ConstructorArgument(name, obj));
                return _kernel.Get<TObject>(pa.Cast<IParameter>().ToArray());
            }
            
        }
    }

    public static class Utility
    {

        public static void InjectFuncFactory<T>(this IKernel kernel, Func<T> func)
        {
            kernel.Bind<IFactory<T>>().ToConstant(new FuncFactory<T>(func));
        }

        public static IBindingWhenInNamedWithOrOnSyntax<T> AndFromFactory<T>(this IBindingWhenInNamedWithOrOnSyntax<T> binding)
        {

            binding.Kernel.Bind<IFactory<T>>().ToConstant(new NinjectFactory<T>(binding.Kernel));
            return binding;
        }
    }
}