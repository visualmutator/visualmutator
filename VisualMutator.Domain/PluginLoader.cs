namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Text;

    using VisualMutator.Extensibility;

    public class PluginLoader
    {
//        [ImportMany(AllowRecomposition = true)]
//        public IEnumerable<IMutationOperator> Senders
//        {
//            get;

        //            set;

        //        }

        [ImportMany(AllowRecomposition = true)]
                public IEnumerable<IOperatorsPack> Senders
                {
                    get;
                    set;
                }

        public PluginLoader()
        {
            var catalog = new DirectoryCatalog(Path.Combine(Environment.CurrentDirectory, "Extensions"));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);



         
            foreach (var operatorsPack in Senders)
            {
                var catalog2 = new AssemblyCatalog(operatorsPack.GetType().Assembly);
                var container2 = new CompositionContainer(catalog);

                var pack = new MyClass();

                container.ComposeParts(pack);

                foreach (var mutationOperator in pack.Oper)
                {
                    Console.Out.WriteLine(mutationOperator);
                }
            }

            //
            //            foreach (var mutationOperator in Senders)
            //            {
            //                Console.Out.WriteLine(mutationOperator);
            //            }


        }
    }

    class MyClass
    {
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IMutationOperator> Oper { get; set; } 
    }


}