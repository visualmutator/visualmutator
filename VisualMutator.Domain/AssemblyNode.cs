namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Domain;

    public class AssemblyNode : ElementToMutate
    {
        private readonly AssemblyDefinition _assembly;

        public AssemblyNode(AssemblyDefinition assembly)
        {
            _assembly = assembly;

            Types = new ObservableCollection<TypeNode>();

            Name = assembly.Name.Name;
            Included = true;

        }

      //  ObservableCollection<TypeNode> Types 

        private ObservableCollection<TypeNode> _types;

        public ObservableCollection<TypeNode> Types
        {
            set
            {
                _types = value;
                this.RaisePropertyChangedExt(() => Types);
            }
            get
            {
                return _types;
            }
        }
    }
}