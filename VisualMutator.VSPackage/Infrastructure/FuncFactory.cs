namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FuncFactory<TObject> : IFactory<TObject>
    {
        private Func<TObject> _func;

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