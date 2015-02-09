namespace VisualMutator.OperatorsObject.Operators.Methods
{
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

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
                accessor = null;
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
              //  _log.Info("Visiting IMethodCall: " + methodCall);
              /*  TypeHelper.GetDefiningUnitReference(methodCall.MethodToCall.ContainingType.ResolvedType);
                Host.CoreAssemblySymbolicIdentity
                var namedTypeReference = methodCall.MethodToCall.ContainingType.ResolvedType as INamedTypeReference;
                if (namedTypeReference != null)
                {
                    namedTypeReference.
                }*/
                if(IsPropertyModifier(methodCall.MethodToCall.ResolvedMethod))
                {
                    IMethodReference accessor;
                    if(TryGetCompatibileModifier(methodCall.MethodToCall.ResolvedMethod, out accessor))
                    {
                        MarkMutationTarget(methodCall, accessor.Name.Value);//w MutationVariant(accessor.Name.Value, accessor));
                    }

                }

            }

       

       
        }

        public class EMMRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall methodCall)
            {
                _log.Info("Rewrite IMethodCall: " + OperatorUtils.Formatter.Format(methodCall));
                var methodDefinition = TypeHelper.GetMethod(methodCall.MethodToCall.ContainingType.ResolvedType, 
                    NameTable.GetNameFor(MutationTarget.PassInfo), methodCall.Arguments.Select(a => a.Type).ToArray());
                var newCall = new MethodCall(methodCall);
                newCall.MethodToCall = methodDefinition;//
             //   (IMethodReference)MutationTarget.StoredObjects.Values.Single();
                _log.Info("Returning MethodCall to: " + OperatorUtils.Formatter.Format(methodCall));
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
