namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using System.Collections.Generic;
    using System.Waf.Applications;

    using EnvDTE;

    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;

    using Controller = PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Controller;

    #endregion

    public class ILMutationsController : Controller
    {
        private readonly IMutantGenerator _mutantGenerator;

        private readonly IOperatorsManager _operatorsManager;

        private readonly ITypesManager _typesManager;

        private readonly ILMutationsViewModel _viewModel;

        private readonly IVisualStudioConnection _visualStudio;

        public ILMutationsController(
            ILMutationsViewModel viewModel,
            IVisualStudioConnection visualStudio,
            IMutantGenerator mutantGenerator,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager
            )
        {
            _viewModel = viewModel;
            _visualStudio = visualStudio;
            _mutantGenerator = mutantGenerator;
            _typesManager = typesManager;
            _operatorsManager = operatorsManager;

            _viewModel.CommandMutate = new DelegateCommand(Mutate);
            _viewModel.CommandRefresh = new DelegateCommand(Refresh);

            _viewModel.Assemblies = _typesManager.Assemblies;
            _viewModel.MutationPackages = _operatorsManager.OperatorPackages;
        }

        public ILMutationsViewModel ILMutationsVm
        {
            get
            {
                return _viewModel;
            }
        }

        public Infrastructure.ObservableCollection<MutationSession> GeneratedMutants
        {
            get
            {
                return _mutantGenerator.GeneratedMutants;
            }
        }

        public void Initialize()
        {
            _viewModel.IsVisible = true;
            _visualStudio.SolutionEvents.Opened += Activate;
            _visualStudio.SolutionEvents.AfterClosing += Deactivate;
            _visualStudio.SolutionEvents.ProjectAdded += HandleProjectAdded;
        }

        private void Activate()
        {
            _viewModel.IsVisible = true;
            _mutantGenerator.LoadSessions();
            Refresh();
        }

        private void Deactivate()
        {
            _viewModel.IsVisible = false;
            _viewModel.Assemblies.Clear();
        }

        private void HandleProjectAdded(Project project)
        {
            Refresh();
        }

        public void Mutate()
        {
            Refresh();
            _mutantGenerator.GenerateMutants();
        }

        public void Refresh()
        {
            //  MessageBox.Show(_visualStudio.Test());

            IEnumerable<string> paths = _visualStudio.GetProjectPaths();

            _typesManager.RefreshTypes(paths);

            _operatorsManager.LoadOperators();
        }
    }
}