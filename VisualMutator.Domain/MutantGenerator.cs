namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IMutantGenerator
    {
        IOperatorsManager OperatorsManager { get; }
        void GenerateMutants();
    }

    public class MutantGenerator : IMutantGenerator
    {
        private readonly IOperatorsManager _operatorsManager;

        public IOperatorsManager OperatorsManager
        {
            get
            {
                return _operatorsManager;
            }
        }

        public MutantGenerator(IOperatorsManager operatorsManager)
        {
            _operatorsManager = operatorsManager;
        }

        public void GenerateMutants()
        {
            _operatorsManager.ApplyOperators();
        }

    }
}