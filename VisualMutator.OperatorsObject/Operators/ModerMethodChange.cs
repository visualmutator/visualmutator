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

    public class ModerMethodChange : IMutationOperator
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



      
      
    
        public class ModiﬁerMethodChangeVisitor : OperatorCodeVisitor
        {

            public bool IsPropertyAccessor(IMethodDefinition method)
            {
                return method.IsSpecialName && method.ContainingTypeDefinition
                                                   .Properties.Any(p => p.Getter.Name.UniqueKey == method.Name.UniqueKey);

            }


            public override void Visit(IMethodCall methodCall)
            {
                _log.Info("Visiting IMethodCall: " + methodCall);



                if(IsPropertyAccessor(methodCall.MethodToCall.ResolvedMethod))
                {
                    IMethodDefinition accessor;
                    if(TryGetCompatibileAccessor(methodCall.MethodToCall.ResolvedMethod, out accessor))
                    {
                        
                    }

                }

               // methodCall.MethodToCall.ResolvedMethod.
               // methodCall.ThisArgument.Type.ResolvedType.Properties
                var defaultEqualsDefinition = TypeHelper.GetMethod(Host.PlatformType.SystemObject.ResolvedType.Members,
                                                                  Host.NameTable.GetNameFor("Equals"),
                                                                  Host.PlatformType.SystemObject);
               TypeHelper.GetProperty(methodCall.MethodToCall.ResolvedMethod.ContainingTypeDefinition, 
                   Host.NameTable.GetNameFor())
                
                var methodDefinition = methodCall.MethodToCall.ResolvedMethod;
                var containingType = methodCall.ThisArgument.Type.ResolvedType;
                _log.Info("IMethodCall is of " + methodDefinition);
                //Check if the type overrides the Equals method
                if (methodDefinition.Equals(defaultEqualsDefinition) && containingType.IsClass
                    && containingType.BaseClasses.Any())
                {
                    var overridingMethod = TypeHelper.GetMethod(containingType.Members,
                        Host.NameTable.GetNameFor("Equals"),
                        Host.PlatformType.SystemObject);

                    if(overridingMethod.IsVirtual)
                    {
                        MarkMutationTarget(methodCall);
                    }

                }

                


            }

            private bool TryGetCompatibileAccessor(IMethodDefinition resolvedMethod, out IMethodDefinition accessor)
            {
                var result = resolvedMethod.ContainingTypeDefinition.Properties
                    .FirstOrDefault(p => p.Getter.Name.UniqueKey != resolvedMethod.Name.UniqueKey
                    && TypeHelper.ParameterListsAreEquivalent(p.Getter.Parameters, resolvedMethod.Parameters));
                if(result == null)
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

            public override void Visit(IEquality operation)
            {
                _log.Info("Visiting IEquality: " + operation);
                var passes = new List<string>();
                foreach (Tuple<IExpression, string> pair in 
                    Utility.Pairs<IExpression, string>(operation.LeftOperand, "Left", operation.RightOperand, "Right"))
                {
                    ITypeDefinition operandType = pair.Item1.Type.ResolvedType;
                    if (operandType.BaseClasses.Any() && operandType.IsClass 
                        && TypeHelper.GetMethod(operandType.Members, 
                        Host.NameTable.GetNameFor("Equals"), Host.PlatformType.SystemObject) != Dummy.MethodDefinition)
                    {
                        passes.Add(pair.Item2);
                    }
                }
                if(passes.Any())
                {
                    MarkMutationTarget(operation, passes);
                }

            }
        }

        public class ModiﬁerMethodChangeRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall methodCall)
            {
                _log.Info("Rewriting IMethodCall: " + methodCall + " Pass: " + MutationTarget.PassInfo);
                var equality = new Equality();
                equality.LeftOperand = methodCall.ThisArgument;
                equality.RightOperand = methodCall.Arguments.Single();
                equality.Type = Host.PlatformType.SystemBoolean;
                return equality;
            }
            public override IExpression Rewrite(IEquality operation)
            {
                _log.Info("Rewriting IEquality: " + operation + " Pass: " + MutationTarget.PassInfo);
                var methodCall = new MethodCall();
                IExpression thisArgument;
                IExpression argument;
                if (MutationTarget.PassInfo == "Left")
                {
                    thisArgument = operation.LeftOperand;
                    argument = operation.RightOperand;
                }
                else
                {
                    thisArgument = operation.RightOperand;
                    argument = operation.LeftOperand;
                }
                methodCall.ThisArgument = thisArgument;
                methodCall.MethodToCall = TypeHelper.GetMethod(Host.PlatformType.SystemObject.ResolvedType.Members,
                                                                  Host.NameTable.GetNameFor("Equals"),
                                                                  Host.PlatformType.SystemObject);
                methodCall.Arguments = argument.InList();

                return methodCall;
            }
        }

    }
}
