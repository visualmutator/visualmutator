namespace VisualMutator.OperatorsObject.Operators.Variables
{
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    public class RFI_ReferencingFaultInsertion : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("RFI", "Referencing fault insertion", "");
            }
        }
      

        public class RFIVisitor : OperatorCodeVisitor
        {
            private bool CheckForExistingNull(object obj)
            {
                var conversion = obj as IConversion;
                if (conversion != null)
                {
                    var constant = conversion.ValueToConvert as ICompileTimeConstant;
                    if (constant != null && constant.Value == null)
                    {
                        return true;
                    }
                }
                var initialConstant = obj as ICompileTimeConstant;
                if (initialConstant != null && initialConstant.Value == null)
                {
                    return true;
                }
                return false;
            }
            public override void Visit(IAssignment assignment)
            {
                if (!CheckForExistingNull(assignment.Source))
                {
                    MarkMutationTarget(assignment);
                }
            }
            public override void Visit(ILocalDeclarationStatement declaration)
            {
                if (!CheckForExistingNull(declaration.InitialValue))
                {
                    MarkMutationTarget(declaration);
                }
            }
        }

        public class RFIRewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IAssignment assignment)
            {

                return new Assignment(assignment)
                {
                    Source = new Conversion()
                    {
                        Type = assignment.Source.Type,
                        TypeAfterConversion = assignment.Source.Type,
                        ValueToConvert = new CompileTimeConstant()
                        {
                            Type = Host.PlatformType.SystemObject,
                            Value = null,
                        }
                    }
                };
            }
            public override IStatement Rewrite(ILocalDeclarationStatement declaration)
            {

                return new LocalDeclarationStatement(declaration)
                {
                    InitialValue = new Conversion()
                    {
                        Type = declaration.InitialValue.Type,
                        TypeAfterConversion = declaration.InitialValue.Type,
                        ValueToConvert = new CompileTimeConstant()
                        {
                            Type = Host.PlatformType.SystemObject,
                            Value = null,
                        }
                    }
                };
            } 
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new RFIVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new RFIRewriter();
        }



    
    }
}
