namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using CommonUtilityInfrastructure;

    using ICSharpCode.Decompiler;

    using Mono.Cecil;

    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations.Structure;

    using log4net;

    public enum CodeLanguage
    {
        CSharp,
        IL
    }

   
 
}