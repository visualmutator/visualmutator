namespace VisualMutator.OperatorsObject.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using Microsoft.Cci.Ast;
    using VisualMutator.Extensibility;

    using log4net;
    using Equality = Microsoft.Cci.MutableCodeModel.Equality;
    using MethodCall = Microsoft.Cci.MutableCodeModel.MethodCall;

    // using OpCodes = Mono.Cecil.Cil.OpCodes;

    public class ReferenceAssignmentChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        #region Nested type: ExceptionHandlerRemovalVisitor

        public class ReferenceAssignmentChangeVisitor : OperatorCodeVisitor
        {
            private bool isCompatibile(ITypeReference target, ITypeDefinition source)
            {
                return TypeHelper.Type1DerivesFromOrIsTheSameAsType2(source, target)
                    || TypeHelper.Type1ImplementsType2(source, target);
            }
            public override void Visit(IAssignment assignment)
            {
                _log.Info("Visiting IAssignment: " + assignment);
                var defaultEqualsDefinition = TypeHelper.GetMethod(Host.PlatformType.SystemObject.ResolvedType.Members,
                                                                  Host.NameTable.GetNameFor("Equals"), 
                                                                  Host.PlatformType.SystemObject);
                var targetType = assignment.Target.Type;
                IMethodDefinition currentMethod = this.Parent.CurrentMethod;
                IFieldDefinition d;


                var field = currentMethod.ContainingTypeDefinition.Fields
                    .Where(f => f.IsStatic == currentMethod.IsStatic)
                    .FirstOrDefault(f => isCompatibile(targetType, f.Type.ResolvedType));

            //    assignment.Source = new BoundExpression();
                /*
                

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

                    if (overridingMethod.IsVirtual)
                    {
                        MarkMutationTarget(methodCall);
                    }

                }
                */



            }
          
        }

        #endregion

        #region Nested type: ExceptionHandlerRemovalRewriter

        public class ReferenceAssignmentChangeRewriter : OperatorCodeRewriter
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

        #endregion

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("PRV", " Reference assignment with other compatible type", "");
            }
        }
      

        public IOperatorCodeVisitor FindTargets()
        {
            return new ReferenceAssignmentChangeVisitor();

        }

        public IOperatorCodeRewriter Mutate()
        {
            return new ReferenceAssignmentChangeRewriter();
        }



    }
}
