namespace VisualMutator.Model
{
    using UsefulTools.Core;

    public class MutantsCreationOptions : ModelElement
    {
        public MutantsCreationOptions()
        {
            AdditionalFilesToCopy = new NotifyingCollection<string>();
        }
        private bool _createMoreMutants;

        public bool CreateMoreMutants
        {
            get
            {
                return _createMoreMutants;
            }
            set
            {
                SetAndRise(ref _createMoreMutants, value, () => CreateMoreMutants);
            }
        }

        private NotifyingCollection<string> _additionalFilesToCopy;

        public NotifyingCollection<string> AdditionalFilesToCopy
        {
            get
            {
                return _additionalFilesToCopy;
            }
            set
            {
                SetAndRise(ref _additionalFilesToCopy, value, () => AdditionalFilesToCopy);
            }
        }

        private bool _isMutantVerificationEnabled;
        

        public bool IsMutantVerificationEnabled
        {
            get
            {
                return _isMutantVerificationEnabled;
            }
            set
            {
                SetAndRise(ref _isMutantVerificationEnabled, value, () => IsMutantVerificationEnabled);
            }
        }

        private int _maxNumerOfMutantPerOperator;
        public int MaxNumerOfMutantPerOperator
        {
            get { return _maxNumerOfMutantPerOperator; }
            set
            {
                SetAndRise(ref _maxNumerOfMutantPerOperator, value, () => MaxNumerOfMutantPerOperator);
            }
        }
    }
}