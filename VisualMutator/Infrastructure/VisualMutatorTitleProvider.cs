namespace VisualMutator.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    public class VisualMutatorTitleProvider : IApplicationTitleProvider
    {
        public VisualMutatorTitleProvider()
        {
            
        }

        public string GetTitle()
        {
            return "Visual Mutator";
        }
    }
}