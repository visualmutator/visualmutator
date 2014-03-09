namespace VisualMutator.Infrastructure
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Ninject;
    using Ninject.Activation;
    using Ninject.Extensions.ChildKernel;
    using Ninject.Parameters;
    using Ninject.Syntax;
    using UsefulTools.DependencyInjection;

    #endregion
    public interface IBindingFactory<out TObject> : IFactory<TObject>
    {
        TObject CreateWithBindings<T1>(T1 param1);
        TObject CreateWithBindings<T1, T2>(T1 param1, T2 param2);
        TObject CreateWithBindings<T1, T2, T3>(T1 param1, T2 param2, T3 param3);
    }
    public class NinjectFactory<TObject> : IBindingFactory<TObject>
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

        public virtual TObject CreateWithBindings<T1>(T1 param1)
        {
            _kernel.Bind<T1>().ToConstant(param1);
            return CreateWithParams(_kernel, param1);
        }
        public virtual TObject CreateWithBindings<T1, T2>(T1 param1, T2 param2)
        {
            _kernel.Bind<T1>().ToConstant(param1);
            _kernel.Bind<T2>().ToConstant(param2);
            return CreateWithParams(_kernel, param1, param2);
        }
        public virtual TObject CreateWithBindings<T1, T2, T3>(T1 param1, T2 param2, T3 param3)
        {
            _kernel.Bind<T1>().ToConstant(param1);
            _kernel.Bind<T2>().ToConstant(param2);
            _kernel.Bind<T3>().ToConstant(param3);
            return CreateWithParams(_kernel, param1, param2, param3);
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
        public override TObject CreateWithBindings<T1>(T1 param1)
        {
            IKernel child = BindKernel();
            child.Bind<T1>().ToConstant(param1);
            return CreateWithParams(child);
        }
        public override TObject CreateWithBindings<T1, T2>(T1 param1, T2 param2)
        {
            IKernel child = BindKernel();
            child.Bind<T1>().ToConstant(param1);
            child.Bind<T2>().ToConstant(param2);
            return CreateWithParams(child);
        }
        public override TObject CreateWithBindings<T1, T2, T3>(T1 param1, T2 param2, T3 param3)
        {
            IKernel child = BindKernel();
            child.Bind<T1>().ToConstant(param1);
            child.Bind<T2>().ToConstant(param2);
            child.Bind<T3>().ToConstant(param3);
            return CreateWithParams(child);
        }
        public override TObject Create()
        {
            return BindKernel().Get<TObject>();
        }
        public override TObject CreateWithParams(params object[] parameters)
        {
            return CreateWithParams(BindKernel(), parameters);
        }
        private IKernel BindKernel()
        {
            var childKernel = new ChildKernel(_kernel);
            _childKernels.Add(childKernel);
            _childBindings(childKernel);
            return childKernel;
        }

    }
    public static class Utility
    {

        public static void InjectFuncFactory<T>(this IKernel kernel, Func<T> func)
        {
            kernel.Bind<IFactory<T>>().ToConstant(new FuncFactory<T>(func));
        }

        public static IBindingWhenInNamedWithOrOnSyntax<T> AndFromFactory<T>(
            this IBindingWhenInNamedWithOrOnSyntax<T> binding)
        {

            binding.Kernel.Bind<IBindingFactory<T>>().ToProvider(new FactoryProvider<T>(binding.Kernel));
            binding.Kernel.Bind<IFactory<T>>().ToProvider(new FactoryProvider<T>(binding.Kernel));
            return binding;
        }
        public static void InjectChildFactory<T>(this IKernel kernel, Action<IKernel> childBindings)
        {

            kernel.Bind<IFactory<T>>().ToConstant(new NinjectChildFactory<T>(kernel, childBindings));
            kernel.Bind<IBindingFactory<T>>().ToConstant(new NinjectChildFactory<T>(kernel, childBindings));
         
        }
    }

    public class FactoryProvider<T> : IProvider<IBindingFactory<T>>
    {
        private readonly IKernel _kernel;

        public FactoryProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object Create(IContext context)
        {
            return new NinjectFactory<T>(_kernel);
        }

        public Type Type
        {
            get
            {
                return typeof(IBindingFactory<T>);
            } 
        }
    }


}