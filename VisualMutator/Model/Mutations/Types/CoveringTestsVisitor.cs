namespace VisualMutator.Model.Mutations.Types
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CoverageFinder;
    using log4net;
    using Microsoft.Cci;
    using UsefulTools.ExtensionMethods;

    public class CoveringTestsVisitor : CodeVisitor
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly HashSet<MethodIdentifier> _foundTests;
        private readonly ICodePartsMatcher _searcher;
        private IMethodDefinition _currentTestMethod;
        public int ScannedMethods { get; private set; }
        public int ScannedMethodCalls { get; private set; }

        public CoveringTestsVisitor(ICodePartsMatcher constraints)
        {
            _foundTests = new HashSet<MethodIdentifier>();
            _searcher = constraints;
        }

        public override void Visit(IMethodDefinition method)
        {
            ScannedMethods++;
            _currentTestMethod = method.Attributes.Any(a =>
            {
                var attrType = a.Type as INamespaceTypeReference;
                return attrType != null && attrType.GetTypeFullName().IsIn("NUnit.Framework.TestAttribute", "Xunit.FactAttribute");
            }) ? method : null;

            if (_currentTestMethod != null && _searcher.Matches(_currentTestMethod))
            {
                _log.Debug("selected test method: " + _currentTestMethod);
                IsChoiceError = true;
            }
        }

        
        public override void Visit(IMethodCall methodCall)
        {
            base.Visit(methodCall);
            ScannedMethodCalls++;
            if (_currentTestMethod != null )
            {
                if (_searcher.Matches(methodCall.MethodToCall))
                {
                    _log.Debug("Adding test" + _currentTestMethod + " invoking method "
                        + methodCall.MethodToCall);
                    _foundTests.Add(CciMethodMatcher.CreateIdentifier(_currentTestMethod));
                }
            }
        }

        public HashSet<MethodIdentifier> FoundTests
        {
            get
            {
                return _foundTests;
            }
        }

        public bool IsChoiceError
        {
            get;
            set;
        }

       

    }
}