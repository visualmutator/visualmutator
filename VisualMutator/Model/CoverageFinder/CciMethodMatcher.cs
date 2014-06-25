namespace VisualMutator.Model.CoverageFinder
{
    using System.Reflection;
    using log4net;
    using Microsoft.Cci;

    public class CciMethodMatcher : ICodePartsMatcher
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MethodIdentifier _identifier;

        public CciMethodMatcher(MethodIdentifier identifier)
        {
            _identifier = identifier;
            _log.Debug("Creatng searcher for: " + _identifier);
        }
      
        public bool Matches(IMethodReference method)
        {
            var sig = CreateIdentifier(method);
            _log.Debug("matching: "+sig);
            return sig == _identifier;
        }

        public bool Matches(ITypeReference typeReference)
        {
            typeReference = TypeHelper.UninstantiateAndUnspecialize(typeReference);
            string name = TypeHelper.GetTypeName(typeReference,
                    NameFormattingOptions.TypeParameters );
            _log.Debug("Matching type : " + name+" by " + _identifier.ClassName);
            return _identifier.ClassName == name;
        }

        public static MethodIdentifier CreateIdentifier(IMethodReference method)
        {
            method = MemberHelper.UninstantiateAndUnspecialize(method);
            return new MethodIdentifier(MemberHelper.GetMethodSignature(method,
                    NameFormattingOptions.Signature |
                    NameFormattingOptions.TypeParameters |
                    NameFormattingOptions.ParameterModifiers));
        }
    }
}