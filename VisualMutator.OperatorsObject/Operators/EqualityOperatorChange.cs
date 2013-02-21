namespace VisualMutator.OperatorsObject.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using VisualMutator.Extensibility;

    // using OpCodes = Mono.Cecil.Cil.OpCodes;

    public class EqualityOperatorChange : IMutationOperator
    {


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EOC", "Equality Operator Change", "");
            }
        }
      

        public IOperatorCodeVisitor FindTargets()
        {
            return new ExceptionHandlerRemovalVisitor();

        }

        public IOperatorCodeRewriter Mutate()
        {
            return new ExceptionHandlerRemovalRewriter();
        }



        #region Nested type: ExceptionHandlerRemovalRewriter

        public class ExceptionHandlerRemovalRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall methodCall)
            {
                var equality = new Equality();
                equality.LeftOperand = methodCall.ThisArgument;
                equality.RightOperand = methodCall.Arguments.Single();
                equality.Type = Host.PlatformType.SystemBoolean;
                return methodCall;
            }
            public override IExpression Rewrite(IEquality operation)
            {
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
                methodCall.MethodToCall = TypeHelper.GetMethod(thisArgument.Type.ResolvedType.Members,
                                                                  Host.NameTable.GetNameFor("Equals"),
                                                                  Host.PlatformType.SystemObject);
                methodCall.Arguments = argument.InList();
                
                return methodCall;
            }
        }

        #endregion

        #region Nested type: ExceptionHandlerRemovalVisitor

        public class ExceptionHandlerRemovalVisitor : OperatorCodeVisitor
        {

            public override void Visit(IMethodCall methodCall)
            {
                var defaultEqualsDefinition = TypeHelper.GetMethod(Host.PlatformType.SystemObject.ResolvedType.Members,
                                                                  Host.NameTable.GetNameFor("Equals"),
                                                                  Host.PlatformType.SystemObject);
                var methodDefinition = methodCall.MethodToCall.ResolvedMethod;
                var containingType = methodCall.MethodToCall.ContainingType.ResolvedType;

                //Check if the type overrides the Equals method
                if (methodCall.MethodToCall.Name.Value == "Equals" && containingType.IsClass 
                    && containingType.BaseClasses.Any()
                    && TypeHelper.ParameterListsAreEquivalent(methodDefinition.Parameters,
                    defaultEqualsDefinition.Parameters))
                {
                    MarkMutationTarget(methodCall);
                }


            }
            public override void Visit(IEquality operation)
            {
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

        #endregion
    }
}
