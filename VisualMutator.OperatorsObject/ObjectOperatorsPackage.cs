namespace VisualMutator.MvcMutations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Text;

    using VisualMutator.Extensibility;
    using VisualMutator.OperatorsStandard;

    [PackageExport]
    public class ObjectOperatorsPackage : IOperatorsPackage
    {
        public ObjectOperatorsPackage()
        {
          ////  var catalog = new AssemblyCatalog(GetType().Assembly);
         //   var container = new CompositionContainer(catalog);
         //   container.ComposeParts(this);



            Operators = new[]
            {
                new EqualityOperatorChange(), 
            };

        }
     //   [ImportMany]
        public IEnumerable<IMutationOperator> Operators
        {
            get; 
            set;
        }

        public string Name
        {
            get
            {
                return "Standard";
            }
        }

        public string Description
        {
            get
            {
                return "Standard imperative operators.";
            }
        }
    }
}