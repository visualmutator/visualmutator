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

    public class EAM_AccessorMethodChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EAM", "Accessor Method Change", "");
            }
        }
      

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new EAMVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new EAMRewriter();
        }



        public static bool IsPropertyAccessor(IMethodDefinition method)
        {
            return method.IsSpecialName && method.ContainingTypeDefinition
                                               .Properties.Any(p => p.Getter.Name.UniqueKey == method.Name.UniqueKey);

        }

        private static bool TryGetCompatibileAccessor(IMethodDefinition resolvedMethod, out IMethodDefinition accessor)
        {
            var result = resolvedMethod.ContainingTypeDefinition.Properties
                .FirstOrDefault(p => p.Getter.Name.UniqueKey != resolvedMethod.Name.UniqueKey
                && TypeHelper.ParameterListsAreEquivalent(p.Getter.Parameters, resolvedMethod.Parameters));
            if (result == null)
            {
                accessor = default(IMethodDefinition);
                return false;
            }
            else
            {
                accessor = result.Getter.ResolvedMethod;
                return true;
            }

        }
      
    
        public class EAMVisitor : OperatorCodeVisitor
        {
            public override void Visit(IMethodCall methodCall)
            {
                

                if(IsPropertyAccessor(methodCall.MethodToCall.ResolvedMethod))
                {
                    IMethodDefinition accessor;
                    if(TryGetCompatibileAccessor(methodCall.MethodToCall.ResolvedMethod, out accessor))
                    {
                        _log.Info("Marking IMethodCall: " + methodCall.MethodToCall.ResolvedMethod + " - " + methodCall.MethodToCall.ResolvedMethod.GetType());
                        MarkMutationTarget(methodCall, accessor.Name.Value.InList());
                    }

                }

            }

       

       
        }

        public class EAMRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall methodCall)
            {
                _log.Info("Rewrite IMethodCall: " + methodCall.MethodToCall.ResolvedMethod + methodCall.MethodToCall.ResolvedMethod.GetType());

                IMethodDefinition accessor;
                if (!TryGetCompatibileAccessor(methodCall.MethodToCall.ResolvedMethod, out accessor))
                {
                    throw new InvalidOperationException("The same accessor was not found.");
                }
                

             //   var methodDefinition = TypeHelper.GetMethod(methodCall.MethodToCall.ContainingType.ResolvedType, 
             //       NameTable.GetNameFor(MutationTarget.PassInfo), methodCall.Arguments.Select(a => a.Type).ToArray());
                var newCall = new MethodCall(methodCall);
                newCall.MethodToCall = accessor;
                return newCall;
            }
           
        }

    }
}
