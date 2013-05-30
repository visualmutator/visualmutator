namespace VisualMutator.OperatorsObject.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;

    public class OAO_ArgumentOrderChange : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("OAO", "Argument Order Change", "");
            }
        }

    
        public class OAOVisitor : OperatorCodeVisitor
        {


            public override void Visit(IMethodCall methodCall)
            {
                
                var thisMethod = methodCall.MethodToCall.ResolvedMethod;
                var currentDefinition = thisMethod.ContainingTypeDefinition;
                //Find overloads in this class
                List<IMethodDefinition> allOverloadingMethods = 
                currentDefinition.GetMatchingMembersNamed(thisMethod.Name,
                    false, member => member is IMethodDefinition).Cast<IMethodDefinition>().ToList();


                var currentClass = currentDefinition;
                //Add overloads from base classes
                while (currentClass.BaseClasses.Any())
                {
                    allOverloadingMethods.AddRange(currentClass.BaseClasses.Single()
                        .ResolvedType.GetMatchingMembersNamed(thisMethod.Name,
                    false, member => member is IMethodDefinition).Cast<IMethodDefinition>());
                    currentClass = currentClass.BaseClasses.Single().ResolvedType;
                }

                allOverloadingMethods = allOverloadingMethods.Where(m =>
                    !TypeHelper.ParameterListsAreEquivalent(m.Parameters, thisMethod.Parameters)).ToList();

               // MarkMutationTarget(methodCall, );

               // allOverloadingMethods = allOverloadingMethods.Where(m =>
              //      m.Parameters.Count() == thisMethod.Parameters.Count()).ToList();

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

       

        public class OAORewriter : OperatorCodeRewriter
        {

            public override IMethodDefinition Rewrite(IMethodDefinition method)
            {
                return Dummy.MethodDefinition;
            }


        }

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new OAOVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new OAORewriter();
        }
    }
}