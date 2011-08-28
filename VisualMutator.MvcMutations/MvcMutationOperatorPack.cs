namespace VisualMutator.MvcMutations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Text;

    using VisualMutator.Extensibility;


    [PackageExport]
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

        public string Name
        {
            get
            {
                return "ASP.NET MVC3 Operators Package";
            }
        }

        public string Description
        {
            get
            {
                return "Operators specific to ASP.NET MVC3 web framework.";
            }
        }
    }
}