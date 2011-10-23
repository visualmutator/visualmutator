namespace VisualMutator.Model.Mutations.Operators
{
    #region Usings

    using System.Collections.Generic;

    using VisualMutator.Extensibility;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.CheckboxedTree;

    #endregion
    public class FakeOperatorPackageRootNode : FakeRootNode<FakeOperatorPackageRootNode, PackageNode>
    {
        public FakeOperatorPackageRootNode(string name)
            : base(name)
        {
        }
    }
    public class PackageNode : NormalNode<FakeOperatorPackageRootNode, PackageNode, OperatorNode>
    {
        private readonly IOperatorsPack _operatorsPack;


        public PackageNode(FakeOperatorPackageRootNode root, IOperatorsPack operatorsPack)
            : base(root, operatorsPack.Name)
        {
            _operatorsPack = operatorsPack;


        }

        public IOperatorsPack OperatorsPack
        {
            get
            {
                return _operatorsPack;
            }
        }

        public IList<OperatorNode> Operators
        {

            get
            {
                return Children;
            }
        }
    }
    public class OperatorNode : LeafNode<PackageNode, OperatorNode>
    {
        public OperatorNode(PackageNode parent, IMutationOperator mutationOperator)
            : base(parent, mutationOperator.Name)
        {
            Operator = mutationOperator;
         
        }

        public IMutationOperator Operator { get; set; }
    }
}