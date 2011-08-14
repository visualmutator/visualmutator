namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

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