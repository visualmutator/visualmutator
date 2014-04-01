namespace VisualMutator.Model
{
    using System;
    using System.Reflection;
    using log4net;
    using Microsoft.Cci;

    public class CciMethodSearcher
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MethodIdentifier _identifier;

        public CciMethodSearcher(MethodIdentifier identifier)
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