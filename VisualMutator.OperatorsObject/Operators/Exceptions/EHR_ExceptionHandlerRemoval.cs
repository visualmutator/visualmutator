namespace VisualMutator.OperatorsObject.Operators.Exceptions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    public class EHR_ExceptionHandlerRemoval : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EHR", "Exception handler removal", 
                    "Removes a catch block from a group of two or more catch blocks or removes a finally block.");
            }
        }

        public class EHRVisitor : OperatorCodeVisitor
        {

            public override void Visit(ITryCatchFinallyStatement operation)
            {
              //  _log.Info("Visit ITryCatchFinallyStatement: " + operation);
                if (operation.CatchClauses.Count() >= 2 )
                {
                    var passes = new List<string>();
                    passes.AddRange(Enumerable.Range(0, operation.CatchClauses.Count()).Select(i => i.ToString()));

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