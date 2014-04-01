namespace VisualMutator.Model.Mutations.Types
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.Immutable;
    using NUnit.Core;
    using UsefulTools.ExtensionMethods;
    using GenericTypeInstanceReference = Microsoft.Cci.MutableCodeModel.GenericTypeInstanceReference;
    using TypeHelper = Microsoft.Cci.TypeHelper;

    public class CoveringTestsVisitor : CodeVisitor
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly MethodIdentifier _constraints;
        private readonly HashSet<MethodIdentifier> _foundTests;
        private readonly CciMethodSearcher _searcher;
        private IMethodDefinition _currentTestMethod;

        public CoveringTestsVisitor(MethodIdentifier constraints)
        {
            _constraints = constraints;
            _foundTests = new HashSet<MethodIdentifier>();
            _searcher = new CciMethodSearcher(_constraints);
        }

        public override void Visit(IMethodDefinition method)
        {
            _currentTestMethod = method.Attributes.Any(a =>
            {
                var attrType = a.Type as INamespaceTypeReference;
                return attrType != null && attrType.GetTypeFullName() == "NUnit.Framework.TestAttribute";
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
            
            if (_currentTestMethod != null )
            {
                if (_searcher.Matches(methodCall.MethodToCall))
                {
                    _log.Debug("Adding test" + _currentTestMethod + " invoking method "
                        + _constraints);
                    _foundTests.Add(CciMethodSearcher.CreateIdentifier(_currentTestMethod));
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