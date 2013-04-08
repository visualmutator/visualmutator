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
    using CommonUtilityInfrastructure.Paths;
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
            try
            {
                OperatorPacks = null;
                var extensionDirectory = new Uri(Assembly.GetExecutingAssembly().CodeBase)
                    .LocalPath.ToFilePathAbsolute().ParentDirectoryPath;

                var catalog = new DirectoryCatalog(extensionDirectory.Path);
                var container = new CompositionContainer(catalog);

                container.ComposeParts(this);

                return OperatorPacks;
            }
            catch (ReflectionTypeLoadException e)
            {
                
                throw new Exception(e.LoaderExceptions.Select(ee=>ee.Message).Aggregate( (e1,e2)=>e1+", "+e2),e);
            }
           
        }
    }

    internal class LoadedOperatorPack
    {
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IMutationOperator> Operators { get; set; }
    }
}