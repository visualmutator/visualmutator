namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;
    using log4net;
    using Microsoft.Win32;
    using Model;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public class ResultsSavingController : Controller
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ResultsSavingViewModel _viewModel;

        private readonly IFileSystem _fs;

        private readonly CommonServices _svc;

        private readonly XmlResultsGenerator _generator;

        private MutationTestingSession _currentSession;

        public ResultsSavingController(
            ResultsSavingViewModel viewModel, 
            IFileSystem fs,
            CommonServices svc,
            XmlResultsGenerator generator)
        {
            _viewModel = viewModel;
            _fs = fs;
            _svc = svc;
            _generator = generator;

            _viewModel.CommandSaveResults = new SmartCommand(() => SaveResults());

            _viewModel.CommandClose = new SmartCommand(Close);
            _viewModel.CommandBrowse = new SmartCommand(BrowsePath);

            if (_svc.Settings.ContainsKey("MutationResultsFilePath"))
            {
                _viewModel.TargetPath = _svc.Settings["MutationResultsFilePath"];
            }
           
        }

        public ResultsSavingViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public void Run(MutationTestingSession currentSession)
        {
            _currentSession = currentSession;
            _viewModel.Show();

        }
        public void BrowsePath()
        {

            SaveFileDialog dlg = new SaveFileDialog
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
        public void SaveResults(string path = null)
        {
            if(path == null)
            {
                if (string.IsNullOrEmpty(_viewModel.TargetPath)
                || !Path.IsPathRooted(_viewModel.TargetPath))
                {
                    _svc.Logging.ShowError("Invalid path");
                    return;
                }
                path = _viewModel.TargetPath;
            }

            try
            {
                XDocument document = _generator.GenerateResults(_currentSession,
                _viewModel.IncludeDetailedTestResults, _viewModel.IncludeCodeDifferenceListings);
                try
                {

                    using (var writer = _fs.File.CreateText(path))
                    {
                        writer.Write(document.ToString());
                    }
                    _svc.Settings["MutationResultsFilePath"] = path;

                    _viewModel.Close();


                    var p = new Process();

                    p.StartInfo.FileName = path;
                    p.Start();
                }
                catch (IOException)
                {
                    _svc.Logging.ShowError("Cannot write file: " + path);
                }
            }
            catch (Exception e)
            {
                _svc.Logging.ShowError(e);
                throw;
            }
            
            
        }
        public void Close()
        {
            _viewModel.Close();
        }

       







    }
}