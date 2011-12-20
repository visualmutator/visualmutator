namespace VisualMutator.MvcMutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;
    using Mono.Collections.Generic;

    using VisualMutator.Extensibility;

    public class ChangeParameterName : AspNetMvcOperator
    {
        public override string Identificator
        {
            get
            {
                return "CAPN";
            }
        }

        public override string Name
        {
            get
            {
                return "Change Action Parameter Name";
            }
        }

        public override string Description
        {
            get
            {
                return "Changes ation prameters names";
            }
        }

        public override IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {

            return from actionDef in GetAllActions(types)
            where actionDef.Parameters.Count != 0
            select new MutationTarget().Add("actionWithParameters", actionDef);
        /*    var actionTypes = (from type in types
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
*/
        }
        

        public override void Mutate(MutationContext context)
        {
            var method = context.Method("actionWithParameters");

            foreach (var param in method.Parameters)
            {
                param.Name = "mutatedParameterName";
            }
          
        }
    }
}