namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
  
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    public interface IMutantGenerator
    {
        IOperatorsManager OperatorsManager { get; }
        void GenerateMutants();

        ObservableCollection<MutationSession> GeneratedMutants { get; }

        void LoadSessions();
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
        public string SessionsFile
        {
            get
            {
                string path = _visualStudio.GetMutantsRootFolderPath();
                return Path.Combine(path, "mutants.xml");
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

            
        
            foreach (var mutationOperator in operators)
            {
                mutationOperator.Operator.Mutate(types);
            }


            SaveSession(operators, types);
            

        }


        private void SaveSession(IEnumerable<MutationOperator> operators, IEnumerable<TypeDefinition> types)
        {
            var assemblies = types.Select(t => t.Module.Assembly).Distinct();
            string name = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");


            var session = new MutationSession
            {
                Name = name,
                UsedOperators = operators.Select(o => o.Name).ToList(),
                MutatedTypes = types.Select(t => t.Name).ToList(),
                Assemblies = new List<string>()
            };
            

            string path = _visualStudio.GetMutantsRootFolderPath();
            foreach (var assemblyDefinition in assemblies)
            {
                string dir = Path.Combine(path, name);
                Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, assemblyDefinition.Name.Name + ".dll");
                assemblyDefinition.Write(file);
                session.Assemblies.Add(file);
            }
            
            _generatedMutants.Add(session);

          //  File.Create(SessionsFile);
            var ser = new XmlSerializer(typeof(List<MutationSession>));

            using (var file = new StreamWriter(SessionsFile))
            {
                ser.Serialize(file, _generatedMutants.ToList());
            }

            
        }


        public void LoadSessions()
        {
            if (File.Exists(SessionsFile))
            {
                var ser = new XmlSerializer(typeof(List<MutationSession>));
                List<MutationSession> list = null;
                using (var file = new StreamReader(SessionsFile))
                {
                    try
                    {
                        list = (List<MutationSession>)ser.Deserialize(file);
                        foreach (var session in list)
                        {
                            _generatedMutants.Add(session);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
                
                
            }
            



        }




    }
}