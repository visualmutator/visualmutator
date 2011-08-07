namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Domain;

    public class TypeNode : TreeElement
    {
        private string _fullName;

        public string FullName
        {
            get
            {
                return _fullName;
            }
        }

        public TypeNode(string fullName, string name)
        {
            _fullName = fullName;

            Name = name;
            IsIncluded = true;
        }
    }
}