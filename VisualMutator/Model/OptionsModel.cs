namespace VisualMutator.Model
{
    using System;
    using UsefulTools.Core;

    public class OptionsModel : ModelElement
    {
        public OptionsModel()
        {
            WhiteCacheEnabled = true;
            MutantsCacheEnabled = true;
            ProcessingThreadsCount = Environment.ProcessorCount + 1;
        }

        private bool _mutantsCacheEnabled;
        public bool MutantsCacheEnabled
        {
            get
            {
                return _mutantsCacheEnabled;
            }
            set
            {
                SetAndRise(ref _mutantsCacheEnabled, value, () => MutantsCacheEnabled);
            }
        }

        private bool _whiteCacheEnabled;
        public bool WhiteCacheEnabled
        {
            get
            {
                return _whiteCacheEnabled;
            }
            set
            {
                SetAndRise(ref _whiteCacheEnabled, value, () => WhiteCacheEnabled);
            }
        }

        private int _processingThreadsCount;
        public int ProcessingThreadsCount
        {
            get
            {
                return _processingThreadsCount;
            }
            set
            {
                SetAndRise(ref _processingThreadsCount, value, () => ProcessingThreadsCount);
            }
        }
    }
}