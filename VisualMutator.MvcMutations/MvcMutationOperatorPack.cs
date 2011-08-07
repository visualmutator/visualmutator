namespace VisualMutator.MvcMutations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Text;

    using VisualMutator.Extensibility;


    [Export(typeof(IOperatorsPack))]
    public class MvcMutationOperatorPack : IOperatorsPack
    {
        public MvcMutationOperatorPack()
        {
            var catalog = new AssemblyCatalog(GetType().Assembly);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
        [ImportMany]
        public IEnumerable<IMutationOperator> Operators
        {
            get; set;
        }
    }
}