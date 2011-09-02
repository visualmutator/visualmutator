namespace PiotrTrzpil.VisualMutator_VSPackage.Model
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using EnvDTE;

    using EnvDTE80;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.Win32;

    #endregion

    public interface IVisualStudioConnection
    {
        SolutionEvents SolutionEvents { get; }

        string InstallPath { get; }

        IEnumerable<string> GetProjectPaths();

        string GetMutantsRootFolderPath();

        string Test();

        IEnumerable<string> GetReferencedAssemblies();
    }

    public class VisualStudioConnection : IVisualStudioConnection
    {
        private readonly DTE2 _dte;

        private readonly SolutionEvents _solutionEvents;

        public VisualStudioConnection()
        {
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            _solutionEvents = ((Events2)_dte.Events).SolutionEvents;
          //  _dte.
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
                         && p.ConfigurationManager.ActiveConfiguration.IsBuildable);

            foreach (Project project in chosenProjects)
            {
                IEnumerable<Property> properties = project.Properties.Cast<Property>();

                var localPath = (string)properties
                                            .Single(prop => prop.Name == "LocalPath").Value;
                var outputFileName = (string)properties
                                                 .Single(prop => prop.Name == "OutputFileName").
                                                 Value;

                var outputPath = (string)project.ConfigurationManager
                                             .ActiveConfiguration.Properties.Cast<Property>()
                                             .Single(prop => prop.Name == "OutputPath").Value;

                yield return Path.Combine(localPath, outputPath, outputFileName);
            }
        }
        public IEnumerable<string> GetReferencedAssemblies()
        {
            var projects = GetProjectPaths();
            string binDir = Path.GetDirectoryName(projects.First());
            return Directory.GetFiles(binDir, "*.dll", SearchOption.AllDirectories).Where(p => !projects.Contains(p));
        }

        public string GetMutantsRootFolderPath()
        {
            var slnPath =
                (string)
                _dte.Solution.Properties.Cast<Property>().Single(p => p.Name == "Path").Value;
            return Directory.GetParent(slnPath).CreateSubdirectory("visal_mutator_mutants").FullName;
        }

        public string Test()
        {
            if (_dte.Solution.IsOpen)
            {
                var sb = new StringBuilder();
                foreach (Property pro in _dte.Solution.Properties.Cast<Property>())
                {
                    try
                    {
                        sb.AppendLine(pro.Name + " --- " + pro.Value);
                    }
                    catch (Exception)
                    {
                    }
                }
                return sb.ToString();
            }
            return "";
        }
    }
}