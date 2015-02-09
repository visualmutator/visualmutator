namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Microsoft.Win32;
    using Model;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;
    using UsefulTools.Wpf;
    using ViewModels;
    using Assembly = System.Reflection.Assembly;

    #endregion

    public class OptionsController : Controller
    {
        private readonly OptionsViewModel _viewModel;
        private readonly CommonServices _svc;
        private readonly IOptionsManager _optionsManager;

      
        public OptionsController(
            OptionsViewModel viewModel, 
            CommonServices svc,
            IOptionsManager optionsManager)
        {
            _viewModel = viewModel;
            _svc = svc;
            _optionsManager = optionsManager;

            _viewModel.CommandSave = new SmartCommand(SaveResults);
            _viewModel.CommandClose = new SmartCommand(Close);
        }
        
        public void Run()
        {
            OptionsModel optionsModel = _optionsManager.ReadOptions();
            _viewModel.Options = optionsModel;
            _viewModel.Options.ParsedParams = null;
            _viewModel.Show();
        }

        public void SaveResults()
        {
            try
            {
                
                bool ok = _viewModel.Options.ParsedParams != null;
                _optionsManager.WriteOptions(_viewModel.Options);
                _viewModel.Close();
            }
            catch (Exception e)
            {
                _svc.Logging.ShowError("Params are incorrect: "+ e);
            }
            
        }

        public void Close()
        {
            _viewModel.Close();
        }

        public OptionsViewModel ViewModel
        {
            get { return _viewModel; }
        }
    }
}