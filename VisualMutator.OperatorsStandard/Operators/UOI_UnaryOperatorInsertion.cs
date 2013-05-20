namespace VisualMutator.OperatorsStandard
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using Extensibility;
    using Microsoft.Cci;

    using Microsoft.Cci.MutableCodeModel;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;

    public class UOI_UnaryOperatorInsertion : IMutationOperator
    {
     
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("UOI", "Unary Operator Insertion", "");
            }
        }

     
        public class UOIVisitor : OperatorCodeVisitor
        {
            private void ProcessOperation(IExpression operation)
            {
                //TODO:other types
                if (operation.Type.TypeCode == PrimitiveTypeCode.Int32)
                {
                    MarkMutationTarget(operation, new List<string> { "Negation" });

                }
                else if (operation.Type.TypeCode == PrimitiveTypeCode.Boolean)
                {
                    MarkMutationTarget(operation, new List<string> { "Not" });
                }
            }
            public override void Visit(IExpression operation)
            {
                ProcessOperation(operation);
            }

        }
       
        public class UOIRewriter : OperatorCodeRewriter
        {
            private IExpression ReplaceOperation<T>(T operation) where T : IExpression
            {
                if (MutationTarget.PassInfo == "Plus")
                {
                    return new UnaryPlus
                    {
                        Operand = operation,
                        Type = operation.Type,
                    };
                }
                if (MutationTarget.PassInfo == "Negation")
                {
                    return new UnaryNegation
                    {
                        CheckOverflow = false,
                        Operand = operation,
                        Type = operation.Type,
                    };
                }
                if (MutationTarget.PassInfo == "Not")
                {
                    return new LogicalNot
                    {    
                        Operand = operation,
                        Type = operation.Type,
                    };
                }
                throw new InvalidOperationException();
            }

            public override IExpression Rewrite(IExpression operation)
            {
                return ReplaceOperation(operation);
            }
          
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new UOIVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new UOIRewriter();
        }


    }
}