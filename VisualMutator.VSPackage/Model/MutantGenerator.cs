namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

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

        private readonly IVisualStudioConnection _visualStudio;



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
            IVisualStudioConnection visualStudio
            )
        {
            _operatorsManager = operatorsManager;
            _typesManager = typesManager;
            _visualStudio = visualStudio;

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
            string name = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");
            var session = new MutationSession(name, operators, types);

            session.Run();


            SaveSession(session);
            

            _generatedMutants.Add(session);

            


        }


        private void SaveSession(MutationSession session)
        {
            var assemblies = session.Types.Select(t => t.Module.Assembly).Distinct();

            string path = _visualStudio.CreateMutantsRootFolderPath();
            foreach (var assemblyDefinition in assemblies)
            {
                string dir = Path.Combine(path, session.Name);
                Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, assemblyDefinition.Name.Name + ".dll");
                assemblyDefinition.Write(file);
            }
    
         
        }

    }
}