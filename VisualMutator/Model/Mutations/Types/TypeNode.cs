namespace VisualMutator.Model.Mutations.Types
{
    #region Usings

    using CommonUtilityInfrastructure.Paths;
    using Microsoft.Cci;
    using Mono.Cecil;

    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.CheckboxedTree;

    #endregion
    public class AssemblyNode : NormalNode
    {
        private IModule _assemblyDefinition;


        public AssemblyNode(string name, IModule assemblyDefinition)
            : base( name, true)
        {
            _assemblyDefinition = assemblyDefinition;
        }

        public IModule AssemblyDefinition
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
        private readonly INamespaceTypeDefinition _typeDefinition;

        public TypeNode(NormalNode parent, string name, INamespaceTypeDefinition typeDefinition)
            : base( name, false)
        {
            _typeDefinition = typeDefinition;
            Parent = parent;
        }

        public INamespaceTypeDefinition TypeDefinition
        {
            get
            {
                return _typeDefinition;
            }
        }
    }



}