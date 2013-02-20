using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorsStandard
{
    using System.Collections;
    using System.ComponentModel.Composition;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

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

                return methodCall;
            }
            public override IExpression Rewrite(IEquality methodCall)
            {

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
                var passes = (from pair in Utility.Pairs<IExpression, string>(operation.LeftOperand, "Left", 
                                                                              operation.RightOperand, "Right")
                              let operandType = pair.Item1.Type.ResolvedType
                              where operandType.BaseClasses.Any() && operandType.IsClass 
                                  && TypeHelper.GetMethod(operandType.Members, Host.NameTable.GetNameFor("Equals"), 
                                  Host.PlatformType.SystemObject) != Dummy.MethodDefinition
                              select pair.Item2).ToList();
                if(passes.Any())
                {
                    MarkMutationTarget(operation, passes);
                }

            }
        }

        #endregion
    }
}
