namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
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

        private IDisposable _listenerForCurrentMutant;

        private AssembliesProvider _currentOriginalAssemblies;

        private Mutant _currentMutant;

        public MutantDetailsController(
            MutantDetailsViewModel viewModel, 
            ICodeDifferenceCreator codeDifferenceCreator,
            CommonServices commonServices)
        {
            _viewModel = viewModel;
            _codeDifferenceCreator = codeDifferenceCreator;
            _commonServices = commonServices;

            _viewModel.RegisterPropertyChanged(_=>_.SelectedTabHeader)
                .Where(x=> _currentMutant != null).Subscribe(LoadData);

            _viewModel.RegisterPropertyChanged(_ => _.SelectedLanguage).Subscribe(LoadCode);

        }
        public void LoadDetails(Mutant mutant, AssembliesProvider originalAssemblies)
        {
            _currentOriginalAssemblies = originalAssemblies;
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

  

        public void LoadCode(CodeLanguage selectedLanguage)
        {
            _viewModel.IsCodeLoading = true;
            _viewModel.ClearCode();

            var mutant = _currentMutant;
            var assemblies = _currentOriginalAssemblies;
            _commonServices.Threading.ScheduleAsync(
                () =>
                {
                    if (mutant == null)
                    {
                        return null;
                    }
                    return _codeDifferenceCreator.CreateDifferenceListing(selectedLanguage,
                        mutant, assemblies);
                   
                },
                code =>
                {
                    if(code != null)
                    {
                        _viewModel.PresentCode(code);
                        _viewModel.IsCodeLoading = false;
                    }
                   
                });
           
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
            _currentOriginalAssemblies = null;
            _viewModel.IsCodeLoading = false;
            _viewModel.TestNamespaces.Clear();
            _viewModel.SelectedLanguage = CodeLanguage.CSharp;
            _viewModel.ClearCode();

        }
    }
}