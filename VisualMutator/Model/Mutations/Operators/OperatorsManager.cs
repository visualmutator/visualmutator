namespace VisualMutator.Model.Mutations.Operators
{
    #region

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Extensibility;

    #endregion

    public interface IOperatorsManager
    {

        OperatorPackagesRoot LoadOperators();
        Task<OperatorPackagesRoot> GetOperators();

    }

    public class OperatorsManager : IOperatorsManager
    {
        private readonly IOperatorLoader _loader;
        private Task<OperatorPackagesRoot> loadingTask;


        public OperatorsManager(IOperatorLoader loader)
        {
            _loader = loader;
           
        }

        public Task<OperatorPackagesRoot> GetOperators()
        {
            if(loadingTask == null)
            {
                loadingTask = Task.Run(() => LoadOperators());
            }
            return loadingTask;
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