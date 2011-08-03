namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;

    public interface IMutantGenerator
    {
        IOperatorsManager OperatorsManager { get; }
        void GenerateMutants();

        ObservableCollection<MutationSession> GeneratedMutants { get; }
    }

    public class MutantGenerator : IMutantGenerator
    {
        private readonly IOperatorsManager _operatorsManager;

        private readonly ITypesManager _typesManager;

        private readonly IAssemblyWriter _assemblyWriter;

        private ObservableCollection<MutationSession> _generatedMutants; 

        public IOperatorsManager OperatorsManager
        {
            get
            {
                return _operatorsManager;
            }
        }

        public MutantGenerator(
            IOperatorsManager operatorsManager, 
            ITypesManager typesManager,
            IAssemblyWriter assemblyWriter
            )
        {
            _operatorsManager = operatorsManager;
            _typesManager = typesManager;
            _assemblyWriter = assemblyWriter;

            _generatedMutants = new ObservableCollection<MutationSession>();
        }

        public ObservableCollection<MutationSession> GeneratedMutants
        {
            get
            {
                return _generatedMutants;
            }
        }

        public void GenerateMutants()
        {

            var types = _typesManager.GetIncludedTypes();
            

          //  var man = new SessionsManager();

            



            var operators = _operatorsManager.GetActiveOperators();

            var session = new MutationSession(operators, types);

            session.Run();



            

            _generatedMutants.Add(session);


         //   man.SaveSession(session);

//            foreach (var assemblyDefinition in assemblies)
//            {
//                _assemblyWriter.Write("test", assemblyDefinition);
//            }

        }


        private void SaveSession(MutationSession session)
        {
            var assemblies = session.Types.Select(t => t.Module.Assembly).Distinct();
            string path = 
            Directory.CreateDirectory()

        }

    }
}