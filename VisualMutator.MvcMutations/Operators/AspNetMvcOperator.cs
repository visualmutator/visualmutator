namespace VisualMutator.MvcMutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Extensibility;

    public abstract class AspNetMvcOperator : IMutationOperator
    {


        public IEnumerable<MethodDefinition> GetAllActions(ICollection<TypeDefinition> types)
        {
             var controllers = types.Where(t => t.IsOfType("System.Web.Mvc.Controller"));

             return controllers.SelectMany(c=>c.Methods)
                     .Where(m =>
                            !m.IsAbstract &&
                            m.ReturnType.FullName == "System.Web.Mvc.ActionResult");
             
        }


        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types);

        public abstract void Mutate(MutationContext context);
    }
}