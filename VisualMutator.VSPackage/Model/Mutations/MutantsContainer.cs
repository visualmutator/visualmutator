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

    public class MutantsContainer : IMutantsContainer
    {
        private readonly BetterObservableCollection<MutationSession> _generatedMutants;

        private readonly IOperatorsManager _operatorsManager;

        private readonly ITypesManager _typesManager;

        private readonly IVisualStudioConnection _visualStudio;

        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        private readonly IFactory<DateTime> _dateTimeNowFactory;

        private readonly IDirectory _directory;

        private readonly IFile _file;
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public MutantsContainer(
            IOperatorsManager operatorsManager,
            ITypesManager typesManager,
            IVisualStudioConnection visualStudio,
            IAssemblyReaderWriter assemblyReaderWriter,
            IFactory<DateTime> dateTimeNowFactory,
            IDirectory directory, IFile file
            )
        {
            _operatorsManager = operatorsManager;
            _typesManager = typesManager;
            _visualStudio = visualStudio;
            _assemblyReaderWriter = assemblyReaderWriter;
            _dateTimeNowFactory = dateTimeNowFactory;
            _directory = directory;
            _file = file;

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



        public MutationSession GenerateMutant(string name, Action<string> mutationLog)
        {
            IEnumerable<TypeDefinition> types = _typesManager.GetIncludedTypes();
      

            IEnumerable<OperatorNode> operators = _operatorsManager.GetActiveOperators();

            foreach (OperatorNode mutationOperator in operators)
            {
                mutationLog("Applying operator: " + mutationOperator.Operator.Name);
                mutationOperator.Operator.Mutate(types);
            }



            var session = new MutationSession
            {
                Name = name,
                DateOfCreation = _dateTimeNowFactory.Create(),
                UsedOperators = operators.Select(o => o.Name).ToList(),
                MutatedTypes = types.Select(t => t.Name).ToList(),
                Assemblies = new List<string>(),
            };
            mutationLog("Saving mutant...");
            StoreMutant(session);

            return session;


        }


        public string MutantDirectoryPath(MutationSession mutant)
        {
            string path = _visualStudio.GetMutantsRootFolderPath();
            return Path.Combine(path, mutant.Name + " - " 
                + mutant.DateOfCreation.ToString("dd.MM.yy, HH.mm.ss"));
            
        }


        public void StoreMutant(MutationSession mutant)
        {
            IEnumerable<AssemblyDefinition> assemblies = _typesManager.GetLoadedAssemblies();

            string mutantDirectoryPath = MutantDirectoryPath(mutant);
            _directory.CreateDirectory(mutantDirectoryPath);
            foreach (AssemblyDefinition assemblyDefinition in assemblies)
            {
                string file = Path.Combine(mutantDirectoryPath, assemblyDefinition.Name.Name + ".dll");
                _assemblyReaderWriter.WriteAssembly(assemblyDefinition,file); 
                mutant.Assemblies.Add(file);   
            }
            var refer = _visualStudio.GetReferencedAssemblies();
            foreach (var referenced in refer)
            {
                string destination = Path.Combine(mutantDirectoryPath , Path.GetFileName(referenced));
                _file.Copy(referenced, destination, overwrite:true);
            }
        }




        public void LoadSessions()
        {
            if (File.Exists(SessionsFile))
            {
                var ser = new XmlSerializer(typeof(List<MutationSession>));
                using (var file = new StreamReader(SessionsFile))
                {
                    try
                    {
                        var list = (List<MutationSession>)ser.Deserialize(file);
                        foreach (MutationSession session in list)
                        {
                            _generatedMutants.Add(session);
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        _log.Error("Invalid mutants file.", e);
                    }
                }
            }
        }

        public void DeleteMutant(MutationSession mutant)
        {
            _generatedMutants.Remove(mutant);
            SaveSettingsFile();

            string dir = MutantDirectoryPath(mutant);
            _directory.Delete(dir);

            
        }

        public void Clear()
        {
           _generatedMutants.Clear();
        }

        public void SaveSettingsFile()
        {
            var ser = new XmlSerializer(typeof(List<MutationSession>));

            using (var file = new StreamWriter(SessionsFile))
            {
                ser.Serialize(file, _generatedMutants.ToList());
            }
        }


    }
}