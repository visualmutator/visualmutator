namespace VisualMutator.Infrastructure.Factories
{
    #region Usings

    using System;

    #endregion

    public class FuncFactory<TObject> : IFactory<TObject>
    {
        private readonly Func<TObject> _func;

        public FuncFactory(Func<TObject> func)
        {
            _func = func;
        }

        public TObject Create()
        {
            return _func();
        }
    }
}