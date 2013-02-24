namespace VisualMutator.OperatorsObject.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;

    public class ArgumentOrderChange : IMutationOperator
    {
        #region IMutationOperator Members

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("OAO", "Argument Order Change", "");
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


            public override void Visit(IMethodCall methodCall)
            {
                
                var thisMethod = methodCall.MethodToCall.ResolvedMethod;
                var currentDefinition = thisMethod.ContainingTypeDefinition;
                //Find overloads in this class
                List<IMethodDefinition> allOverloadingMethods = 
                currentDefinition.GetMatchingMembersNamed(thisMethod.Name,
                    false, member => member is IMethodDefinition).Cast<IMethodDefinition>().ToList();

                //Add overloads from base classes
                while (currentDefinition.BaseClasses.Any())
                {
                    allOverloadingMethods.AddRange(currentDefinition.BaseClasses.Single()
                        .ResolvedType.GetMatchingMembersNamed(thisMethod.Name,
                    false, member => member is IMethodDefinition).Cast<IMethodDefinition>());
                }

                allOverloadingMethods = allOverloadingMethods.Where(m =>
                    !TypeHelper.ParameterListsAreEquivalent(m.Parameters, thisMethod.Parameters)).ToList();

                allOverloadingMethods = allOverloadingMethods.Where(m =>
                    m.Parameters.Count() == thisMethod.Parameters.Count()).ToList();

              /*  foreach (var method in allOverloadingMethods)
                {
                    method.Parameters.Di
                }



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
                }*/
            }

        }

        #endregion
    }
}