namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Domain;

    public class TypeNode : ElementToMutate
    {
        private readonly TypeDefinition _type;

        public TypeNode(TypeDefinition type)
        {
            _type = type;

            Name = type.Name;
            Included = true;
        }
    }
}