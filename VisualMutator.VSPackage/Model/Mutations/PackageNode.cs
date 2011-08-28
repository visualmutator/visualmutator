namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using VisualMutator.Extensibility;

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
}