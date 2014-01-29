namespace VisualMutator.OperatorsObject
{
    using System.Collections.Generic;
    using Extensibility;
    using Operators.Delegates;
    using Operators.Exceptions;
    using Operators.Methods;
    using Operators.Variables;

    [PackageExport]
    public class ObjectOperatorsPackage : IOperatorsPackage
    {
        public ObjectOperatorsPackage()
        {
 

            Operators = new IMutationOperator[]
            {
                new DEH_MethodDelegatedForEventHandlingChange(), 
                new DMC_DelegatedMethodChange(), 
                //new EOC_EqualityOperatorChange(), 
                new EMM_ModiferMethodChange(), 
                new EAM_AccessorMethodChange(), 
                new EHC_ExceptionHandlingChange(), 
                new EHR_ExceptionHandlerRemoval(), 
                new EXS_ExceptionSwallowing(), 
                new ISD_BaseKeywordDeletion(), 
                new JID_FieldInitializationDeletion(), 
                new JTD_ThisKeywordDeletion(), 
               // new RFI_ReferencingFaultInsertion(), 
                new PRV_ReferenceAssignmentChange(), 
                new MCI_MemberCallFromAnotherInheritedClass(), 
              //  new NullInsertion(), 
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
                return "Object";
            }
        }

        public string Description
        {
            get
            {
                return "Object oriented Operators.";
            }
        }
    }
}