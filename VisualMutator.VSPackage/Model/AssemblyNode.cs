namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Domain;

    public class AssemblyNode : TreeElement
    {

        private string _fullPath;

        public AssemblyNode(string name, string fullPath)
        {
            _fullPath = fullPath;
      

            Types = new ObservableCollection<TypeNode>();

            Name = name;
            IsIncluded = true;

        }

        public string FullPath
        {
            get
            {
                return _fullPath;
            }
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