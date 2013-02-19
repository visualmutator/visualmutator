namespace VisualMutator.OperatorsStandard
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;

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
      
        

        public IOperatorCodeVisitor FindTargets()
        {
            return new AbsoluteValueInsertionVisitor();
        }

        public IOperatorCodeRewriter Mutate()
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