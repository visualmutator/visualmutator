namespace VisualMutator.MvcMutations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Text;

    using VisualMutator.Extensibility;
    using VisualMutator.OperatorsObject.Operators;

    [PackageExport]
    public class ObjectOperatorsPackage : IOperatorsPackage
    {
        public ObjectOperatorsPackage()
        {
 

            Operators = new[]
            {
                new EqualityOperatorChange(), 
            };

        }

        public IEnumerable<IMutationOperator> Operators
        {
            get; 
            set;
        }

        public string Name
        {
            get
            {
                return "Object";
            }
        }

        public string Description
        {
            get
            {
                return "Object oriented Operators.";
            }
        }
    }
}