namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    #endregion

    public interface IMutantsContainer
    {
        IOperatorsManager OperatorsManager { get; }

        BetterObservableCollection<MutationSession> GeneratedMutants { get; }

        void GenerateMutants();

        void LoadSessions();
    }

    public class MutantsContainer : IMutantsContainer
    {
        private readonly BetterObservableCollection<MutationSession> _generatedMutants;

        private readonly IOperatorsManager _operatorsManager;

        private readonly ITypesManager _typesManager;

        private readonly IVisualStudioConnection _visualStudio;

        public MutantsContainer(
            IOperatorsManager operatorsManager,
            ITypesManager typesManager,
            IVisualStudioConnection visualStudio
            )
        {
            _operatorsManager = operatorsManager;
            _typesManager = typesManager;
            _visualStudio = visualStudio;

            _generatedMutants = new BetterObservableCollection<MutationSession>();
        }

        public string SessionsFile
        {
            get
            {
                string path = _visualStudio.GetMutantsRootFolderPath();
                return Path.Combine(path, "mutants.xml");
            }
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



        public void GenerateMutants()
        {
            IEnumerable<TypeDefinition> types = _typesManager.GetIncludedTypes();

            //  var man = new SessionsManager();

            IEnumerable<OperatorNode> operators = _operatorsManager.GetActiveOperators();

            foreach (OperatorNode mutationOperator in operators)
            {
                mutationOperator.Operator.Mutate(types);
            }

            SaveSession(operators, types);
        }

        public void DeleteSession(MutationSession session)
        {
            _generatedMutants.Remove(session);

            SaveSettingsFile();
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
                        foreach (MutationSession session in list)
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

        private void SaveSession(
            IEnumerable<OperatorNode> operators, IEnumerable<TypeDefinition> types)
        {
            IEnumerable<AssemblyDefinition> assemblies =
                types.Select(t => t.Module.Assembly).Distinct();
            string name = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");

            var session = new MutationSession
            {
                Name = name,
                UsedOperators = operators.Select(o => o.Name).ToList(),
                MutatedTypes = types.Select(t => t.Name).ToList(),
                Assemblies = new List<string>()
            };

            string path = _visualStudio.GetMutantsRootFolderPath();
            string dir = Path.Combine(path, name);
            Directory.CreateDirectory(dir);
            foreach (AssemblyDefinition assemblyDefinition in assemblies)
            {
                
                
                string file = Path.Combine(dir, assemblyDefinition.Name.Name + ".dll");
                assemblyDefinition.Write(file);
                session.Assemblies.Add(file);
            }
            foreach (var referenced in _visualStudio.GetReferencedAssemblies())
            {
             
                File.Copy(referenced, dir+@"\"+ Path.GetFileName(referenced));
            }


            _generatedMutants.Add(session);

           
            SaveSettingsFile();
        }


        private void SaveSettingsFile()
        {
            var ser = new XmlSerializer(typeof(List<MutationSession>));

            using (var file = new StreamWriter(SessionsFile))
            {
                ser.Serialize(file, _generatedMutants.ToList());
            }
        }


    }
}