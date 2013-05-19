namespace VisualMutator.OperatorsObject.Operators
{
    using System.Linq;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;

    public class BaseKeywordDeletion : IMutationOperator
    {
       
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("ISD", "Super Keyword Deletion", "");
            }
        }
      
        

   

        public class BaseKeywordDeletionVisitor : OperatorCodeVisitor
        {
           
    
            public override void Visit(IMethodDefinition method)
            {
                
               
            }

        }



        public class BaseKeywordDeletionRewriter : OperatorCodeRewriter
        {

            public override IMethodDefinition Rewrite(IMethodDefinition method)
            {
                return null;
            }
        }

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new BaseKeywordDeletionVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new BaseKeywordDeletionRewriter();
        }

    }
}