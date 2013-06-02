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


    public class EOC_EqualityOperatorChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EOC", "Equality Operator Change", "");
            }
        }

        public class EOCVisitor : OperatorCodeVisitor
        {

            public override void Visit(IMethodCall methodCall)
            {
                _log.Info("Visiting IMethodCall: " + methodCall);
                var defaultEqualsDefinition = TypeHelper.GetMethod(Host.PlatformType.SystemObject.ResolvedType.Members,
                                                                  Host.NameTable.GetNameFor("Equals"),
                                                                  Host.PlatformType.SystemObject);


                var methodDefinition = methodCall.MethodToCall.ResolvedMethod;
                var containingType = methodCall.ThisArgument.Type.ResolvedType;
                _log.Info("IMethodCall is of " + methodDefinition);
                //Check if the type overrides the Equals method
                if (containingType != Host.PlatformType.SystemString &&
                        containingType != Host.PlatformType.SystemObject &&
                        !containingType.IsValueType &&
                    methodDefinition.Equals(defaultEqualsDefinition) && containingType.IsClass
                    && containingType.BaseClasses.Any())
                {
                    var overridingMethod = TypeHelper.GetMethod(containingType.Members,
                        Host.NameTable.GetNameFor("Equals"),
                        Host.PlatformType.SystemObject);

                    if (overridingMethod.IsVirtual)
                    {
                        MarkMutationTarget(methodCall);
                    }

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
                    if (operandType != Host.PlatformType.SystemString &&
                        operandType != Host.PlatformType.SystemObject &&
                        !operandType.IsValueType &&
                        operandType.BaseClasses.Any() && operandType.IsClass
                        && TypeHelper.GetMethod(operandType.Members,
                        Host.NameTable.GetNameFor("Equals"), Host.PlatformType.SystemObject) != Dummy.MethodDefinition)
                    {
                        passes.Add(pair.Item2);
                    }
                }
                if (passes.Any())
                {
                    MarkMutationTarget(operation, passes);
                }
            }
        }
     
        public class EOCRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall methodCall)
            {
                _log.Info("Rewriting IMethodCall: " + methodCall+" Pass: "+MutationTarget.PassInfo);
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

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new EOCVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new EOCRewriter();
        }

      

       
    }
}
