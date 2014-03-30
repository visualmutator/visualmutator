namespace VisualMutator.Model.StoringMutants
{
    #region

    using System;
    using System.Collections.Generic;
    using Infrastructure;

    #endregion

    public class StoredMutantInfo : IDisposable
    {
        private readonly ProjectFilesClone _filesClone;
        public string Directory { get; set; }

        private readonly List<string> _assembliesPaths;

   

        public List<string> AssembliesPaths
        {
            get
            {
                return _assembliesPaths;
            }
        }

        public StoredMutantInfo(ProjectFilesClone filesClone)
        {
            _filesClone = filesClone;
            Directory = _filesClone.ParentPath.ToString();

            _assembliesPaths = new List<string>();
        }

        public void Dispose()
        {
            _filesClone.Dispose();
        }
    }
}