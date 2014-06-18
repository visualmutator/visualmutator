namespace VisualMutator.Tests.Mutations
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Model;
    using Model.Mutations.MutantsTree;
    using Model.StoringMutants;

    public class MutantMaterializer
    {
        private readonly IProjectClonesManager _clonesManager;
        private readonly MutationSessionChoices _choices;
        private readonly IMutantsCache _mutantsCache;

        public MutantMaterializer(IProjectClonesManager clonesManager,
            MutationSessionChoices choices,
             IMutantsCache mutantsCache)
        {
            _clonesManager = clonesManager;
            _choices = choices;
            _mutantsCache = mutantsCache;
        }

        public async Task<StoredMutantInfo> StoreMutant(Mutant mutant)
        {
            mutant.State = MutantResultState.Creating;

            var mutationResult = await _mutantsCache.GetMutatedModulesAsync(mutant);

            mutant.State = MutantResultState.Writing;

            var clone = await _clonesManager.CreateCloneAsync("InitTestEnvironment");
            var info = new StoredMutantInfo(clone);

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

            var otherModules = _choices.WhiteSource
                .Where(_ => singleMutated == null || _.Module.Name != singleMutated.Name);

            foreach (var otherModule in otherModules)
            {
                string file = Path.Combine(info.Directory, otherModule.Module.Name + ".dll");
                info.AssembliesPaths.Add(file);
            }

            return info;
        }
    }
}