namespace VisualMutator.OperatorsObject.Operators
{
    using System.Linq;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;

    public class OMD_OverloadingMethodDeletion : IMutationOperator
    {
    
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("OMD", "Overloading Method Deletion", "");
            }
        }
        public class OMDVisitor : OperatorCodeVisitor
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
        
        public class OMDRewriter : OperatorCodeRewriter
        { 
            public override IMethodDefinition Rewrite(IMethodDefinition method)
            {
                return Dummy.MethodDefinition;
            }
        }

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new OMDVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new OMDRewriter();
        }

     
    }
}