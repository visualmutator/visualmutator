namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Views;

    public class MutantDetailsController : Controller
    {
        private readonly MutantDetailsViewModel _viewModel;

        private IDisposable _listenerForCurrentMutant;

        public MutantDetailsController(MutantDetailsViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        public void LoadDetails(Mutant mutant)
        {
            LoadTests(mutant);

            


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

    public class MutantDetailsViewModel: ViewModel<IMutantDetailsView>
    {
        public MutantDetailsViewModel(IMutantDetailsView view)
            : base(view)
        {
            TestNamespaces = new BetterObservableCollection<TestNodeNamespace>();
        }


        private BetterObservableCollection<TestNodeNamespace> _testNamespaces;

        public BetterObservableCollection<TestNodeNamespace> TestNamespaces
        {
            get
            {
                return _testNamespaces;
            }
            set
            {
                SetAndRise(ref _testNamespaces, value, () => TestNamespaces);
            }
        }
       

    }
}