namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Comparers;
    using CommonUtilityInfrastructure.WpfUtils;

    using ICSharpCode.Decompiler;

    using ICSharpCode.ILSpy;

    using Mono.Cecil;

    using VisualMutator.Model;
    using VisualMutator.Model.CodeDifference;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.ViewModels;

    public class MutantDetailsController : Controller
    {
        private readonly MutantDetailsViewModel _viewModel;

        private readonly ICodeDifferenceCreator _codeDifferenceCreator;


        private readonly CommonServices _commonServices;

        private readonly IMutantsContainer _mutantsContainer;

        private IDisposable _listenerForCurrentMutant;

       

        private Mutant _currentMutant;
        private MutationTestingSession _session;

        public MutantDetailsController(
            MutantDetailsViewModel viewModel, 
            ICodeDifferenceCreator codeDifferenceCreator,
            IMutantsContainer mutantsContainer,
            CommonServices commonServices)
        {
            _viewModel = viewModel;
            _codeDifferenceCreator = codeDifferenceCreator;
            _mutantsContainer = mutantsContainer;

            _commonServices = commonServices;

            _viewModel.RegisterPropertyChanged(_=>_.SelectedTabHeader)
                .Where(x=> _currentMutant != null).Subscribe(LoadData);

            _viewModel.RegisterPropertyChanged(_ => _.SelectedLanguage).Subscribe(LoadCode);

        }
        public void LoadDetails(Mutant mutant, MutationTestingSession session)
        {
            _session = session;
            _currentMutant = mutant;

            LoadData(_viewModel.SelectedTabHeader);

        }
        public void LoadData(string header)
        {
            Functional.Switch(header)
                .Case("Tests", () => LoadTests(_currentMutant))
                .Case("Code", () => LoadCode(_viewModel.SelectedLanguage))
                .ThrowIfNoMatch();   
        }

  

        public async void LoadCode(CodeLanguage selectedLanguage)
        {
            _viewModel.IsCodeLoading = true;
            _viewModel.ClearCode();

            var mutant = _currentMutant;
            var assemblies = _session.OriginalAssemblies;

            if(mutant != null)
            {
                CodeWithDifference diff = await GetCodeWithDifference(selectedLanguage, mutant, assemblies);
                if (diff != null)
                {
                    _viewModel.PresentCode(diff);
                    _viewModel.IsCodeLoading = false;
                }
            }
         
           
        }

        private async Task<CodeWithDifference> GetCodeWithDifference(CodeLanguage selectedLanguage, Mutant mutant, AssembliesProvider assemblies)
        {
            if (mutant.MutatedModules == null)
            {
                _mutantsContainer.ExecuteMutation(mutant, _session.StoredSourceAssemblies.Modules,
                    _session.SelectedTypes.ToList(), ProgressCounter.Inactive());
            }
            return _codeDifferenceCreator.CreateDifferenceListing(selectedLanguage,
                mutant, assemblies);
        }

        private void LoadTests(Mutant mutant)
        {
            _viewModel.TestNamespaces.Clear();

            if (_listenerForCurrentMutant != null)
            {
                _listenerForCurrentMutant.Dispose();
                _listenerForCurrentMutant = null;
            }

            if (mutant.MutantTestSession.IsComplete)
            {
                _viewModel.TestNamespaces.AddRange(mutant.MutantTestSession.TestNamespaces);
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

        }
    }
}