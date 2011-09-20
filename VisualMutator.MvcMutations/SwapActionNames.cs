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
        
        public MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types)
        {

            var controllers = types.Where(t => t.IsOfType("System.Web.Mvc.Controller"));
            var list = new List<Tuple<MethodDefinition, MethodDefinition>>();
            foreach (var controllerType in controllers)
            {
                var validGroups = controllerType.Methods.GroupBy(m => m.Parameters, 
             //       new FuncComparer<Collection<ParameterDefinition>>((c1,c2)=>Colle))
               // Where(g => g.Count() >= 2).ToList();
                
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



            return null;


        }

        private void MutateRoute(Instruction ldstrRouteInstr)
        {
            ldstrRouteInstr.Operand = "MutatedString";
        }



        public MethodDefinition GetActionNameAttribute(ModuleDefinition currentModule)
        {
            var mvcModule = CecilExtensions.GetAspNetMvcModule(currentModule);


            var attr = mvcModule.Types.Single(t => t.FullName == "System.Web.Mvc.ActionNameAttribute");
            return attr.Methods.Single(m => m.FullName ==
        "System.Void System.Web.Mvc.ActionNameAttribute::.ctor(System.String)");

         
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
        private class FuncComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;

            public FuncComparer(Func<T, T, bool> comparer)
            {
                if (comparer == null)
                    throw new ArgumentNullException("comparer");

                _comparer = comparer;
            }

            public bool Equals(T x, T y)
            {
                return _comparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                throw new NotSupportedException();
            }
        }

        private static class Comparers
        {
            public static FuncComparer<T> Func<T>(Func<T, T, bool> comparer)
            {
                return new FuncComparer<T>(comparer);
            }
        }


    }

  
}