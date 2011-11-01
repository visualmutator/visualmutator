namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
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

        private MutationTestingSession _currentSession;

        public ResultsSavingController(ResultsSavingViewModel viewModel, IFileSystem fs )
        {
            _viewModel = viewModel;
            _fs = fs;

            _viewModel.CommandSaveResults = new BasicCommand(SaveResults);
            _viewModel.CommandClose = new BasicCommand(Close);
        }

        public void Run(MutationTestingSession currentSession)
        {
            _currentSession = currentSession;
            _viewModel.Show();


            //    _viewModel.TargetPath



        }
        public void SaveResults()
        {

            var generator = new XmlResultsGenerator();

            XDocument document = generator.GenerateResults(_currentSession, _viewModel.IncludeDetailedTestResults);

            using (var writer = _fs.File.CreateText(_viewModel.TargetPath))
            {
                writer.Write(document.ToString());
            }
            

            _viewModel.Close();
        }
        public void Close()
        {
            _viewModel.Close();
        }

       







    }
}