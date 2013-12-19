namespace VisualMutator.Model.Mutations.Operators
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;
    using UsefulTools.CheckboxedTree;

    #endregion
    public class FakeOperatorPackageRootNode : CheckedNode
    {
        public FakeOperatorPackageRootNode(string name)
            : base(name)
        {
        }
    }
    public class PackageNode : CheckedNode
    {
        private readonly IOperatorsPackage _operatorsPackage;


        public PackageNode(FakeOperatorPackageRootNode root, IOperatorsPackage operatorsPackage)
            : base( operatorsPackage.Name)
        {
            _operatorsPackage = operatorsPackage;
            Parent = root;

        }

        public IOperatorsPackage OperatorsPackage
        {
            get
            {
                return _operatorsPackage;
            }
        }

        public IEnumerable<OperatorNode> Operators
        {

            get
            {
                return Children.Cast<OperatorNode>();
            }
        }
    }
    public class OperatorNode : CheckedNode
    {
        public OperatorNode(PackageNode parent, IMutationOperator mutationOperator)
            : base(mutationOperator.Info.Id+" - "+ mutationOperator.Info.Name)
        {
            Operator = mutationOperator;
            Parent = parent;
        }

        public IMutationOperator Operator { get; set; }
    }
}