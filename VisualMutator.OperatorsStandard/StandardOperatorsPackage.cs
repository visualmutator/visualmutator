namespace VisualMutator.OperatorsStandard
{
    using System.Collections.Generic;
    using Extensibility;
    using Operators;

    [PackageExport]
    public class StandardOperatorsPackage : IOperatorsPackage
    {
        public StandardOperatorsPackage()
        {
            Operators = new IMutationOperator[]
            {
  
                new AOR_ArithmeticOperatorReplacement(), 
               // new ABS_AbsoluteValueInsertion(),
                new SOR_ShiftOperatorReplacement(), 
               // new UOI_UnaryOperatorInsertion(), 
                new LCR_LogicalConnectorReplacement(), 
                new LOR_LogicalOperatorReplacement(), 
                new ROR_RelationalOperatorReplacement(), 
                new OODL_OperatorDeletion(),
                new SSDL_StatementBlockDeletion(),
                

            };
        }
        public IEnumerable<IMutationOperator> Operators
        {
            get; 
            set;
        }
        public string Name
        {
            get
            {
                return "Standard";
            }
        }
        public string Description
        {
            get
            {
                return "Standard imperative operators.";
            }
        }
    }
}