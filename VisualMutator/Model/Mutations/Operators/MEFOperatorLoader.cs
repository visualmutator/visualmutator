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

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.DependencyInjection;

    using VisualMutator.Extensibility;

    #endregion

    public interface IOperatorLoader
    {
        IEnumerable<IOperatorsPackage> ReloadOperators();
    }

    public class MEFOperatorLoader : IOperatorLoader
    {
        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<IOperatorsPackage> OperatorPacks { get; set; }

        public IQueryable<ComposablePart> SelectParts(ComposablePartCatalog catalog)
        {
            var parts = from part in catalog.Parts
                        where part.ExportDefinitions.Any(d => d.ContractName == typeof(IOperatorsPackage).FullName)
                        select part.CreatePart();
            return parts;
        }
        public IEnumerable<IOperatorsPackage> ReloadOperators()
        {
            OperatorPacks = null;
            var extensionDirectory = new Uri(Assembly.GetExecutingAssembly().CodeBase)
                .LocalPath.ToFilePathAbsolute().ParentDirectoryPath;

       //     string path = Path.GetDirectoryName(p);
            var catalog = new DirectoryCatalog(extensionDirectory.Path);
            var container = new CompositionContainer(catalog);

/*
            var compositionBatch = new CompositionBatch();

            foreach (var composablePart in SelectParts(catalog))
            {

                compositionBatch.AddPart(composablePart);
            }*/

            container.ComposeParts(this);

            return OperatorPacks;
        }
    }

    internal class LoadedOperatorPack
    {
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IMutationOperator> Operators { get; set; }
    }
}