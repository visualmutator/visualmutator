namespace VisualMutator.MvcMutations
{
    #region Usings

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    using VisualMutator.Extensibility;

    #endregion

    [Export(typeof(IMutationOperator))]
    public class SwapActionNames : IMutationOperator
    {
  


        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {
            var controllers = types.Where(t => t.IsOfType("System.Web.Mvc.Controller")).ToList();
            var list = new List<Tuple<MethodDefinition, MethodDefinition>>();
            foreach (var controllerType in controllers)
            {

                var validGroups = controllerType.Methods
                    .Where(m => m.ReturnType.Resolve().IsOfType("System.Web.Mvc.ActionResult")).ToList()
                    .GroupBy(m => m.Parameters, new ParametersCollectionComparer()).ToList()
                    .Where(g => g.Count() >= 2).ToList();


                foreach (var group in validGroups)
                {
                    var chosen = group.Take(group.Count() - group.Count() % 2).ToArray();
                    for (int i = 0; i < chosen.Length; i += 2)
                    {
                        list.Add(Tuple.Create(chosen[i], chosen[i + 1]));
                    }
                }
            }


            var duplicatedSwapped = list.SelectMany(pair => new[] { pair, Tuple.Create(pair.Item2, pair.Item1) });

            return duplicatedSwapped.Select(pair => new MutationTarget()
                .Add("Method1", pair.Item1).Add("Method2", pair.Item2));

        }

        public void Mutate(MutationContext context)
        {
            var module = context.AssembliesToMutate.First().MainModule;
            var contructor = GetActionNameAttribute(module);
            var stringType = module.Import(typeof(string));

            var method1 = context.Method("Method1");
            var method2 = context.Method("Method2");

            
            var result = new CustomAttribute(contructor);
            result.ConstructorArguments.Add(new CustomAttributeArgument(stringType, method2.Name));
            method1.CustomAttributes.Add(result);

            var result2 = new CustomAttribute(contructor);
            result2.ConstructorArguments.Add(new CustomAttributeArgument(stringType, method1.Name));
            method2.CustomAttributes.Add(result2);
        }

    

        public MethodReference GetActionNameAttribute(ModuleDefinition currentModule)
        {
            var mvcModule = CecilExtensions.GetAspNetMvcModule(currentModule);

            
            var attr = mvcModule.Types.Single(t => t.FullName == "System.Web.Mvc.ActionNameAttribute");
            var method = attr.Methods.Single(m => m.FullName ==
        "System.Void System.Web.Mvc.ActionNameAttribute::.ctor(System.String)");
            return currentModule.Import(method);
       
        }


        public string Name
        {
            get
            {
                return "SWAN - Swap Action Names";
            }
        }

        public string Description
        {
            get
            {
                return "Swaps the names of two actions.";
            }
        }





  

    }

  
}