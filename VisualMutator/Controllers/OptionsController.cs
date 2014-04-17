namespace VisualMutator.Controllers
{
    #region

    using System.Diagnostics;
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Microsoft.Cci.Ast;
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
        private readonly IOptionsManager _optionsManager;

        private readonly IFileSystem _fs;

        private readonly CommonServices _svc;
      
        public OptionsController(
            OptionsViewModel viewModel, 
            IOptionsManager optionsManager,
            IFileSystem fs,
            CommonServices svc)
        {
            _viewModel = viewModel;
            _optionsManager = optionsManager;
            _fs = fs;
            _svc = svc;

            _viewModel.CommandSave = new SmartCommand(SaveResults);
            _viewModel.CommandClose = new SmartCommand(Close);
        }
        
        public void Run()
        {
            OptionsModel optionsModel = _optionsManager.ReadOptions();
            _viewModel.Options = optionsModel;
            _viewModel.Show();
        }

        public void SaveResults()
        {
            _optionsManager.WriteOptions(_viewModel.Options);
        }

        public void Close()
        {
            _viewModel.Close();
        }

    }
}