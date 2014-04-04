namespace VisualMutator.Model.Mutations.Operators
{
    #region

    using System.Collections.Generic;
    using Extensibility;

    #endregion

    public interface IOperatorsManager
    {

        OperatorPackagesRoot LoadOperators();


    }

    public class OperatorsManager : IOperatorsManager
    {
        private readonly IOperatorLoader _loader;

        
        public OperatorsManager(IOperatorLoader loader)
        {
            _loader = loader;
           
        }


        public OperatorPackagesRoot LoadOperators()
        {
            var list = new List<PackageNode>();

            var root = new OperatorPackagesRoot("Root");

            IEnumerable<IOperatorsPackage> packages = _loader.ReloadOperators();
            foreach (IOperatorsPackage operatorsPack in packages)
            {
                var package = new PackageNode(root,operatorsPack);
                foreach (IMutationOperator mutationOperator in operatorsPack.Operators)
                {
                    var operatorNode = new OperatorNode(package,mutationOperator);
                    package.Children.Add(operatorNode);
                }
                root.Children.Add(package);
                list.Add(package);
            }
            root.IsIncluded = true;

            return root;
        }

  
    }
}