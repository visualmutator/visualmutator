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
    public class StandardOperatorsPackage : IOperatorsPackage
    {
        public StandardOperatorsPackage()
        {
          ////  var catalog = new AssemblyCatalog(GetType().Assembly);
         //   var container = new CompositionContainer(catalog);
         //   container.ComposeParts(this);



            Operators = new[]
            {
                new ChangeAdditionIntoSubstraction(), 
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