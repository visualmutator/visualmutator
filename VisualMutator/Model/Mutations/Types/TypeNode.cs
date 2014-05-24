namespace VisualMutator.Model.Mutations.Types
{
    #region

    using System.Text;
    using Microsoft.Cci;
    using MutantsTree;
    using StoringMutants;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    #endregion
    public class AssemblyNode : MutationNode
    {
        private readonly IModuleInfo _assemblyDefinition;


        public AssemblyNode(string name, IModuleInfo assemblyDefinition)
            : base( name, true)
        {
            _assemblyDefinition = assemblyDefinition;
        }

        public IModuleInfo AssemblyDefinition
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


        public string Namespace
        {
            get
            {
                var sb = new StringBuilder();
                MutationNode node = this;
                while (node.Parent != null)
                {
                    sb.Append(node.Name);
                    node = (MutationNode) node.Parent;
                }
                return sb.ToString();
            }
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