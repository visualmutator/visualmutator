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
        BetterObservableCollection<OperatorPackage> OperatorPackages { get; set; }

        void LoadOperators();

        IEnumerable<MutationOperator> GetActiveOperators();
    }

    public class OperatorsManager : IOperatorsManager
    {
        private readonly IOperatorLoader _loader;

        //ObservableCollection<OperatorsPackage> OperatorPackages

        public OperatorsManager(IOperatorLoader loader)
        {
            _loader = loader;
            OperatorPackages = new BetterObservableCollection<OperatorPackage>();
        }

        public BetterObservableCollection<OperatorPackage> OperatorPackages { get; set; }

        public void LoadOperators()
        {
            OperatorPackages.Clear();
            IEnumerable<IOperatorsPack> packages = _loader.ReloadOperators();
            foreach (IOperatorsPack operatorsPack in packages)
            {
                var package = new OperatorPackage(operatorsPack);
                foreach (IMutationOperator mutationOperator in operatorsPack.Operators)
                {
                    var operatorNode = new MutationOperator(mutationOperator);
                    package.Operators.Add(operatorNode);
                }

                OperatorPackages.Add(package);
            }
        }

        public IEnumerable<MutationOperator> GetActiveOperators()
        {
            return OperatorPackages.SelectMany(pack => pack.Operators)
                .Where(oper => oper.IsIncluded);
        }
    }
}