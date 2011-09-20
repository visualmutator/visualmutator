namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    using VisualMutator.Extensibility;

    #endregion

    public interface IOperatorsManager
    {
        BetterObservableCollection<PackageNode> OperatorPackages { get; set; }

        void LoadOperators();

        IEnumerable<IMutationOperator> GetActiveOperators();
    }

    public class OperatorsManager : IOperatorsManager
    {
        private readonly IOperatorLoader _loader;

        
        public OperatorsManager(IOperatorLoader loader)
        {
            _loader = loader;
            OperatorPackages = new BetterObservableCollection<PackageNode>();
        }

        public BetterObservableCollection<PackageNode> OperatorPackages { get; set; }

        public void LoadOperators()
        {
            OperatorPackages.Clear();

            var root = new FakeOperatorPackageRootNode("Root");

            IEnumerable<IOperatorsPack> packages = _loader.ReloadOperators();
            foreach (IOperatorsPack operatorsPack in packages)
            {
                var package = new PackageNode(root,operatorsPack);
                foreach (IMutationOperator mutationOperator in operatorsPack.Operators)
                {
                    var operatorNode = new OperatorNode(package,mutationOperator);
                    package.Operators.Add(operatorNode);
                }
                root.Children.Add(package);
                OperatorPackages.Add(package);
            }
            root.IsIncluded = true;
        }

        public IEnumerable<IMutationOperator> GetActiveOperators()
        {
            return OperatorPackages.SelectMany(pack => pack.Operators)
                .Where(oper => oper.IsLeafIncluded).Select(n=>n.Operator);
        }
    }
}