namespace VisualMutator.Domain
{
    #region Usings

    using System.Collections.ObjectModel;

    using VisualMutator.Extensibility;

    #endregion

    public class OperatorPackage : ExtModel
    {
        private  ObservableCollection<MutationOperator> _operators;

        public OperatorPackage(IOperatorsPack operatorsPack)
        {
            throw new System.NotImplementedException();
        }

        public ObservableCollection<MutationOperator> Operators
        {
            set
            {
                if (_operators != value)
                {
                    _operators = value;
                    RaisePropertyChangedExt(() => Operators);
                }
            }
            get
            {
                return _operators;
            }
        }
    }
}