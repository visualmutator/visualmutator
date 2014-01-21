namespace VisualMutator.Model.Mutations.Types
{
    #region

    using Microsoft.Cci;
    using MutantsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Paths;

    #endregion
    public class AssemblyNode : MutationNode
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

    public class TypeNamespaceNode : MutationNode
    {

        public TypeNamespaceNode(CheckedNode parent, string name)
            : base( name, true)
        {
            Parent = parent;
        }
        
    }


    public class TypeNode : MutationNode
    {

        public TypeNode(CheckedNode parent, string name)
            : base( name, true)
        {
            Parent = parent;
        }

      
    }

    public class MethodNode : MutationNode
    {
        private readonly IMethodDefinition _methodDefinition;

        public MethodNode(CheckedNode parent, string name, IMethodDefinition methodDefinition, bool hasChildren)
            : base(name, hasChildren)
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