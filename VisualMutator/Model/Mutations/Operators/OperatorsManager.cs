namespace VisualMutator.Model.Mutations.Operators
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Extensibility;

    #endregion

    public interface IOperatorsManager
    {

        IList<PackageNode> LoadOperators();


    }

    public class OperatorsManager : IOperatorsManager
    {
        private readonly IOperatorLoader _loader;

        
        public OperatorsManager(IOperatorLoader loader)
        {
            _loader = loader;
           
        }

      
        public IList<PackageNode> LoadOperators()
        {
            var list = new List<PackageNode>();

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
                list.Add(package);
            }
            root.IsIncluded = true;

            return list;
        }

        //public IEnumerable<IMutationOperator> GetActiveOperators()
       // {
           // return OperatorPackages.SelectMany(pack => pack.Operators)
          //      .Where(oper => oper.IsLeafIncluded).Select(n=>n.Operator);
       // }
    }
}