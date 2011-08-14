namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    using System;
    using System.Collections.Generic;

    using System.Linq;
    using System.Text;
    using System.Waf.Applications;
    using System.Windows;

    using EnvDTE;

    using Ninject;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;

    using VisualMutator.Domain;

    public class ILMutationsController : Controller
    {
        private readonly ILMutationsViewModel _viewModel;

        private readonly IVisualStudioConnection _visualStudio;

        private readonly IMutantGenerator _mutantGenerator;

        private readonly ITypesManager _typesManager;

        private readonly IOperatorsManager _operatorsManager;

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

            var paths = _visualStudio.GetProjectPaths();

             _typesManager.RefreshTypes(paths);


            _operatorsManager.LoadOperators();

        }

        public ILMutationsViewModel ILMutationsVm
        {
            get
            {
                return _viewModel;
            }

        }

        public ObservableCollection<MutationSession> GeneratedMutants
        {
            get
            {
                return _mutantGenerator.GeneratedMutants;
            }
        }

        
    }
}