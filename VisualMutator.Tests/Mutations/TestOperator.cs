namespace VisualMutator.Tests.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Extensibility;

    public class TestOperator : IMutationOperator
    {
        public string Identificator
        {
            get
            {
                return "T";
            }
        }

        public string Name
        {
            get
            {
                return "TestOperatorName";
            }
        }

        public string Description
        {
            get
            {
                return "TestOperatorDescription";
            }
        }

        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {
            yield return new MutationTarget().Add("0", types.Single(t => t.Name == "Type1").Methods.Single());
            yield return new MutationTarget().Add("0", types.Single(t => t.Name == "Type3").Methods.Single());
          

        }

        public void Mutate(MutationContext context)
        {
            context.Method("0").Name = "MutatedMethodName";


        }

    


    }
}