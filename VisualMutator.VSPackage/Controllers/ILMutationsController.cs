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
        private readonly IMutantsContainer _mutantsContainer;

        private readonly IOperatorsManager _operatorsManager;

        private readonly ITypesManager _typesManager;

        private readonly ILMutationsViewModel _viewModel;

        private readonly IVisualStudioConnection _visualStudio;

        public ILMutationsController(
            ILMutationsViewModel viewModel,
            IVisualStudioConnection visualStudio,
            IMutantsContainer mutantsContainer,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager
            )
        {
            _viewModel = viewModel;
            _visualStudio = visualStudio;
            _mutantsContainer = mutantsContainer;
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


        public void Initialize()
        {
            _viewModel.IsVisible = true;
            _visualStudio.SolutionEvents.Opened += ActivateOnSolutionOpened;
            _visualStudio.SolutionEvents.AfterClosing += DeactivateOnSolutionClosed;
            _visualStudio.SolutionEvents.ProjectAdded += HandleProjectAdded;
        }

        private void ActivateOnSolutionOpened()
        {
            _viewModel.IsVisible = true;
            _mutantsContainer.LoadSessions();
            Refresh();
        }

        private void DeactivateOnSolutionClosed()
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
            _mutantsContainer.GenerateMutants();
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