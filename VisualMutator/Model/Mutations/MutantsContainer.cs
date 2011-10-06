namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure.WpfUtils;

    using Mono.Cecil;

    using VisualMutator.Extensibility;
    using VisualMutator.Infrastructure.Factories;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests;

    using log4net;

    #endregion

    public interface IMutantsContainer
    {

        ReadOnlyObservableCollection<MutationSession> GeneratedMutants
        {
            get;
        }

        MutationSession GenerateMutant(string name, Action<string> mutationLog);

        void LoadSessions();

        void DeleteMutant(MutationSession selectedMutant);

        void Clear();

        void DeleteAllMutants();

        void AddMutant(MutationSession result);
    }





    public class MutantsContainer : IMutantsContainer
    {
        private readonly BetterObservableCollection<MutationSession> _generatedMutants;

        private readonly IOperatorsManager _operatorsManager;

        private readonly ITypesManager _typesManager;

        private readonly IMutantsFileManager _mutantsFileManager;

        private readonly IFactory<DateTime> _dateTimeNowFactory;

   

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public MutantsContainer(
            IOperatorsManager operatorsManager,
            ITypesManager typesManager,
            IMutantsFileManager mutantsFileManager,
            IFactory<DateTime> dateTimeNowFactory
            )
        {
            _operatorsManager = operatorsManager;
            _typesManager = typesManager;
            _mutantsFileManager = mutantsFileManager;
            _dateTimeNowFactory = dateTimeNowFactory;
       

            _generatedMutants = new BetterObservableCollection<MutationSession>();
        }


        public ReadOnlyObservableCollection<MutationSession> GeneratedMutants
        {
            get
            {
                return new ReadOnlyObservableCollection<MutationSession>(_generatedMutants);
            }
        }



        public MutationSession GenerateMutant(string name, Action<string> mutationLog)
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


        }

        public void LoadSessions()
        {
      
            _generatedMutants.ReplaceRange(_mutantsFileManager.LoadSessions());
   
        }

        public void DeleteMutant(MutationSession mutant)
        {
            
            _generatedMutants.Remove(mutant);
            _mutantsFileManager.SaveSettingsFile(_generatedMutants);

            _mutantsFileManager.DeleteMutantFiles(mutant);

        }

        public void Clear()
        {
           
           _generatedMutants.Clear();
        }

  
        public void DeleteAllMutants()
        {

            foreach (MutationSession mutant in _generatedMutants)
            {
                
                _mutantsFileManager.DeleteMutantFiles(mutant);
            }
            
           
            _generatedMutants.Clear();
            _mutantsFileManager.SaveSettingsFile(_generatedMutants);
        }

        public void AddMutant(MutationSession result)
        {
            _generatedMutants.Add(result);
            _mutantsFileManager.SaveSettingsFile(_generatedMutants);
        }
    }
}