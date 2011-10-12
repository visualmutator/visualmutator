namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure.WpfUtils;

    using Mono.Cecil;

    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Infrastructure.Factories;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests;

    using log4net;

    #endregion

    public interface IMutantsContainer
    {

        ReadOnlyObservableCollection<StoredMutantInfo> GeneratedMutants
        {
            get;
        }

    
     
        ExecutedOperator GenerateMutantsForOperator(MutationSessionChoices choices, IMutationOperator op);
    }





    public class MutantsContainer : IMutantsContainer
    {
        private readonly BetterObservableCollection<StoredMutantInfo> _generatedMutants;



        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public MutantsContainer(
            )
        {
 

            _generatedMutants = new BetterObservableCollection<StoredMutantInfo>();
        }


        public ReadOnlyObservableCollection<StoredMutantInfo> GeneratedMutants
        {
            get
            {
                return new ReadOnlyObservableCollection<StoredMutantInfo>(_generatedMutants);
            }
        }


        /*
        public StoredMutantInfo GenerateMutant(string name, Action<string> mutationLog)
        {
            
            IEnumerable<TypeDefinition> types = _typesManager.GetIncludedTypes();
            if (!types.Any())
            {
                throw new NoTypesSelectedException();
            }

            ModuleDefinition module = types.First().Module;

            IEnumerable<IMutationOperator> operators = _operatorsManager.GetActiveOperators();

        

            var list = new List<MutationResultDetails>();


            foreach (IMutationOperator mutationOperator in operators)
            {
                mutationLog("Applying operator: " + mutationOperator.Name);

                MutationResultDetails result = null;
                try
                {
                    result = mutationOperator.Mutate(module, types);
                    
                }
                catch (Exception e)
                {
                   // _log.Error("Operator exception: ", e);
                    mutationLog("Operator threw an exception: " + e);
                   // _log.Error("Mutation failed.");
                    throw new MutationException("Mutation failed.", e);
                }
                if (result != null)
                {
                    list.Add(result);
                }
            }

           // IEnumerable<IEnumerable<TypeDefinition>> enumerable = list.Select(r => r.ModifiedMethods
            //.GroupBy(m => m.DeclaringType).Select(g => g.Key));

            var session = new MutationSession
            {
                Name = name,
                DateOfCreation = _dateTimeNowFactory.Create(),
                UsedOperators = list,
           
                Assemblies = new List<string>(),
            };
            mutationLog("Saving mutant...");

            IEnumerable<AssemblyDefinition> assemblies = _typesManager.GetLoadedAssemblies();

            _mutantsFileManager.StoreMutant(session, assemblies);
            return session;

            
            throw new NotImplementedException();
        }

      */


        public void Clear()
        {
           
           _generatedMutants.Clear();
        }

  







        public ExecutedOperator GenerateMutantsForOperator(MutationSessionChoices choices, IMutationOperator op)
        {
            var stream = new MemoryStream();
            foreach (var assemblyDefinition in choices.Assemblies)
            {
                assemblyDefinition.Write(stream);
            }

            var targets = op.FindTargets(choices.SelectedTypes);

            var results = op.CreateMutants(targets, 
                new AssembliesToMutateFactory(() => Read(stream, choices.Assemblies.Count)));

            return new ExecutedOperator
            {
                Name = op.Name,
                Mutants = results.MutationResults.Select(res => new Mutant(res.MutatedAssemblies)).ToList()
            };

        }


        private IList<AssemblyDefinition> Read(Stream stream, int count)
        {
            stream.Position = 0;
            var list = new List<AssemblyDefinition>();
            for (int i = 0; i < count; i++)
            {
                list.Add(AssemblyDefinition.ReadAssembly(stream));
            }
            return list;
        }






    }
}