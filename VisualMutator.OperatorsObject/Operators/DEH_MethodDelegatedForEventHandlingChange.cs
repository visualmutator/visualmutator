namespace VisualMutator.OperatorsObject.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using VisualMutator.Extensibility;

    using log4net;

    // using OpCodes = Mono.Cecil.Cil.OpCodes;

    public class DEH_MethodDelegatedForEventHandlingChange : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("DEH", "Method delegated for event handling change", "");
            }
        }
      

        public class DEHVisitor : OperatorCodeVisitor
        {
            public override void Visit(IExpressionStatement statement)
            {
                var call = statement.Expression as MethodCall;
                if (call != null && (call.MethodToCall.Name.Value.StartsWith("remove_") 
                    || call.MethodToCall.Name.Value.StartsWith("add_")))
                {
                    if(call.MethodToCall.ResolvedMethod.IsCil
                    && call.MethodToCall.ResolvedMethod.Parameters.Count() == 1 
                    && call.MethodToCall.ResolvedMethod.Parameters.Single()
                        .Type.ResolvedType.BaseClasses.OfType<NamespaceTypeReference>()
                        .Any(type => type.Name.Value == ("MulticastDelegate")) )
                    {
                        MarkMutationTarget(statement);       
                    }
                             
                }
                
            }
        }

        public class DEHRewriter : OperatorCodeRewriter
        {

            public override IStatement Rewrite(IExpressionStatement statement)
            {
               
                return new EmptyStatement();
            } 
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new DEHVisitor();

        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new DEHRewriter();
        }



    
    }
}
