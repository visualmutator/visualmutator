namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;

    using VisualMutator.Extensibility;

    #endregion

    public class StoredMutantInfo
    {
        private readonly string _directoryPath;
        private readonly List<string> _assembliesPaths;

        public string DirectoryPath
        {
            get
            {
                return _directoryPath;
            }
        }

        public List<string> AssembliesPaths
        {
            get
            {
                return _assembliesPaths;
            }
        }

        public StoredMutantInfo(string directoryPath)
        {
            _directoryPath = directoryPath;
            _assembliesPaths = new List<string>();
        }

        

    }
}