namespace VisualMutator.OperatorsObject.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using VisualMutator.Extensibility;

    using log4net;

    public class PRV_ReferenceAssignmentChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("PRV", " Reference assignment with other compatible type", "");
            }
        }
      
        private static bool isCompatibile(ITypeReference target, ITypeDefinition source)
        {
            return TypeHelper.Type1DerivesFromOrIsTheSameAsType2(source, target)
                || TypeHelper.Type1ImplementsType2(source, target);
        }

        public class PRVVisitor : OperatorCodeVisitor
        {
           
            public override void Visit(IAssignment assignment)
            {
                _log.Info("Visiting IAssignment: " + assignment);
            //    var defaultEqualsDefinition = TypeHelper.GetMethod(Host.PlatformType.SystemObject.ResolvedType.Members,
              //                                                    Host.NameTable.GetNameFor("Equals"), 
              //                                                    Host.PlatformType.SystemObject);
                var targetType = assignment.Target.Type;
                IMethodDefinition currentMethod = this.Parent.CurrentMethod;
                


                var field = currentMethod.ContainingTypeDefinition.Fields
                    .Where(f => f.IsStatic == currentMethod.IsStatic)
                    .FirstOrDefault(f => isCompatibile(targetType, f.Type.ResolvedType)
                        && FieldIsNotSource(f, assignment.Source));

                if (field != null)
                {

                    MarkMutationTarget(assignment);
                }
               //.. assignment.Source = new BoundExpression();
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

            private bool FieldIsNotSource(IFieldDefinition fieldDefinition, IExpression source)
            {
                var bound = source as BoundExpression;
                var field = bound == null? null : bound.Definition as IFieldReference;
                bool ret = bound == null || field == null || field.ResolvedField != fieldDefinition;
                return ret;
            }
        }

        public class PRVRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IAssignment assignment)
            {
                _log.Info("Rewriting IAssignment: " + assignment + " Pass: " + MutationTarget.PassInfo);

                var targetType = assignment.Target.Type;
                IMethodDefinition currentMethod = CurrentMethod;
                var field = currentMethod.ContainingTypeDefinition.Fields
                    .Where(f => f.IsStatic == currentMethod.IsStatic)
                    .First(f => isCompatibile(targetType, f.Type.ResolvedType));

                var assignmentNew = new Assignment(assignment);

                assignmentNew.Source = new BoundExpression
                {
                    Instance = new ThisReference(){Type = CurrentMethod.ContainingTypeDefinition},
                    Definition = field,
                    Type = field.Type,
                };
                return assignmentNew;
            }
         
        }

      

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new PRVVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new PRVRewriter();
        }



    }
}
