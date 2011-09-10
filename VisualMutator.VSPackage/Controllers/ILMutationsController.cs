namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using System.Collections.Generic;


    using EnvDTE;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
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

            _viewModel.CommandMutate = new BasicCommand(Mutate);
            _viewModel.CommandRefresh = new BasicCommand(Refresh);

            _viewModel.Assemblies = new BetterObservableCollection<AssemblyNode>();
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
          //  _viewModel.IsVisible = true;
            _visualStudio.BuildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(BuildEvents_OnBuildDone);
       
            _visualStudio.SolutionEvents.ProjectAdded += HandleProjectAdded;

            _viewModel.IsVisible = true;
            _mutantsContainer.LoadSessions();
            Refresh();
        }

        void BuildEvents_OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            
        }

        public void Deactivate()
        {
            _viewModel.IsVisible = false;
            _viewModel.Assemblies.Clear();
            _mutantsContainer.Clear();
        }


        private void HandleProjectAdded(Project project)
        {
            Refresh();
        }

        public void Mutate()
        {
            Refresh();
            _mutantsContainer.GenerateMutants("Mutant");
        }

        public void Refresh()
        {
            //  MessageBox.Show(_visualStudio.Test());

            IEnumerable<string> paths = _visualStudio.GetProjectPaths();

            var assemblies = _typesManager.BuildTypesTree(paths);
            _viewModel.Assemblies.ReplaceRange(assemblies);


            _operatorsManager.LoadOperators();
        }

        
    }
}