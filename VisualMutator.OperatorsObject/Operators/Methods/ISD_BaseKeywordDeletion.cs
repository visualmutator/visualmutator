namespace VisualMutator.OperatorsObject.Operators.Methods
{
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    // using OpCodes = Mono.Cecil.Cil.OpCodes;

    public class ISD_BaseKeywordDeletion : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("ISD", "Base keyword deletion", "");
            }
        }
      

        public class ISDVisitor : OperatorCodeVisitor
        {
            public override void Visit(IExpressionStatement statement)
            {
                var call = statement.Expression as MethodCall;
                if (call != null)
                {
                    var method = call.MethodToCall.ResolvedMethod;
                    if (method.IsVirtual && !Parent.CurrentMethod.IsNewSlot
                     && method.Name.Value == Parent.CurrentMethod.Name.Value )
                    {//
                  //  && Parent.CurrentMethod.ContainingTypeDefinition.BaseClasses
                  //      .Any(c => c.ResolvedType == method.ContainingTypeDefinition)
                        MarkMutationTarget(statement);
                    }
                }
                
            }
        }

        public class ISDRewriter : OperatorCodeRewriter
        {

            public override IStatement Rewrite(IExpressionStatement statement)
            {
               
                return new EmptyStatement();
            } 
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new ISDVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new ISDRewriter();
        }



    
    }
}
