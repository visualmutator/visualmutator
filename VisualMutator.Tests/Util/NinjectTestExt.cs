namespace VisualMutator.Tests.Util
{
    using System;
    using Moq;
    using Ninject;
    using VisualMutator.Infrastructure;

    public static class NinjectExt
    {
        public static void BindMock<TMock>(this KernelBase kernel, Action<Mock<TMock>> mockSetup) where TMock : class
        {
            var mock = new Mock<TMock>();
            mockSetup(mock);
            kernel.Bind<TMock>().ToConstant(mock.Object);
        }

        public static T GetFromFactory<T>(this KernelBase kernel, params object[] pars)
        {
            return kernel.Get<IBindingFactory<T>>().CreateWithParams(pars);
        }
    }
}