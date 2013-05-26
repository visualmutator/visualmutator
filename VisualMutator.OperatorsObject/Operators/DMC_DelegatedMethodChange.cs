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

    // using OpCodes = Mono.Cecil.Cil.OpCodes;

    public class DMC_DelegatedMethodChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("DMC", "Delegated method change", "");
            }
        }
      

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new DMCVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new DMCRewriter();
        }



    
        public class DMCVisitor : OperatorCodeVisitor
        {
            public override void Visit(ICreateDelegateInstance createDelegate)
            {
                var delegatedMethod = createDelegate.MethodToCallViaDelegate.ResolvedMethod;
                var otherMethod = Parent.CurrentMethod.ContainingTypeDefinition.Methods
                    .FirstOrDefault(m => m != delegatedMethod
                    && TypeHelper.ParameterListsAreEquivalent(delegatedMethod.Parameters, m.Parameters)
                    && m.Type == delegatedMethod.Type);

                if (otherMethod != null)
                {
                    MarkMutationTarget(createDelegate, otherMethod.ToString());
                }
            }

       

       
        }

        public class DMCRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(ICreateDelegateInstance createDelegate)
            {
                var newDel = new CreateDelegateInstance(createDelegate);
                newDel.MethodToCallViaDelegate =
                    CurrentMethod.ContainingTypeDefinition.Methods
                    .First(m => m.ToString() == MutationTarget.PassInfo);
                return newDel;
            }
           
        }

    }
}
