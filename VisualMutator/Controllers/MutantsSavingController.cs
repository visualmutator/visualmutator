namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using Model;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Wpf;
    using ViewModels;
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

    #endregion

    public class MutantsSavingController : Controller
    {
        private readonly MutantsSavingViewModel _viewModel;

        private readonly IFileSystem _fs;

        private readonly CommonServices _svc;



        public MutantsSavingController(
            MutantsSavingViewModel viewModel, 
            IFileSystem fs,
            CommonServices svc)
        {
            _viewModel = viewModel;
            _fs = fs;
            _svc = svc;

            _viewModel.CommandSaveResults = new SmartCommand(SaveResults);

            _viewModel.CommandClose = new SmartCommand(Close);
            _viewModel.CommandBrowse = new SmartCommand(BrowseForMutantsFolder);
            
        }

        
        public void Run()
        {
            _viewModel.Show();

        }

        public void BrowseForMutantsFolder()
        {
            var dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                _viewModel.TargetPath = dlg.SelectedPath;
            }
        }
      
        public void SaveResults()
        {
            var targetPath = _viewModel.TargetPath;
            if (!string.IsNullOrEmpty(targetPath)
                && Path.IsPathRooted(targetPath))
            {
                if (!_svc.FileSystem.Directory.Exists(targetPath))
                {
                    try
                    {
                        _svc.FileSystem.Directory.CreateDirectory(targetPath);
                    }
                    catch (Exception)
                    {
                        _svc.Logging.ShowError("Could not create directory.", view: _viewModel.View);
                        return;
                    }
                }


                if (_svc.FileSystem.Directory.GetDirectories(targetPath).Length == 0
                    && _svc.FileSystem.Directory.GetFiles(targetPath).Length == 0)
                {
                    Result = targetPath;
                    _viewModel.Close();
                }
                else
                {
                    _svc.Logging.ShowError("Selected directory is not empty. Select empty directory.",
                                           view: _viewModel.View);
                }
            }
            else
            {
                _svc.Logging.ShowError("Selected path is invalid", view: _viewModel.View);
            }
            
        }

        public string Result { get; set; }

        public void Close()
        {
            _viewModel.Close();
        }

       







    }
}