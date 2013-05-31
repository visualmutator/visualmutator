namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;

    public class OperatorCodeVisitor : OperatorCodeVisitorBase, IOperatorCodeVisitor
    {

 
        public void MarkMutationTarget<T>(T obj, List<string> passesInfo = null)
        {
            if (passesInfo == null)
            {
                passesInfo = new List<string>(){""};
            }
            Parent.MarkMutationTarget(obj, passesInfo.Select(s => new MutationVariant(s, new Dictionary<string, object>())).ToList());
        }
        public void MarkMutationTarget<T>(T obj, string passInfo)
        {
            MarkMutationTarget(obj, passInfo.InList());
        }
        public void MarkMutationTarget<T>(T obj, MutationVariant variant)
        {
            Parent.MarkMutationTarget(obj, variant.InList());
        }
        public void MarkMutationTarget<T>(T obj, List<MutationVariant> variants)
        {
            Parent.MarkMutationTarget(obj, variants);
        }
        public void MarkCommon<T>(T obj)
        {
            Parent.MarkSharedTarget(obj);
        }
       
        
        public virtual void Initialize()
        {
            
        }


        public VisualCodeVisitor Parent
        {
            get;
            set;
        }

        public IOperatorUtils OperatorUtils
        {
            get;
            set;
        }
        public MetadataReaderHost Host
        {
            get;
            set;
        }
    }
}