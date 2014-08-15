namespace VisualMutator.Model.Mutations.Operators
{
    #region

    using Extensibility;
    using Microsoft.Cci;

    #endregion

    public class IdentityOperator : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("I", "Identity operator", "Operator that does not create any changes.");
            }
        }
      
        public IOperatorCodeVisitor CreateVisitor()
        {
            return new OperatorCodeVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new OperatorCodeRewriter();
        }
    }

    public class IdentityOperator2 : IMutationOperator
    {
        class Visitor :OperatorCodeVisitor
        {
            private bool _found;
            public override void Visit(IAddition addition)
            {
                if (!_found)
                {
                      _found = true;
                      MarkMutationTarget(addition);
                }
            }
        }

        class Rewriter : OperatorCodeRewriter
        {
//            public override IExpression Rewrite(IAddition addition)
//            {
//                return base.Rewrite(addition);
//            }
        }
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("I", "Identity operator", "Operator that does not create any changes.");
            }
        }
      
        public IOperatorCodeVisitor CreateVisitor()
        {
            return new Visitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new Rewriter();
        }
    }
}