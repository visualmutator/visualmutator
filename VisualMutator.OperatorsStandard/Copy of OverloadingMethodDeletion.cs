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

    using NList;

    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;

    public class ArgumentOrderChange : IMutationOperator
    {
        #region IMutationOperator Members

        public string Identificator
        {
            get
            {
                return "OAO";
            }
        }

        public string Name
        {
            get
            {
                return "Argument Order Change";
            }
        }

        public string Description
        {
            get { return ""; }
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

        #region Nested type: AbsoluteValueInsertionRewriter

        public class AbsoluteValueInsertionRewriter : OperatorCodeRewriter
        {
            
            public override IMethodDefinition Rewrite(IMethodDefinition method)
            {
                return Dummy.MethodDefinition;
            }
          
         
        }

        #endregion

        #region Nested type: AbsoluteValueInsertionVisitor

        public class AbsoluteValueInsertionVisitor : OperatorCodeVisitor
        {
           
    
            public override void Visit(IMethodCall method)
            {
                var thisMethod = method.MethodToCall.ResolvedMethod;
                var currentDefinition = thisMethod.ContainingTypeDefinition;
                var allOverloadingMethods = 
                currentDefinition.GetMatchingMembersNamed(thisMethod.Name,
                    false, member => member is IMethodDefinition).ToList();
                
                while (currentDefinition.BaseClasses.Any() )
                {
                    allOverloadingMethods.AddRange(currentDefinition.BaseClasses.Single()
                        .ResolvedType.GetMatchingMembersNamed(thisMethod.Name,
                    false, member => member is IMethodDefinition));
                }
                thisMethod.ContainingTypeDefinition.GetMatchingMembersNamed(thisMethod.Name, 
                    false, member => member is IMethodDefinition)
                    .Concat(thisMethod.ContainingTypeDefinition.BaseClasses)



                var types = thisMethod.Parameters.Select(p => p.Type);
                var sublist = types.ToList().ToSublist();
                
              //  sublist.
                //  new[]{4}.ToSublist().
                while(List.NextPermutation(sublist))
                {
                    
                }



                if (method.IsVirtual && method.ContainingTypeDefinition
                    .BaseClasses.Single()
                    .ResolvedType.GetMembersNamed(method.Name, false).Any())
                {
                    MarkMutationTarget(method);
                }
            }

        }

        #endregion
    }
}