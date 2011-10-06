namespace PiotrTrzpil.VisualMutator_VSPackage.Model
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;

    using System.Linq;
    using System.Reflection;

    using EnvDTE;

    using EnvDTE80;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.Win32;

    using VisualMutator.Infrastructure;

    using log4net;

    #endregion


    public class VisualStudioConnection : IVisualStudioConnection
    {
        private readonly DTE2 _dte;

        private readonly SolutionEvents _solutionEvents;

        private BuildEvents _buildEvents;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool IsSolutionOpen
        {
            get
            {
                return _dte.Solution.IsOpen;
            }
        }

        public VisualStudioConnection()
        {
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            _solutionEvents = ((Events2)_dte.Events).SolutionEvents;
            _buildEvents = ((Events2)_dte.Events).BuildEvents;

          //  _dte.Solution.Projects.
            //  _dte.
           
        }

        public void Initialize()
        {
            _buildEvents.OnBuildBegin += _buildEvents_OnBuildBegin;
            _buildEvents.OnBuildDone +=_buildEvents_OnBuildDone;

            _solutionEvents.Opened += _solutionEvents_Opened;
            _solutionEvents.AfterClosing += _solutionEvents_AfterClosing;
         
        }

        void _solutionEvents_AfterClosing()
        {
            _log.Info("Solution closed.");
            OnSolutionAfterClosing();
        }

        void _solutionEvents_Opened()
        {
            _log.Info("Solution opened.");
            OnSolutionOpened();
        }

        void _buildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            _log.Info("Build begin.");
            OnOnBuildDone();
        }

        void _buildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            _log.Info("Build done.");
            OnOnBuildBegin();
        }


        public void OpenFile(string className)
        {
            IEnumerable<ProjectItem> projectItems = _dte.Solution.Cast<Project>()
                .SelectMany(p => p.ProjectItems.Cast<ProjectItem>()).ToList();
            ProjectItem projectItem = projectItems.First(i => i.Name == className);
        }

        public BuildEvents BuildEvents
        {
            get
            {
                return _buildEvents;
            }
        }

        public SolutionEvents SolutionEvents
        {
            get
            {
                return _solutionEvents;
            }
        }

        public string InstallPath
        {
            get
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\10.0\Setup\VS");
                string vsInstallationPath = regKey.GetValue("ProductDir").ToString();
                regKey.Close();
                return vsInstallationPath;
            }
        }

        public IEnumerable<string> GetProjectPaths()
        {
            IEnumerable<Project> chosenProjects = _dte.Solution.Cast<Project>()
                .Where(
                    p => p.ConfigurationManager != null
                         && p.ConfigurationManager.ActiveConfiguration != null
                         && p.ConfigurationManager.ActiveConfiguration.IsBuildable).ToList();
            var list = new List<string>();
            foreach (Project project in chosenProjects)
            {
                IEnumerable<Property> properties = project.Properties.Cast<Property>().ToList();

                var localPath = (string)properties
                                            .Single(prop => prop.Name == "LocalPath").Value;
                var outputFileName = (string)properties
                                                 .Single(prop => prop.Name == "OutputFileName").
                                                 Value;

                var outputPath = (string)project.ConfigurationManager
                                             .ActiveConfiguration.Properties.Cast<Property>()
                                             .Single(prop => prop.Name == "OutputPath").Value;

                list.Add(Path.Combine(localPath, outputPath, outputFileName));
            }
            return list;
        }
        public IEnumerable<string> GetReferencedAssemblies()
        {
            var projects = GetProjectPaths().ToList();
           // string binDir = Path.GetDirectoryName(projects.First());
            var list = new List<string>();
            foreach (var binDir in projects.Select(p => Path.GetDirectoryName(p)))
            {
                var files = Directory.GetFiles(binDir, "*.dll", SearchOption.AllDirectories)
                    .Where(p => !projects.Contains(p));
                list.AddRange(files);
            }
            return list;
        }

        public event Action OnBuildBegin;

        public void OnOnBuildBegin()
        {
            Action handler = OnBuildBegin;
            if (handler != null)
            {
                handler();
            }
        }

        public event Action OnBuildDone;

        public void OnOnBuildDone()
        {
            Action handler = OnBuildDone;
            if (handler != null)
            {
                handler();
            }
        }

        public event Action SolutionOpened;

        public void OnSolutionOpened()
        {
            Action handler = SolutionOpened;
            if (handler != null)
            {
                handler();
            }
        }

        public event Action SolutionAfterClosing;

        public void OnSolutionAfterClosing()
        {
            Action handler = SolutionAfterClosing;
            if (handler != null)
            {
                handler();
            }
        }

        public string GetMutantsRootFolderPath()
        {
            var slnPath =
                (string)
                _dte.Solution.Properties.Cast<Property>().Single(p => p.Name == "Path").Value;
            return Directory.GetParent(slnPath).CreateSubdirectory("visal_mutator_mutants").FullName;
        }
    }
}