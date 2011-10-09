namespace VisualMutator.Model.Mutations.Types
{
    #region Usings

    using Mono.Cecil;

    using VisualMutator.Infrastructure;

    #endregion
    public class AssemblyNode : GenericNode
    {
        private AssemblyDefinition _assemblyDefinition;
        

        public AssemblyNode(string name, AssemblyDefinition assemblyDefinition)
            : base(null, name, true)
        {
            _assemblyDefinition = assemblyDefinition;
        }

        public AssemblyDefinition AssemblyDefinition
        {
            get
            {
                return _assemblyDefinition;
            }
        }
    }

    public class TypeNamespaceNode : GenericNode
    {

        public TypeNamespaceNode(GenericNode parent, string name)
            : base(parent, name, true)
        {
           
        }
        
    }


    public class TypeNode : GenericNode
    {
        private readonly TypeDefinition _typeDefinition;

        public TypeNode(GenericNode parent, string name, TypeDefinition typeDefinition)
            : base(parent, name, false)
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