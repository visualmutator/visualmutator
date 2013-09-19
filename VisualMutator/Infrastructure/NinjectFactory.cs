namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.DependencyInjection;
    using Mono.Collections.Generic;
    using Ninject;
    using Ninject.Activation;
    using Ninject.Extensions.ChildKernel;
    using Ninject.Parameters;
    using Ninject.Syntax;

    #endregion

    public class NinjectFactory<TObject> : IFactory<TObject>
    {
        protected readonly IKernel _kernel;

        public NinjectFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public virtual TObject Create()
        {
            return _kernel.Get<TObject>();
        }

        public virtual TObject CreateWithParams(params object[] parameters)
        {
            return CreateWithParams(_kernel, parameters);

        }

        protected TObject CreateWithParams(IKernel kernel, params object[] parameters)
        {
            if (parameters.Length == 0)
            {
                return kernel.Get<TObject>();
            }
            else
            {
                var constr = typeof(TObject).GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
                var pars = constr.GetParameters()
                    .Skip(constr.GetParameters().Length - parameters.Length)
                    .Select(param => param.Name);
                var pa = pars.Zip(parameters, (name, obj) => new ConstructorArgument(name, obj));
                return kernel.Get<TObject>(pa.Cast<IParameter>().ToArray());
            }
        }
    }
    public class NinjectChildFactory<TObject> : NinjectFactory<TObject>
    {
        private readonly Action<IKernel> _childBindings;
        private readonly ICollection<ChildKernel> _childKernels;

        public NinjectChildFactory(IKernel kernel, Action<IKernel> childBindings) : base(kernel)
        {
            _childBindings = childBindings;
            _childKernels = new Collection<ChildKernel>();
        }
        public override TObject Create()
        {
            var childKernel = new ChildKernel(_kernel);
            _childKernels.Add(childKernel);
            _childBindings(childKernel);
            return childKernel.Get<TObject>();
        }

        public override TObject CreateWithParams(params object[] parameters)
        {
            var childKernel = new ChildKernel(_kernel);
            _childKernels.Add(childKernel);
            _childBindings(childKernel);

            return CreateWithParams(childKernel, parameters);

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

            binding.Kernel.Bind<IFactory<T>>().ToProvider(new FactoryPrivider<T>(binding.Kernel));//ToConstant(new NinjectFactory<T>(binding.Kernel));
            return binding;
        }
        public static void InjectChildFactory<T>(this IKernel kernel, Action<IKernel> childBindings)
        {

            kernel.Bind<IFactory<T>>().ToConstant(new NinjectChildFactory<T>(kernel, childBindings));
         
        }
    }

    public class FactoryPrivider<T> : IProvider<IFactory<T>>
    {
        private readonly IKernel _kernel;

        public FactoryPrivider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object Create(IContext context)
        {
            return new NinjectFactory<T>(_kernel);
        }

        public Type Type
        {
            get { return typeof (IFactory<T>); } 
        }
    }


}