namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using Ninject;

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
}