namespace VisualMutator.Model.Mutations.Types
{
    #region Usings

    using CommonUtilityInfrastructure.Paths;

    using Mono.Cecil;

    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.CheckboxedTree;

    #endregion
    public class AssemblyNode : NormalNode
    {
        private AssemblyDefinition _assemblyDefinition;
        

        public AssemblyNode(string name, AssemblyDefinition assemblyDefinition)
            : base( name, true)
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

        public FilePathAbsolute AssemblyPath
        {
            get;
            set;
        }
    }

    public class TypeNamespaceNode : NormalNode
    {

        public TypeNamespaceNode(NormalNode parent, string name)
            : base( name, true)
        {
            Parent = parent;
        }
        
    }


    public class TypeNode : NormalNode
    {
        private readonly TypeDefinition _typeDefinition;

        public TypeNode(NormalNode parent, string name, TypeDefinition typeDefinition)
            : base( name, false)
        {
            _typeDefinition = typeDefinition;
            Parent = parent;
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