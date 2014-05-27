namespace VisualMutator.Infrastructure
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Ninject;
    using Ninject.Activation;
    using Ninject.Activation.Strategies;
    using Ninject.Extensions.ChildKernel;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;
    using NinjectModules;
    using UsefulTools.DependencyInjection;

    #endregion
    public interface IBindingFactory<out TObject> : IFactory<TObject>
    {
        TObject CreateWithBindings<T1>(T1 param1);
        TObject CreateWithBindings<T1, T2>(T1 param1, T2 param2);
        TObject CreateWithBindings<T1, T2, T3>(T1 param1, T2 param2, T3 param3);
    }

    public interface IRootFactory<out TObject>
    {
        IObjectRoot<TObject> Create();
        IObjectRoot<TObject> CreateWithParams(params object[] parameters);
        IObjectRoot<TObject> CreateWithBindings<T1>(T1 param1);
        IObjectRoot<TObject> CreateWithBindings<T1, T2>(T1 param1, T2 param2);
        IObjectRoot<TObject> CreateWithBindings<T1, T2, T3>(T1 param1, T2 param2, T3 param3);
    }
    class Uti
    {
        internal static TObject CreateWithParams<TObject>(IKernel kernel, params object[] parameters)
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
            return Uti.CreateWithParams<TObject>(_kernel, param1);
        }
        public virtual TObject CreateWithBindings<T1, T2>(T1 param1, T2 param2)
        {
            _kernel.Bind<T1>().ToConstant(param1);
            _kernel.Bind<T2>().ToConstant(param2);
            return Uti.CreateWithParams<TObject>(_kernel, param1, param2);
        }
        public virtual TObject CreateWithBindings<T1, T2, T3>(T1 param1, T2 param2, T3 param3)
        {
            _kernel.Bind<T1>().ToConstant(param1);
            _kernel.Bind<T2>().ToConstant(param2);
            _kernel.Bind<T3>().ToConstant(param3);
            return Uti.CreateWithParams<TObject>(_kernel, param1, param2, param3);
        }

        public virtual TObject CreateWithParams(params object[] parameters)
        {
            return Uti.CreateWithParams<TObject>(_kernel, parameters);
        }

       
    }
    public class NinjectChildFactory<TObject> : IRootFactory<TObject>
    {
        private readonly IKernel _kernel;
        private readonly Action<IKernel> _childBindingAction;
        private Action<IKernel> bindAction;

        public NinjectChildFactory(IKernel kernel, Action<IKernel> childBindings) 
        {
            _kernel = kernel;
            _childBindingAction = childBindings;
            bindAction = k => k.Bind<TObject>().ToSelf().InSingletonScope();
        }
        public IObjectRoot<TObject> CreateWithBindings<T1>(T1 param1)
        {
            IKernel child = CreateChildKernel();
            child.Bind<T1>().ToConstant(param1);
            return CreateInternal(child);
        }
        public IObjectRoot<TObject> CreateWithBindings<T1, T2>(T1 param1, T2 param2)
        {
            IKernel child = CreateChildKernel();
            child.Bind<T1>().ToConstant(param1);
            child.Bind<T2>().ToConstant(param2);
            return CreateInternal(child);
        }


        public IObjectRoot<TObject> CreateWithBindings<T1, T2, T3>(T1 param1, T2 param2, T3 param3)
        {
            IKernel child = CreateChildKernel();
            child.Bind<T1>().ToConstant(param1);
            child.Bind<T2>().ToConstant(param2);
            child.Bind<T3>().ToConstant(param3);
            return CreateInternal(child);
        }

        private IObjectRoot<TObject> CreateInternal(IKernel childKernel)
        {
            var obj = childKernel.Get<TObject>();
            return new ObjectRoot<TObject>(childKernel, obj);
        }

        public IObjectRoot<TObject> Create()
        {
            return CreateInternal(CreateChildKernel());
        }
     
        public IObjectRoot<TObject> CreateWithParams(params object[] parameters)
        {
            IKernel childKernel = CreateChildKernel();
            var obj = Uti.CreateWithParams<TObject>(childKernel, parameters);
            return new ObjectRoot<TObject>(childKernel, obj);
        }

        private IKernel CreateChildKernel()
        {
            var childKernel = new ChildKernel(_kernel);
            childKernel.Components.Add<IActivationStrategy, MyMonitorActivationStrategy>();
            bindAction(childKernel);
            _childBindingAction(childKernel);
            return childKernel;
        }

        public NinjectChildFactory<TObject> AndBindToInterface<T, TObject2>() where TObject2 : T
        {
            bindAction = kernel => kernel.Bind<T>().To<TObject2>().InSingletonScope();
            return this;
        }
    }

    public interface IObjectRoot<out TObject>
    {
        TObject Get
        {
            get;
        }
    }

    public class ObjectRoot<TObject> : IObjectRoot<TObject>, IDisposable
    {
        private IKernel _kernel; //only for keeping
        private readonly TObject _obj;

        public IKernel Kernel
        {
            get { return _kernel; }
        }

        public ObjectRoot(IKernel kernel, TObject obj)
        {
            _kernel = kernel;
            _obj = obj;
            Trace.WriteLine("Created ObjectRoot with kernel: " + _kernel.ToString());
        }

        public TObject Get
        {
           get
           {
               return _obj;
           }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_kernel != null)
            {
                _kernel.Dispose();
                _kernel = null;
            }
        }

        // Required to properly dispose child kernel
        ~ObjectRoot()
        {
            Dispose(false);
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
        public static GoOnBinding<T> BindObjectRoot<T>(this IKernel kernel)
        {
            return new GoOnBinding<T>(kernel);
        }
        public class GoOnBinding<T>
        {
            private readonly IKernel _kernel;

            public GoOnBinding(IKernel kernel)
            {
                _kernel = kernel;
            }

            public void To<C>(Action<IKernel> childBindings) where C : T
            {
                _kernel.Bind<IRootFactory<T>>().ToConstant(
                    new NinjectChildFactory<T>(_kernel, childBindings).AndBindToInterface<T,C>());
            }
            public void ToSelf(Action<IKernel> childBindings)
            {
                _kernel.Bind<IRootFactory<T>>().ToConstant(
                    new NinjectChildFactory<T>(_kernel, childBindings));
            }
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