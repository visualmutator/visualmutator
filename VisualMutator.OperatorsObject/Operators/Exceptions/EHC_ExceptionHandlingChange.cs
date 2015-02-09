namespace VisualMutator.OperatorsObject.Operators.Exceptions
{
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.ExtensionMethods;

    public class EHC_ExceptionHandlingChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("EHC", "Exception handling change", "");
            }
        }

        public class EXSVisitor : OperatorCodeVisitor
        {
            public override void Visit(ICatchClause catchClause)
            {
              //  _log.Info("Visit ICatchClause: " + catchClause);

                if (catchClause.ExceptionContainer != Dummy.LocalVariable) //if local variable is declared
                {
                    if(catchClause.Body.Statements.Any())
                    {
                        var throwStatement = catchClause.Body.Statements.First() as ThrowStatement;
                        if(throwStatement == null || throwStatement.Exception.Type != catchClause.ExceptionType)
                        {
                            //if doesnt already have the same throw
                             MarkMutationTarget(catchClause);
                        }
                    }
                    else // Empty block
                    {
                        MarkMutationTarget(catchClause);
                    }
                }
                else if (TypeHelper.GetMethod(catchClause.ExceptionType.ResolvedType, 
                    Host.NameTable.GetNameFor(".ctor")) != Dummy.MethodDefinition)
                {
                    MarkMutationTarget(catchClause);
                }
            }
        }


        public class EXSRewriter : OperatorCodeRewriter
        {

            public override ICatchClause Rewrite(ICatchClause catchClause)
            {
                _log.Info("Rewriting ITryCatchFinallyStatement: " + catchClause + " Pass: " + MutationTarget.PassInfo);
                IExpression expression;
                if (catchClause.ExceptionContainer != Dummy.LocalVariable)
                {
                    expression = new BoundExpression()
                    {
                        Definition = catchClause.ExceptionContainer,
                        Type = catchClause.ExceptionType,
                    };
                }
                else
                {
                    var ctor = TypeHelper.GetMethod(catchClause.ExceptionType.ResolvedType, Host.NameTable.GetNameFor(".ctor"));
                    expression = new CreateObjectInstance()
                    {
                        MethodToCall = ctor,
                        Type = catchClause.ExceptionContainer.Type
                    };

                }
                return new CatchClause(catchClause)
                {
                    Body = new BlockStatement()
                    {
                        Statements = new ThrowStatement()
                        {
                            Exception = expression
                        }.InList<IStatement>()
                    }
                };
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