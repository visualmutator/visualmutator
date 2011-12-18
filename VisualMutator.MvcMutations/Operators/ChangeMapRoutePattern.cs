namespace VisualMutator.MvcMutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Runtime.Remoting.Contexts;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using VisualMutator.Extensibility;

    #endregion

    [Export(typeof(IMutationOperator))]
    public class ChangeMapRoutePattern : IMutationOperator
    {
        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {
            var methods = types.SelectMany(t => t.Methods).Where(m => m.HasBody); 

            return from method in methods
                   from instrWithIndex in method.Body.Instructions
                   where instrWithIndex.OpCode == OpCodes.Call
                   let methodRef = (MethodReference)instrWithIndex.Operand
                   where methodRef.DeclaringType.FullName == "System.Web.Mvc.RouteCollectionExtensions"
                   where methodRef.Name == "MapRoute"
                   where methodRef.ReturnType.FullName == "System.Web.Routing.Route"
                   where methodRef.FullName == "System.Web.Routing.Route System.Web.Mvc.RouteCollectionExtensions::MapRoute(System.Web.Routing.RouteCollection,System.String,System.String,System.Object)"
                   where instrWithIndex.Previous.OpCode == OpCodes.Newobj
                   let constructorOperand = (MethodReference)instrWithIndex.Previous.Operand
                   where constructorOperand.DeclaringType.Name.StartsWith("<>f__AnonymousType")
                         && constructorOperand.DeclaringType.IsGenericInstance
                   let generic = ((GenericInstanceType)constructorOperand.DeclaringType)
                   let ldstrRouteInstr = instrWithIndex.GoBackBy(generic.GenericArguments.Count + 2)
                   select new MutationTarget().Add("ldstrRouteInstr", method, ldstrRouteInstr);
        }

        public void Mutate(MutationContext context)
        {
            var instr = context.MethodAndInstruction("ldstrRouteInstr");
            instr.Instruction.Operand = "MutatedString";

        }

        public string Name
        {
            get
            {
                return "CMRA - Change MapRoute Address Pattern";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces MapRoute method's pattern string with irrelevant value. Acts only on MapRoute(RouteCollection,String,String,Object) overload.";
            }
        }


        public void Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types)
        {
            var methods = types.SelectMany(t => t.Methods).Where(m => m.HasBody); //.SelectMany(m=>m.Body.Instructions)

            foreach (MethodDefinition methodDefinition in methods)
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
                    .Select(i => new
                    {
                        Instr = i,
                        MethodRef = (MethodReference)i.Operand
                    })
                    .Where(_ => _.MethodRef.FullName == "System.Web.Routing.Route System.Web.Mvc.RouteCollectionExtensions::MapRoute(System.Web.Routing.RouteCollection,System.String,System.String,System.Object)")
                    .Where(_ => _.Instr.Previous.OpCode == OpCodes.Newobj)
                    // .Select(_=>new { _.Instr, ConstrRef = (MethodReference)_.Instr.Previous.Operand})
                    // .Where()
                    .Select(_ => _.Instr)
                    .ToList();

                foreach (Instruction nameRouteValuesInstruction in nameRouteValuesCallInstructions)
                {
                    var constructorOperand = (MethodReference)nameRouteValuesInstruction.Previous.Operand;

                    if (constructorOperand.DeclaringType.Name.StartsWith("<>f__AnonymousType")
                        && constructorOperand.DeclaringType.IsGenericInstance)
                    {
                        var generic = ((GenericInstanceType)constructorOperand.DeclaringType);
                        var ldstrRouteInstr = generic.GenericArguments
                            .Aggregate(nameRouteValuesInstruction.Previous.Previous, (current, x) => current.Previous);

                       // MutateRoute(ldstrRouteInstr);
                    }
                }
            }
        }

    }
}