namespace VisualMutator.OperatorsObject.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using CommonUtilityInfrastructure;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using log4net;
    

    public class ENC_ExceptionNameChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("ENC", "Exception name change", "");
            }
        }

        public class ENCVisitor : OperatorCodeVisitor
        {
            public override void Visit(ITryCatchFinallyStatement operation)
            {
                _log.Info("Visit ITryCatchFinallyStatement: " + operation);
                var systemException = Parent.CurrentMethod.ContainingTypeDefinition.PlatformType.SystemException;
                if (operation.CatchClauses.All(c => ((INamedTypeReference)c.ExceptionType) != systemException) )
                {
                    MarkMutationTarget(operation);
                }
            }
        }


        public class ENCRewriter : OperatorCodeRewriter
        {

            public override void Initialize()
            {
               // CoreAssembly = Host.LoadAssembly(Host.CoreAssemblySymbolicIdentity);

                var host = new PeReader.DefaultHost();
                IModule module = OperatorUtils.CompileModuleFromCode(
@"public class GenException : Exception
{
    public GenException()
    {
    }

    public GenException(object o1) 
    {
    }
    public GenException(object o1, object o2)
    {
    }
}", host);
             /*   GeneratedType = (NamespaceTypeDefinition)module.GetAllTypes().Single(t => t.Name.Value == "VisualMutatorGeneratedClass");
                var methodBody = TypeHelper.GetMethod(GeneratedType,
                    host.NameTable.GetNameFor("FailOnZero"), host.PlatformType.SystemInt32).Body;
                var generatedBody = (SourceMethodBody)methodBody;
                GeneratedBlock = generatedBody.Block;*/
                host.Dispose();
            }


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
            return new ENCVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new ENCRewriter();
        }

    }
}