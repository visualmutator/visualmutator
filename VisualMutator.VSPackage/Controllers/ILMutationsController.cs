namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Waf.Applications;
    using System.Windows;

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


        public ILMutationsController(
            ILMutationsViewModel viewModel,
            IVisualStudioConnection visualStudio,
            IMutantGenerator mutantGenerator,
            ITypesManager typesManager
            )
        {
            _viewModel = viewModel;
            _visualStudio = visualStudio;
            _mutantGenerator = mutantGenerator;
            _typesManager = typesManager;
     
            _viewModel.CommandMutate = new DelegateCommand(Mutate);
            _viewModel.CommandRefresh = new DelegateCommand(Refresh);

            _viewModel.Assemblies = _typesManager.Assemblies;
            _viewModel.MutationPackages = mutantGenerator.OperatorsManager.OperatorPackages;
        }

        public void Mutate()
        {
            Refresh();
            _mutantGenerator.GenerateMutants();
        }

        public void Refresh()
        {
            MessageBox.Show(_visualStudio.Test());

            //var paths = _visualStudio.GetProjectPaths();

            // _typesManager.RefreshTypes(paths);
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

        public void Initialize()
        {
            //Refresh();
        }
    }
}