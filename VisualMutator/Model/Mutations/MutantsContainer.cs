namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Serialization;

    using FileUtils;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    using VisualMutator.Extensibility;
    using VisualMutator.Infrastructure;

    using log4net;

    #endregion

    public interface IMutantsContainer
    {
        IOperatorsManager OperatorsManager { get; }

        BetterObservableCollection<MutationSession> GeneratedMutants { get; }

        MutationSession GenerateMutant(string name, Action<string> mutationLog);

        void LoadSessions();

        void DeleteMutant(MutationSession selectedMutant);

        void Clear();

        void SaveSettingsFile();
    }

    public class TestOperator : IMutationOperator
    {
        public MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types)
        {
            int i = 0;
            foreach (var typeDefinition in types)
            {
                typeDefinition.Name = "TestTypeName"+i++;
            }
            return new MutationResultDetails
            {
                ModifiedMethods = new List<string>(),
                
            };
        }

        public string Name
        {
            get
            {
                return "TestName";
            }
        }

        public string Description
        {
            get
            {
                return "TestDescription";
            }
        }
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

 
        public IOperatorsManager OperatorsManager
        {
            get
            {
                return _operatorsManager;
            }
        }

        public BetterObservableCollection<MutationSession> GeneratedMutants
        {
            get
            {
                return _generatedMutants;
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

            IEnumerable<OperatorNode> operators = _operatorsManager.GetActiveOperators();

        

            var list = new List<MutationResultDetails>();
            foreach (OperatorNode mutationOperator in operators)
            {
                mutationLog("Applying operator: " + mutationOperator.Operator.Name);
                var result = mutationOperator.Operator.Mutate(module, types);
                list.Add(result);
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

        public void SaveSettingsFile()
        {
            _mutantsFileManager.SaveSettingsFile(_generatedMutants);
        }
    }
}