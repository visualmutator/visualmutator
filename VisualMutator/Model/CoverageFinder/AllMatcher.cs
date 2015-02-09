namespace VisualMutator.Model.CoverageFinder
{
    using System;
    using System.Reflection;
    using log4net;
    using Microsoft.Cci;

    public abstract class CodePartsMatcher : ICodePartsMatcher
    {
        public abstract bool Matches(IMethodReference method);
        public abstract bool Matches(ITypeReference typeReference);
        
    }

    public class DelegatingMatcher : CodePartsMatcher
    {
        private readonly Func<IMethodReference, bool> _methodMatching;
        private readonly Func<ITypeReference, bool> _typeMatching;

        public DelegatingMatcher()
        {
        }

        public DelegatingMatcher(
            Func<IMethodReference, bool> methodMatching, 
            Func<ITypeReference, bool> typeMatching)
        {
            _methodMatching = methodMatching;
            _typeMatching = typeMatching;
        }

        public override bool Matches(IMethodReference method)
        {
            return _methodMatching(method);
        }

        public override bool Matches(ITypeReference typeReference)
        {
            return _typeMatching(typeReference);
        }
    }

    public class AllMatcher : CodePartsMatcher
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
      
        public override bool Matches(IMethodReference method)
        {
            return true;
        }

        public override bool Matches(ITypeReference typeReference)
        {
            return true;
        }
    }
}