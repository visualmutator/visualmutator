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
    using VisualMutator.Model.Mutations;

    public class MutantDetailsController : Controller
    {
        private readonly MutantDetailsViewModel _viewModel;



        private readonly ICodeDifferenceCreator _codeDifferenceCreator;

        private readonly Services _services;

        private IDisposable _listenerForCurrentMutant;

        private IList<AssemblyDefinition> _currentOriginalAssemblies;

        private Mutant _currentMutant;

        public MutantDetailsController(
            MutantDetailsViewModel viewModel, 
            ICodeDifferenceCreator codeDifferenceCreator,
            Services services)
        {
            _viewModel = viewModel;

            _viewModel.RegisterPropertyChanged(() => _viewModel.SelectedTabHeader).Subscribe(LoadData);

           
            _codeDifferenceCreator = codeDifferenceCreator;
            _services = services;
        }

        public void LoadData(string header)
        {

            if (_currentMutant != null)
            {
                Functional.Switch(header)
               .Case("Tests", () => LoadTests(_currentMutant))
               .Case("Code", () => LoadCode(_currentMutant, _currentOriginalAssemblies))
               .ThrowIfNoMatch();
            }
           
        }

        public void LoadDetails(Mutant mutant, IList<AssemblyDefinition> originalAssemblies)
        {
            _currentOriginalAssemblies = originalAssemblies;
            _currentMutant = mutant;

            LoadData(_viewModel.SelectedTabHeader);
    
        }

        public void LoadCode(Mutant mutant, IList<AssemblyDefinition> originalAssemblies)
        {
//TODO: remove race
            _viewModel.IsCodeLoading = true;
            _viewModel.ClearCode();
            _services.Threading.ScheduleAsync(
                () => _codeDifferenceCreator.CreateCodeDifference(mutant, originalAssemblies),
                code =>
                {
                    _viewModel.PresentCode(code);
                    _viewModel.IsCodeLoading = false;
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

            if (mutant.TestSession != null)
            {
                _viewModel.TestNamespaces.AddRange(mutant.TestSession.TestNamespaces);
            }
            else
            {
                _listenerForCurrentMutant = mutant.WhenPropertyChanged(() => mutant.TestSession)
                    .Subscribe(testSession => _viewModel.TestNamespaces.AddRange(testSession.TestNamespaces));
            }
        }


        public MutantDetailsViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

      
    }
}