namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure.FileSystem;
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using VisualMutator.ViewModels;

    public class ResultsSavingController : Controller
    {
        private readonly ResultsSavingViewModel _viewModel;

        private readonly IFileSystem _fs;

        private readonly XmlResultsGenerator _generator;

        private MutationTestingSession _currentSession;

        public ResultsSavingController(
            ResultsSavingViewModel viewModel, 
            IFileSystem fs,
            XmlResultsGenerator generator)
        {
            _viewModel = viewModel;
            _fs = fs;
            _generator = generator;

            _viewModel.CommandSaveResults = new BasicCommand(SaveResults);
            _viewModel.CommandClose = new BasicCommand(Close);
            _viewModel.CommandBrowse = new BasicCommand(BrowsePath);
        }

        public void BrowsePath()
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "MutationResults", 
                DefaultExt = ".xml", 
                Filter = "XML documents (.xml)|*.xml"
            };
    

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                _viewModel.TargetPath = dlg.FileName;
    
            }


        }
        public void Run(MutationTestingSession currentSession)
        {
            _currentSession = currentSession;
            _viewModel.Show();

        }
        public void SaveResults()
        {

            XDocument document = _generator.GenerateResults(_currentSession, 
                _viewModel.IncludeDetailedTestResults, _viewModel.IncludeCodeDifferenceListings);

            using (var writer = _fs.File.CreateText(_viewModel.TargetPath))
            {
                writer.Write(document.ToString());
            }
            

            _viewModel.Close();

            
            var p =new Process();

            p.StartInfo.FileName = _viewModel.TargetPath;
            p.Start();
        }
        public void Close()
        {
            _viewModel.Close();
        }

       







    }
}