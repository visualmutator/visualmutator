namespace VisualMutator.Model.Mutations.Types
{
    #region Usings

    using Mono.Cecil;

    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.CheckboxedTree;

    #endregion
    public class AssemblyNode : NormalNode
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

    public class TypeNamespaceNode : NormalNode
    {

        public TypeNamespaceNode(NormalNode parent, string name)
            : base(parent, name, true)
        {
           
        }
        
    }


    public class TypeNode : NormalNode
    {
        private readonly TypeDefinition _typeDefinition;

        public TypeNode(NormalNode parent, string name, TypeDefinition typeDefinition)
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