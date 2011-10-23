namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;

    using VisualMutator.Extensibility;

    #endregion

    public class StoredMutantInfo
    {

        private readonly List<string> _assembliesPaths;

   

        public List<string> AssembliesPaths
        {
            get
            {
                return _assembliesPaths;
            }
        }

        public StoredMutantInfo()
        {
     
            _assembliesPaths = new List<string>();
        }

        

    }
}