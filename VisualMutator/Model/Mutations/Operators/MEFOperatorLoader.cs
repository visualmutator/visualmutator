namespace VisualMutator.Model.Mutations.Operators
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure.DependencyInjection;

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

        public IQueryable<ComposablePart> SelectParts(ComposablePartCatalog catalog)
        {
            var parts = from part in catalog.Parts
                        where part.ExportDefinitions.Any(d => d.ContractName == typeof(IOperatorsPack).FullName)
                        select part.CreatePart();
            return parts;
        }
        public IEnumerable<IOperatorsPack> ReloadOperators()
        {
            OperatorPacks = null;
            string p = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;

            string path = Path.GetDirectoryName(p);
            var catalog =
                new DirectoryCatalog(path);

            //catalog.Parts.

            var container = new CompositionContainer(catalog);

          /*  var filteredCat = new FilteredCatalog(catalog,
                def => def..Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName) &&
                ((CreationPolicy)def.Metadata[CompositionConstants.PartCreationPolicyMetadataName]) == CreationPolicy.NonShared);
            var child = new CompositionContainer(filteredCat, parent);

            var root = child.GetExportedObject<Root>();
            child.Dispose();
           * 
           * 
*/ 
            var compositionBatch = new CompositionBatch();

            foreach (var composablePart in SelectParts(catalog))
            {
               //compositionBatch.
                compositionBatch.AddPart(composablePart);
            }


           
     
            //compositionBatch.AddExport(new Export());

           // container.Compose(compositionBatch);

          //  OperatorPacks = container.GetExportedValues<IOperatorsPack>();
            container.ComposeParts(this);
/*
            foreach (IOperatorsPack operatorsPack in OperatorPacks)
            {
                var catalog2 = new AssemblyCatalog(operatorsPack.GetType().Assembly);
                var container2 = new CompositionContainer(catalog2);

                var pack = new LoadedOperatorPack();

                container2.ComposeParts(pack);
            }
*/
            return OperatorPacks;
        }
    }

    internal class LoadedOperatorPack
    {
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IMutationOperator> Operators { get; set; }
    }
}