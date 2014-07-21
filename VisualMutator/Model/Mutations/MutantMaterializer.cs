namespace VisualMutator.Model.Mutations
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Model;
    using Model.StoringMutants;
    using MutantsTree;

    public class MutantMaterializer
    {
        private readonly IProjectClonesManager _clonesManager;
        private readonly OriginalCodebase _originalCodebase;
        private readonly IMutantsCache _mutantsCache;

        public MutantMaterializer(IProjectClonesManager clonesManager,
            OriginalCodebase originalCodebase,
             IMutantsCache mutantsCache)
        {
            _clonesManager = clonesManager;
            _originalCodebase = originalCodebase;
            _mutantsCache = mutantsCache;
        }

        public async Task<StoredMutantInfo> StoreMutant(Mutant mutant)
        {
            mutant.State = MutantResultState.Creating;

            var mutationResult = await _mutantsCache.GetMutatedModulesAsync(mutant);

            mutant.State = MutantResultState.Writing;

            var clone = await _clonesManager.CreateCloneAsync("InitTestEnvironment");
            var info = new StoredMutantInfo(clone);


            if(mutationResult.MutatedModules != null)
            {
                var singleMutated = mutationResult.MutatedModules.Modules.SingleOrDefault();
                if (singleMutated != null)
                {
                    //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
                    string file = Path.Combine(info.Directory, singleMutated.Name + ".dll");

                    var memory = mutationResult.MutatedModules.WriteToStream(singleMutated);
                    // _mutantsCache.Release(mutationResult);

                    using (FileStream peStream = File.Create(file))
                    {
                        await memory.CopyToAsync(peStream);
                    }

                    info.AssembliesPaths.Add(file);
                }

                var otherModules = _originalCodebase.Modules
                    .Where(_ => singleMutated == null || _.Module.Name != singleMutated.Name);

                foreach (var otherModule in otherModules)
                {
                    string file = Path.Combine(info.Directory, otherModule.Module.Name + ".dll");
                    info.AssembliesPaths.Add(file);
                }
            }
            else
            {
                foreach (var cciModuleSource in mutationResult.Old)
                {
                    var module = cciModuleSource.Modules.Single();
                    //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
                    string file = Path.Combine(info.Directory, module.Name + ".dll");

                    var memory = cciModuleSource.WriteToStream(module);
                    // _mutantsCache.Release(mutationResult);

                    using (FileStream peStream = File.Create(file))
                    {
                        await memory.CopyToAsync(peStream);
                    }

                    info.AssembliesPaths.Add(file);
                }
            }
            

            return info;
        }
    }
}