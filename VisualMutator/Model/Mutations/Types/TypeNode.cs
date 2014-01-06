namespace VisualMutator.Model.Mutations.Types
{
    #region

    using Microsoft.Cci;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Paths;

    #endregion
    public class AssemblyNode : CheckedNode
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

    public class TypeNamespaceNode : CheckedNode
    {

        public TypeNamespaceNode(CheckedNode parent, string name)
            : base( name, true)
        {
            Parent = parent;
        }
        
    }


    public class TypeNode : CheckedNode
    {
        private readonly INamespaceTypeDefinition _typeDefinition;

        public TypeNode(CheckedNode parent, string name, INamespaceTypeDefinition typeDefinition)
            : base( name)
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

    public class MethodNode : CheckedNode
    {
        private readonly IMethodDefinition _methodDefinition;
        private readonly INamespaceTypeDefinition _typeDefinition;

        public MethodNode(CheckedNode parent, string name, IMethodDefinition methodDefinition)
            : base(name, hasChildren: false)
        {
            _methodDefinition = methodDefinition;
            Parent = parent;
        }

        public IMethodDefinition MethodDefinition
        {
            get { return _methodDefinition; }
        }
    }

}