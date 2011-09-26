namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Mutations;
    using VisualMutator.ViewModels;

    public class MutantsManagementController : Controller
    {
        private readonly MutantsManagementViewModel _viewModel;

        private readonly IMutantsContainer _mutantsContainer;

        private UnitTestsController _unitTestsController;

        public MutantsManagementController( 
            MutantsManagementViewModel viewModel,
            IMutantsContainer mutantsContainer
            )
        {
            _viewModel = viewModel;
            _mutantsContainer = mutantsContainer;

            _viewModel.CommandRemove = new BasicCommand(RemoveMutant);
            _viewModel.CommandRemoveAll = new BasicCommand(RemoveAll);
            _viewModel.Show();
        }


        public void Initialize(UnitTestsController unitTestsController)
        {
            _unitTestsController = unitTestsController;
        }

        public void RemoveAll()
        {
            _unitTestsController.DeleteAllMutants();
            
        }

        public void RemoveMutant()
        {
            _unitTestsController.DeleteMutant(_viewModel.SelectedMutant);
        }

    }
}