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

    public class NullInsertion : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("NU", "Null Insertion", "");
            }
        }
      

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new ModiﬁerMethodChangeVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new ModiﬁerMethodChangeRewriter();
        }


      
    
        public class ModiﬁerMethodChangeVisitor : OperatorCodeVisitor
        {
            public override void Visit(IExpression expression)
            {
                _log.Info("Visiting IMethodCall: " + expression);

                if (!expression.Type.ResolvedType.IsValueType && !(expression is IThisReference))
                {

                    MarkMutationTarget(expression);
                }


            }

       

       
        }

        public class ModiﬁerMethodChangeRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IExpression expression)
            {
                _log.Info("Rewrite IMethodCall: " + expression);
                
                return new CompileTimeConstant{Value = null, Type = expression.Type};
            }
           
        }

    }
}
