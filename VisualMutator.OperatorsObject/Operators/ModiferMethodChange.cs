namespace VisualMutator.OperatorsObject.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using VisualMutator.Extensibility;

    using log4net;

    // using OpCodes = Mono.Cecil.Cil.OpCodes;

    public class ModiferMethodChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EMM", "Modiﬁer Method Change", "");
            }
        }
      

        public IOperatorCodeVisitor FindTargets()
        {
            return new ModiﬁerMethodChangeVisitor();

        }

        public IOperatorCodeRewriter Mutate()
        {
            return new ModiﬁerMethodChangeRewriter();
        }



        public static bool IsPropertyModifier(IMethodDefinition method)
        {
            return method.IsSpecialName && method.ContainingTypeDefinition
                                               .Properties.Any(p => p.Setter.Name.UniqueKey == method.Name.UniqueKey);

        }

        private static bool TryGetCompatibileModifier(IMethodDefinition resolvedMethod, out IMethodDefinition accessor)
        {
            var result = resolvedMethod.ContainingTypeDefinition.Properties
                .FirstOrDefault(p => p.Setter.Name.UniqueKey != resolvedMethod.Name.UniqueKey
                && TypeHelper.ParameterListsAreEquivalent(p.Setter.Parameters, resolvedMethod.Parameters));
            if (result == null)
            {
                accessor = default(IMethodDefinition);
                return false;
            }
            else
            {
                accessor = result.Setter.ResolvedMethod;
                return true;
            }

        }
      
    
        public class ModiﬁerMethodChangeVisitor : OperatorCodeVisitor
        {
            public override void Visit(IMethodCall methodCall)
            {
                _log.Info("Visiting IMethodCall: " + methodCall);

                if(IsPropertyModifier(methodCall.MethodToCall.ResolvedMethod))
                {
                    IMethodDefinition accessor;
                    if(TryGetCompatibileModifier(methodCall.MethodToCall.ResolvedMethod, out accessor))
                    {
                        MarkMutationTarget(methodCall, accessor.Name.Value.InList());
                    }

                }

            }

       

       
        }

        public class ModiﬁerMethodChangeRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall methodCall)
            {
                _log.Info("Rewrite IMethodCall: " + methodCall);
                var methodDefinition = TypeHelper.GetMethod(methodCall.MethodToCall.ContainingType.ResolvedType, 
                    NameTable.GetNameFor(MutationTarget.PassInfo), methodCall.Arguments.Select(a => a.Type).ToArray());
                var newCall = new MethodCall(methodCall);
                newCall.MethodToCall = methodDefinition;
                return newCall;
            }
           
        }

    }
}
