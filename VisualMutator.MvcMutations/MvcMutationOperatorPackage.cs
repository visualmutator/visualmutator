namespace VisualMutator.MvcMutations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Text;

    using VisualMutator.Extensibility;


    [PackageExport]
    public class MvcMutationOperatorPackage : IOperatorsPackage
    {
        public MvcMutationOperatorPackage()
        {
         //   var catalog = new AssemblyCatalog(GetType().Assembly);
        //    var container = new CompositionContainer(catalog);
        //    container.ComposeParts(this);


            Operators = new IMutationOperator[]
            {
                new ChangeMapRoutePattern(), 
                new ReplaceViewWithRedirectToAction(), 
                new SwapActionNames(), 
                new RemoveAuthorizeAttribute(), 
                new RedirectToOtherAction(), 
                new ChangeParameterName(), 
            };
        }
      //  [ImportMany]
        public IEnumerable<IMutationOperator> Operators
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "ASP.NET MVC3";
            }
        }

        public string Description
        {
            get
            {
                return "Operators specific to ASP.NET MVC3 web framework.";
            }
        }
    }
}