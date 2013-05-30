namespace VisualMutator.OperatorsObject.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using log4net;

    public class EHR_ExceptionHandlerRemoval : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EHR", "Exception Handler Removal", "");
            }
        }

        public class EHRVisitor : OperatorCodeVisitor
        {

            public override void Visit(ITryCatchFinallyStatement operation)
            {
                _log.Info("Visit ITryCatchFinallyStatement: " + operation);
                if (operation.CatchClauses.Count() >= 2 )//||
                   // (operation.CatchClauses.Count() == 1 && operation.FinallyBody != null))
                {
                    var passes = new List<string>();
                    passes.AddRange(Enumerable.Range(0, operation.CatchClauses.Count()).Select(i => i.ToString()));
                 /*   if (operation.FinallyBody != null)
                    {
                        passes.Add("Finally");
                    }*/
                    MarkMutationTarget(operation, passes);
                }
            }

        }


        public class EHRRewriter : OperatorCodeRewriter
        {
     
            public override IStatement Rewrite(ITryCatchFinallyStatement operation)
            {
                _log.Info("Rewriting ITryCatchFinallyStatement: " + operation + " Pass: " + MutationTarget.PassInfo);
                var tryCatch = new TryCatchFinallyStatement(operation);
                if (MutationTarget.PassInfo == "Finally")
                {
                    tryCatch.FinallyBody = null;
                }
                else
                {
                    tryCatch.CatchClauses.RemoveAt(int.Parse(MutationTarget.PassInfo));
                }
                return tryCatch;
            }
           
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new EHRVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new EHRRewriter();
        }

    }
}