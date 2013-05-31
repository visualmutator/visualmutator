namespace VisualMutator.OperatorsObject.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CommonUtilityInfrastructure;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using log4net;

    public class EXS_ExceptionSwallowing : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EXS", "Exception swallowing", "");
            }
        }

        public class EXSVisitor : OperatorCodeVisitor
        {
            public override void Visit(ITryCatchFinallyStatement operation)
            {
                _log.Info("Visit ITryCatchFinallyStatement: " + operation);
                var systemException = Parent.CurrentMethod.ContainingTypeDefinition.PlatformType.SystemException;
                if (operation.CatchClauses.Any() && 
                    operation.CatchClauses.All(c => ((INamedTypeReference)c.ExceptionType) != systemException))
                {
                    MarkMutationTarget(operation);
                }
            }
        }


        public class EXSRewriter : OperatorCodeRewriter
        {
     
            public override IStatement Rewrite(ITryCatchFinallyStatement operation)
            {
                _log.Info("Rewriting ITryCatchFinallyStatement: " + operation + " Pass: " + MutationTarget.PassInfo);
                var systemException = CurrentMethod.ContainingTypeDefinition.PlatformType.SystemException;
                var tryCatch = new TryCatchFinallyStatement(operation);

                tryCatch.CatchClauses.Add(new CatchClause
                {
                    ExceptionType = systemException,
                    Body = new BlockStatement()
                    {
                        Statements = new List<IStatement>{ new EmptyStatement()},
                    }
                });
                return tryCatch;
            }
           
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new EXSVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new EXSRewriter();
        }

    }
}