namespace VisualMutator.MvcMutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using VisualMutator.Extensibility;

    #endregion

    [Export(typeof(IMutationOperator))]
    public class ChangeRoute : IMutationOperator
    {
      /*
        public MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types)
        {
           
            var methods = types.SelectMany(t=>t.Methods).Where(m=>m.HasBody);//.SelectMany(m=>m.Body.Instructions)

            foreach (var methodDefinition in methods)
            {
                //TODO: calli, call virt?

                var validCallInstructions = from instr in methodDefinition.Body.Instructions
                where instr.OpCode == OpCodes.Call
                let methodRef = (MethodReference)instr.Operand
                where methodRef.DeclaringType.FullName == "System.Web.Mvc.RouteCollectionExtensions"
                where methodRef.Name == "MapRoute"
                where methodRef.ReturnType.FullName == "System.Web.Routing.Route"
                select instr;
                
                var nameRouteValuesCallInstructions = validCallInstructions
                    .Select(i=>new {Instr = i, MethodRef = (MethodReference)i.Operand})
                    .Where(_ => _.MethodRef.FullName == "System.Web.Routing.Route System.Web.Mvc.RouteCollectionExtensions::MapRoute(System.Web.Routing.RouteCollection,System.String,System.String,System.Object)")
                    .Where(_=>_.Instr.Previous.OpCode == OpCodes.Newobj)
                   // .Select(_=>new { _.Instr, ConstrRef = (MethodReference)_.Instr.Previous.Operand})
                   // .Where()
                    .Select(_=>_.Instr)
                    .ToList();

                foreach (var nameRouteValuesInstruction in nameRouteValuesCallInstructions)
                {
                    var constructorOperand = (MethodReference)nameRouteValuesInstruction.Previous.Operand;

                    if (constructorOperand.DeclaringType.Name.StartsWith("<>f__AnonymousType") 
                        && constructorOperand.DeclaringType.IsGenericInstance)
                    {
                        var generic = ((GenericInstanceType)constructorOperand.DeclaringType);
                        var ldstrRouteInstr = generic.GenericArguments

                            .Aggregate(nameRouteValuesInstruction.Previous.Previous, (current, x) => current.Previous);
                     
                        MutateRoute(ldstrRouteInstr);
                    }

                }
            }


            return null;
        }
        */
        private void MutateRoute(Instruction ldstrRouteInstr)
        {
            ldstrRouteInstr.Operand = "MutatedString";
        }

      



        public string Name
        {
            get
            {
                return "Mutate route.";
            }
        }

        public string Description
        {
            get
            {
                return "..";
            }
        }

        public IEnumerable<MutationTarget> FindTargets(IEnumerable<TypeDefinition> types)
        {
            throw new NotImplementedException();
        }

        public MutationResultsCollection CreateMutants(IEnumerable<MutationTarget> targets, AssembliesToMutateFactory assembliesFactory)
        {
            throw new NotImplementedException();
        }


    }

  
}