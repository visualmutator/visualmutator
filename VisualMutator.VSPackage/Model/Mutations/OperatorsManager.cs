namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;

    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    public interface IOperatorsManager
    {
        ObservableCollection<OperatorPackage> OperatorPackages { get; set; }

        void LoadOperators();

        IEnumerable<MutationOperator> GetActiveOperators();

    }

    public class OperatorsManager : IOperatorsManager
    {
        private readonly IOperatorLoader _loader;

        //ObservableCollection<OperatorsPackage> OperatorPackages

        public ObservableCollection<OperatorPackage> OperatorPackages
        {
            get;
            set;
        }

        public OperatorsManager(IOperatorLoader loader)
        {
            _loader = loader;
            OperatorPackages = new ObservableCollection<OperatorPackage>();
        }

        public void LoadOperators()
        {
            OperatorPackages.Clear();
            var packages = _loader.ReloadOperators();
            foreach (var operatorsPack in packages)
            {
                var package = new OperatorPackage(operatorsPack);
                foreach (var mutationOperator in operatorsPack.Operators)
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