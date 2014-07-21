namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests;
    using UsefulTools.Switches;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public class MutantDetailsController : Controller
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MutantDetailsViewModel _viewModel;
        private readonly ITestsContainer _testsContainer;
        private readonly ICodeVisualizer _codeVisualizer;
        private readonly IMutantsCache _mutantsCache;
        private Mutant _currentMutant;
        private IDisposable _langObs;
        private IDisposable _tabObs;

        public MutantDetailsController(
            MutantDetailsViewModel viewModel, 
            ITestsContainer testsContainer, 
            ICodeVisualizer codeVisualizer,
            IMutantsCache mutantsCache)
        {
            _viewModel = viewModel;
            _testsContainer = testsContainer;
            _codeVisualizer = codeVisualizer;
            _mutantsCache = mutantsCache;
        }
        public void Initialize()
        {
            _tabObs = _viewModel.WhenPropertyChanged(_ => _.SelectedTabHeader)
                .Where(header => _currentMutant != null)
                .Subscribe(LoadData);

            _langObs = _viewModel.WhenPropertyChanged(_ => _.SelectedLanguage)
                .Subscribe(LoadCode);
        }

        public void LoadDetails(Mutant mutant)
        {
            _log.Debug("LoadDetails in object: " + ToString() + GetHashCode());
            _currentMutant = mutant;

            LoadData(_viewModel.SelectedTabHeader);

        }
        public void CleanDetails()
        {
            _log.Debug("CleanDetails in object: " + ToString() + GetHashCode());

            _viewModel.IsCodeLoading = false;
            _viewModel.ClearCode();
            _viewModel.TestNamespaces.Clear();

        }
        public void LoadData(string header)
        {
            FunctionalExt.Switch(header)
                .Case("Tests", () => LoadTests(_currentMutant))
                .Case("Code", () => LoadCode(_viewModel.SelectedLanguage))
                .ThrowIfNoMatch();   
        }

  

        public async void LoadCode(CodeLanguage selectedLanguage)
        {
            _viewModel.IsCodeLoading = true;
            _viewModel.ClearCode();

            var mutant = _currentMutant;

            if(mutant != null)
            {
                MutationResult mutationResult = await _mutantsCache.GetMutatedModulesAsync(mutant);
                CodeWithDifference diff = await _codeVisualizer.CreateDifferenceListing(selectedLanguage,
                    mutant, mutationResult);
                if (diff != null)
                {
                    _viewModel.PresentCode(diff);
                    _viewModel.IsCodeLoading = false;
                }
            }
            
        }

    
        private async void LoadTests(Mutant mutant)
        {
            _viewModel.TestNamespaces.Clear();

           
            
            if (mutant.MutantTestSession.IsComplete)
            {
                var namespaces = await Task.Run(() => _testsContainer.CreateMutantTestTree(mutant));
                _viewModel.TestNamespaces.AddRange(namespaces);
            }
            else
            {
                //_listenerForCurrentMutant = mutant.TestSession.WhenPropertyChanged(_ => _.IsComplete)
                //    .Subscribe(x => _viewModel.TestNamespaces.AddRange(mutant.TestSession.TestNamespaces));
            }
        }


        public MutantDetailsViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public void Clean()
        {
            _currentMutant = null;
           
            _viewModel.IsCodeLoading = false;
            _viewModel.TestNamespaces.Clear();
            _viewModel.SelectedLanguage = CodeLanguage.CSharp;
            _viewModel.ClearCode();
            _langObs.Dispose();
            _tabObs.Dispose();

        }

       
    }
}