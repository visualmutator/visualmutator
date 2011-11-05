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
        
        public void Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types)
        {

            var controllers = types.Where(t => t.IsOfType("System.Web.Mvc.Controller")).ToList();
            var list = new List<Tuple<MethodDefinition, MethodDefinition>>();
            foreach (var controllerType in controllers)
            {

                var validGroups = controllerType.Methods
                    .Where(m => m.ReturnType.Resolve().IsOfType("System.Web.Mvc.ActionResult")).ToList()
                    .GroupBy(m => m.Parameters,  new ParametersComparer()).ToList()
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

            var duplicatedSwapped = list.SelectMany(pair => new[] { pair, Tuple.Create(pair.Item2, pair.Item1) }).ToList();



            var contructor = GetActionNameAttribute(module);
            var stringType = module.Import(typeof(string));

            foreach (var pair in duplicatedSwapped)
            {
                var result = new CustomAttribute(contructor);
                result.ConstructorArguments.Add(new CustomAttributeArgument(stringType, pair.Item2.Name));
    
                pair.Item1.CustomAttributes.Add(result);
            }


        }


        public IEnumerable<MutationTarget> FindTargets(IEnumerable<TypeDefinition> types)
        {
            var controllers = types.Where(t => t.IsOfType("System.Web.Mvc.Controller")).ToList();
            var list = new List<Tuple<MethodDefinition, MethodDefinition>>();
            foreach (var controllerType in controllers)
            {

                var validGroups = controllerType.Methods
                    .Where(m => m.ReturnType.Resolve().IsOfType("System.Web.Mvc.ActionResult")).ToList()
                    .GroupBy(m => m.Parameters, new ParametersComparer()).ToList()
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

            var duplicatedSwapped = list.SelectMany(pair => new[] { pair, Tuple.Create(pair.Item2, pair.Item1) }).ToList();

            return null;

        }

        public void Mutate(MutationTarget target, IList<AssemblyDefinition> assembliesToMutate)
        {
            throw new NotImplementedException();
        }

        public MutationResultsCollection CreateMutants(IEnumerable<MutationTarget> targets, AssembliesToMutateFactory assembliesFactory)
        {
            throw new NotImplementedException();
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
                return "Swap Action Names";
            }
        }

        public string Description
        {
            get
            {
                return "Swaps the names of two actions.";
            }
        }





  
        public class ParametersComparer : IEqualityComparer<Collection<ParameterDefinition>>
    
        {


            public bool Equals(Collection<ParameterDefinition> first, Collection<ParameterDefinition> second)
            {
                if(first.Count != second.Count)
                {
                    return false;
                }
                return !first.Where((t, i) => t.ParameterType.FullName != second[i].ParameterType.FullName).Any();
            }

            public int GetHashCode(Collection<ParameterDefinition> enumerable)
            {
                return enumerable.Aggregate(17, (sum, one) => 7 * sum + one.ParameterType.FullName.GetHashCode());
            }

        }

    }

  
}