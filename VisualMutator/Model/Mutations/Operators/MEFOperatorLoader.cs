namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Reflection;

    using VisualMutator.Extensibility;

    #endregion

    public interface IOperatorLoader
    {
        IEnumerable<IOperatorsPack> ReloadOperators();
    }

    public class MEFOperatorLoader : IOperatorLoader
    {
        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<IOperatorsPack> OperatorPacks { get; set; }

        public IEnumerable<IOperatorsPack> ReloadOperators()
        {
            string p = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;

            string path = Path.GetDirectoryName(p);
            var catalog =
                new DirectoryCatalog(path);

            var container = new CompositionContainer(catalog);

            container.ComposeParts(this);

            foreach (IOperatorsPack operatorsPack in OperatorPacks)
            {
                var catalog2 = new AssemblyCatalog(operatorsPack.GetType().Assembly);
                var container2 = new CompositionContainer(catalog2);

                var pack = new LoadedOperatorPack();

                container2.ComposeParts(pack);
            }

            return OperatorPacks;
        }
    }

    internal class LoadedOperatorPack
    {
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IMutationOperator> Operators { get; set; }
    }
}