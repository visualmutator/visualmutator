namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StaticFactory<T> : IFactory<T>
    {

        public T Create()
        {
            throw new NotImplementedException();
        }
    }
}