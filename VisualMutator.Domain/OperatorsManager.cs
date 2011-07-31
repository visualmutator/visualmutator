namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    public interface IOperatorsManager
    {
        ObservableCollection<OperatorPackage> OperatorPackages { get; set; }

        void LoadOperators();

        void ApplyOperators(IEnumerable<TypeDefinition> types);
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


        public void ApplyOperators(IEnumerable<TypeDefinition> types)
        {
            foreach (var mutOperator in OperatorPackages.SelectMany(pack => pack.Operators)
                .Where(oper => oper.IsEnabled))
            {
                mutOperator.Operator.Mutate(types);
            }
            
        }



    }
}