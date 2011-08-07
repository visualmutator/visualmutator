namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using EnvDTE;

    using Microsoft.VisualStudio.Shell;
    public interface IVisualStudioConnection
    {
        IEnumerable<string> GetProjectPaths();

        string CreateMutantsRootFolderPath();
        string Test();
    }
    public class VisualStudioConnection : IVisualStudioConnection
    {
        private DTE dte;

        public VisualStudioConnection()
        {
            dte = (DTE)Package.GetGlobalService(typeof(DTE));
        }

        public IEnumerable<string> GetProjectPaths()
        {
            var chosenProjects = dte.Solution.Cast<Project>()
                .Where(
                    p => p.ConfigurationManager != null
                         && p.ConfigurationManager.ActiveConfiguration != null
                         && p.ConfigurationManager.ActiveConfiguration.IsBuildable);

            foreach (var project in chosenProjects)
            {
                var properties = project.Properties.Cast<Property>();

                string localPath = (string)properties
                                               .Single(prop => prop.Name == "LocalPath").Value;
                string outputFileName = (string)properties
                                                    .Single(prop => prop.Name == "OutputFileName").Value;

                string outputPath = (string)project.ConfigurationManager
                                                .ActiveConfiguration.Properties.Cast<Property>()
                                                .Single(prop => prop.Name == "OutputPath").Value;

                yield return Path.Combine(localPath, outputPath, outputFileName);
            }
        }
        public string CreateMutantsRootFolderPath()
        {
            string slnPath = (string)dte.Solution.Properties.Cast<Property>().Single(p => p.Name == "Path").Value;
            return Directory.GetParent(slnPath).CreateSubdirectory("visal_mutator_mutants").FullName;
        }

        public string Test()
        {
            if (dte.Solution.IsOpen)
            {
                var sb = new StringBuilder();
                foreach (var pro in dte.Solution.Properties.Cast<Property>())
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