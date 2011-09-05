namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    public class MutationSession
    {
        public List<string> UsedOperators { get; set; }

        public string Name { get; set; }

        public List<string> MutatedTypes { get; set; }

        public List<string> Assemblies
        {
            get;
            set;
        }
    }
}