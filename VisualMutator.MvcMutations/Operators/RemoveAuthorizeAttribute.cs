namespace VisualMutator.MvcMutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;
    using Mono.Collections.Generic;

    using VisualMutator.Extensibility;

    public class RemoveAuthorizeAttribute : IMutationOperator
    {

        public string Name
        {
            get
            {
                return "RAAT - Remove Authorize Attribute";
            }
        }

        public string Description
        {
            get
            {
                return "Removes Authorize attribute";
            }
        }

        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {
            var controllerTypes = (from type in types
                                  where type.IsOfType("System.Web.Mvc.Controller")
                                  select type).ToList();

            var typesWithAuthorize = controllerTypes
                .Where(type => type.CustomAttributes
                                   .Any(a => a.AttributeType.FullName == "System.Web.Mvc.AuthorizeAttribute"))
                                   .Select(type => new MutationTarget().Add("TypeWithAuthorize", type));


            var methodsWithAuthorize = from controller in controllerTypes
                                       from method in controller.Methods
                                       where method.CustomAttributes
                                           .Any(a => a.AttributeType.FullName == "System.Web.Mvc.AuthorizeAttribute")
                                       select new MutationTarget().Add("MethodWithAuthorize", method);


            return typesWithAuthorize.Concat(methodsWithAuthorize);

        }
      

        public void Mutate(MutationContext context)
        {
            var attributes = context.MutationTarget.ContainsKey("TypeWithAuthorize") 
                            ? context.Type("TypeWithAuthorize").CustomAttributes 
                            : context.Method("MethodWithAuthorize").CustomAttributes;

            var attr = attributes.Single(a => a.AttributeType.FullName == "System.Web.Mvc.AuthorizeAttribute");
            attributes.Remove(attr);
        }
    }
}