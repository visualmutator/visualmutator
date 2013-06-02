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

    public class EMM_ModiferMethodChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EMM", "Modiﬁer Method Change", "");
            }
        }
      


        public static bool IsPropertyModifier(IMethodDefinition method)
        {
            return method.IsSpecialName && method.ContainingTypeDefinition
                                               .Properties.Any(p => p.Setter != null 
                                                   && p.Setter.Name.UniqueKey == method.Name.UniqueKey);

        }

        private static bool TryGetCompatibileModifier(IMethodDefinition resolvedMethod, out IMethodReference accessor)
        {
            var result = resolvedMethod.ContainingTypeDefinition.Properties
                .FirstOrDefault(p => p.Setter != null && p.Setter.Name.UniqueKey != resolvedMethod.Name.UniqueKey
                && TypeHelper.ParameterListsAreEquivalent(p.Setter.Parameters, resolvedMethod.Parameters));
            if (result == null)
            {
                accessor = default(IMethodDefinition);
                return false;
            }
            else
            {
                accessor = result.Setter;
                return true;
            }

        }
      
    
        public class EMMVisitor : OperatorCodeVisitor
        {
            public override void Visit(IMethodCall methodCall)
            {
                _log.Info("Visiting IMethodCall: " + methodCall);

                if(IsPropertyModifier(methodCall.MethodToCall.ResolvedMethod))
                {
                    IMethodReference accessor;
                    if(TryGetCompatibileModifier(methodCall.MethodToCall.ResolvedMethod, out accessor))
                    {
                        MarkMutationTarget(methodCall, new MutationVariant(accessor.Name.Value, accessor));
                    }

                }

            }

       

       
        }

        public class EMMRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall methodCall)
            {
                _log.Info("Rewrite IMethodCall: " + Parent.Formatter.Format(methodCall));
               // var methodDefinition = TypeHelper.GetMethod(methodCall.MethodToCall.ContainingType.ResolvedType, 
               //     NameTable.GetNameFor(MutationTarget.PassInfo), methodCall.Arguments.Select(a => a.Type).ToArray());
                var newCall = new MethodCall(methodCall);
                newCall.MethodToCall = (IMethodReference) MutationTarget.StoredObjects.Values.Single();
                _log.Info("Returning MethodCall to: " + Parent.Formatter.Format(methodCall));
                return newCall;
            }
           
        }

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new EMMVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new EMMRewriter();
        }


    }
}
