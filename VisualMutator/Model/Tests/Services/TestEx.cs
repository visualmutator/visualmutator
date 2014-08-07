namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using CoverageFinder;
    using Exceptions;
    using log4net;
    using NUnit.Core;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    #endregion
    public static class TestEx
    {
        public static IEnumerable<ITest> TestsEx(this ITest test)
        {
            return (test.Tests ?? new List<ITest>()).Cast<ITest>();
        }
    }
    
}