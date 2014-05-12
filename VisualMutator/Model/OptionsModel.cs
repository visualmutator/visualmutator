namespace VisualMutator.Model
{
    using System;
    using UsefulTools.Core;

    public class OptionsModel : ModelElement
    {
        public OptionsModel()
        {
            WhiteCacheThreadsCount = Environment.ProcessorCount + 1;
            MutantsCacheEnabled = true;
            ProcessingThreadsCount = Environment.ProcessorCount + 1;
            ForceNUnitDotNedVer = "";
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

        private int _whiteCacheThreadsCount;
        public int WhiteCacheThreadsCount
        {
            get
            {
                return _whiteCacheThreadsCount;
            }
            set
            {
                SetAndRise(ref _whiteCacheThreadsCount, value, () => WhiteCacheThreadsCount);
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

        private string _forceNUnitDotNedVer;
        public string ForceNUnitDotNedVer
        {
            get
            {
                return _forceNUnitDotNedVer;
            }
            set
            {
                SetAndRise(ref _forceNUnitDotNedVer, value, () => ForceNUnitDotNedVer);
            }
        }
        

    }
}