namespace VisualMutator.OperatorsStandard.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    public class SSDL_StatementBlockDeletion : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class SSDLVisitor : OperatorCodeVisitor
        {
            public override void Visit(IAssignment operation)
            {
                ProcessOperation(operation);
            }
            
            private void ProcessOperation (IAssignment assignment) 
            {
                MarkMutationTarget(assignment);
            }

        }
        public class SSDLRewriter : OperatorCodeRewriter
        {

            private IExpression ReplaceOperation(IAssignment operation)
            {
                IExpression result = operation.Source;
                

                return result;
            }
            public override IExpression Rewrite(IAssignment operation)
            {
                return ReplaceOperation(operation);
            }
        }
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("SSDL", "Statement Block Deletion", "");
            }
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new SSDLVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new SSDLRewriter();
        }
    }
}
