namespace VisualMutator.OperatorsObject.Operators
{
    using System.Linq;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;

    public class OverloadingMethodDeletion : IMutationOperator
    {
        #region IMutationOperator Members
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("OMD", "Overloading Method Deletion", "");
            }
        }
      
        

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new AbsoluteValueInsertionVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new AbsoluteValueInsertionRewriter();
        }

        #endregion

        #region Nested type: ExceptionHandlerRemovalRewriter

        public class AbsoluteValueInsertionRewriter : OperatorCodeRewriter
        {
            
            public override IMethodDefinition Rewrite(IMethodDefinition method)
            {
                return Dummy.MethodDefinition;
            }
          
         
        }

        #endregion

        #region Nested type: ExceptionHandlerRemovalVisitor

        public class AbsoluteValueInsertionVisitor : OperatorCodeVisitor
        {
           
    
            public override void Visit(IMethodDefinition method)
            {
                
                if (method.IsVirtual && !method.IsAbstract 
                    && method.ContainingTypeDefinition.BaseClasses.Any())
                {
                    var notFound = true;
                    var currentDefinition = method.ContainingTypeDefinition;
                    while (currentDefinition.BaseClasses.Any() && notFound)
                    {
                        var baseMethod = TypeHelper.GetMethod(currentDefinition
                        .BaseClasses.Single().ResolvedType, method);
                        if (baseMethod != Dummy.MethodDefinition)
                        {
                            MarkMutationTarget(method);
                            notFound = false;
                        }
                    }
                    
                }
            }

        }

        #endregion
    }
}