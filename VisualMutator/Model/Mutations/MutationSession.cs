namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;

    using VisualMutator.Extensibility;

    #endregion

    public class MutationSession
    {
    

        public string Name { get; set; }

        public List<string> Assemblies
        {
            get;
            set;
        }

        public DateTime DateOfCreation { get; set; }


    }
}