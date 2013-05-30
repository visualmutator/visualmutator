namespace VisualMutator.OperatorsObject.Operators
{
    using System.Linq;

    using Microsoft.Cci.MutableCodeModel;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;


    public class MCI_MemberCallFromAnotherInheritedClass : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("MCI", "Member call from another inherited class", "");
            }
        }

        private static bool isCompatibile(ITypeReference target, ITypeDefinition source)
        {
            return TypeHelper.Type1DerivesFromOrIsTheSameAsType2(source, target)
                || TypeHelper.Type1ImplementsType2(source, target);
        }
        private static bool FieldIsNotThis(IFieldDefinition fieldDefinition, IExpression source)
        {
            var bound = source as BoundExpression;
            var field = bound == null ? null : bound.Definition as IFieldReference;
            bool ret = bound == null || field == null || field.ResolvedField != fieldDefinition;
            return ret;
        }
        private static IFieldDefinition TryFindField(IMethodCall call, IMethodDefinition currentMethod)
        {
            var targetType = call.ThisArgument.Type;
   
            var field = currentMethod.ContainingTypeDefinition.Fields
                .Where(f => f.IsStatic == currentMethod.IsStatic)
                .FirstOrDefault(f => isCompatibile(targetType, f.Type.ResolvedType)
                    && FieldIsNotThis(f, call.ThisArgument) && !call.MethodToCall.ResolvedMethod.IsConstructor);
            return field;
        }
        public class MCIVisitor : OperatorCodeVisitor
        {
            public override void Visit(IMethodCall call)
            {


                var field = TryFindField(call, Parent.CurrentMethod);

                if(field != null)
                {
                    MarkMutationTarget(call);
                }

               
            }
        }

        public class MCIRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall call)
            {
                var field = TryFindField(call, CurrentMethod);
                if (field != null)
                {
                    return new MethodCall(call)
                    {
                        ThisArgument = new BoundExpression()
                        {
                            Instance = new ThisReference()
                            {
                                Type = CurrentMethod.ContainingTypeDefinition
                            },
                            Type = field.Type,
                            Definition = field,

                        }
                    };
                }
                else
                {
                    return call;
                }
            }
        }

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new MCIVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new MCIRewriter();
        }

    }
}