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
        private readonly IOperatorsPackage _operatorsPackage;


        public PackageNode(FakeOperatorPackageRootNode root, IOperatorsPackage operatorsPackage)
            : base(root, operatorsPackage.Name)
        {
            _operatorsPackage = operatorsPackage;


        }

        public IOperatorsPackage OperatorsPackage
        {
            get
            {
                return _operatorsPackage;
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
            : base(parent,mutationOperator.Identificator+" - "+ mutationOperator.Name)
        {
            Operator = mutationOperator;
         
        }

        public IMutationOperator Operator { get; set; }
    }
}