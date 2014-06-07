namespace VisualMutator.Model.Mutations.Types
{
    #region

    using System.Collections.Generic;
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
     


        public AssemblyNode(string name)
            : base( name, true)
        {
           
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

        public TypeNode(CheckedNode parent, string name, IEnumerable<MethodNode> children = null)
            : base( name, true)
        {
            Parent = parent;
            if(children != null)
            {
                Children.AddRange(children);
            }
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

        public MethodNode(string name, IMethodDefinition methodDefinition, IEnumerable<MutantGroup> children)
           : base(name, true)
        {
            _methodDefinition = methodDefinition;
            
            Children.AddRange(children);
            foreach (var child in Children)
            {
                child.Parent = this;
            }
        }

        //TODO: refactor and remove 
        public IMethodDefinition MethodDefinition
        {
            get { return _methodDefinition; }
        }

        
    }

}