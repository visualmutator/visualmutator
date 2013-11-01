namespace VisualMutator.Model.StoringMutants
{
    #region

    using System.Collections.Generic;

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