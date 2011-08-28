namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System.Collections.Generic;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;

    #endregion
    public class AssemblyNode : RecursiveNode
    {

        public AssemblyNode(string name)
            : base(null, name)
        {

        }

    }
    public class TypeNamespaceNode : RecursiveNode
    {

        public TypeNamespaceNode(RecursiveNode parent, string name)
            : base(parent,name)
        {
           
        }
        
    }


    public class TypeNode : RecursiveNode
    {
        private readonly TypeDefinition _typeDefinition;

        public TypeNode(RecursiveNode parent, string name, TypeDefinition typeDefinition)
            : base(parent,name)
        {
            _typeDefinition = typeDefinition;
        }

        public TypeDefinition TypeDefinition
        {
            get
            {
                return _typeDefinition;
            }
        }
    }



}