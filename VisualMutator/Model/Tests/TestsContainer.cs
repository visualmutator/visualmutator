namespace VisualMutator.Model.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Exceptions;
    using Infrastructure;
    using LinqLib.Operators;
    using log4net;
    using Mutations;
    using Mutations.MutantsTree;
    using Services;
    using StoringMutants;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using Verification;

    #endregion

    public interface ITestsContainer
    {
        void CancelAllTesting();

        IEnumerable<TestNodeNamespace> CreateMutantTestTree(Mutant mutant);
    }

    public class TestsContainer : ITestsContainer
    {


        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly TestResultTreeCreator _testResultTreeCreator;

        public TestsContainer()
        {
            _testResultTreeCreator = new TestResultTreeCreator();
        }
       



        public void CancelAllTesting()
        {
        }

       

        public IEnumerable<TestNodeNamespace> CreateMutantTestTree(Mutant mutant)
        {
            List<TmpTestNodeMethod> nodeMethods = mutant.TestRunContexts
                .SelectMany(c => c.TestResults.ResultMethods).ToList();

            return _testResultTreeCreator.CreateMutantTestTree(nodeMethods);
        }
    }
}