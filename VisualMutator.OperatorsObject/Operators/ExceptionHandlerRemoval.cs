namespace VisualMutator.OperatorsObject.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    public class ExceptionHandlerRemoval : IMutationOperator
    {
        #region IMutationOperator Members

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EHR", "Exception Handler Removal", "");
            }
        }
      

        public IOperatorCodeVisitor FindTargets()
        {
            return new ExceptionHandlerRemovalVisitor();
        }

        public IOperatorCodeRewriter Mutate()
        {
            return new ExceptionHandlerRemovalRewriter();
        }

        #endregion

        #region Nested type: ExceptionHandlerRemovalRewriter

        public class ExceptionHandlerRemovalRewriter : OperatorCodeRewriter
        {
     
            public override IStatement Rewrite(ITryCatchFinallyStatement operation)
            {
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

        #endregion

        #region Nested type: ExceptionHandlerRemovalVisitor

        public class ExceptionHandlerRemovalVisitor : OperatorCodeVisitor
        {
        
            public override void Visit(ITryCatchFinallyStatement operation)
            {
                if(operation.CatchClauses.Count() >= 2 || 
                    (operation.CatchClauses.Count() == 1 && operation.FinallyBody != null))
                {
                    var passes = new List<string>();
                    passes.AddRange(Enumerable.Range(0, operation.CatchClauses.Count()).Select(i => i.ToString()));
                    if (operation.FinallyBody != null)
                    {
                        passes.Add("Finally");
                    }
                    MarkMutationTarget(operation, passes);
                }
            }
           
        }

        #endregion
    }
}