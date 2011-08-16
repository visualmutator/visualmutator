namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    using System.Collections.Generic;

    public class MutationSession
    {

        public List<string> UsedOperators
        {
            get; set; }

        public string Name
        {
            get; set; }

        public List<string> MutatedTypes
        {
            get; set; 
        }

        public List<string> Assemblies
        {
            get;
            set;
        }

   
        
    }
}