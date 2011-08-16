namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;

    #endregion

    public class ApplicationController
    {
        private readonly ILMutationsController _ilMutationsController;

        private readonly MainWindowViewModel _mainWindowVm;

        private readonly UnitTestsController _unitTestsController;

        private readonly IVisualStudioConnection _visualStudioConnection;

        public ApplicationController(
            MainWindowViewModel mainWindowVm,
            ILMutationsController ilMutationsController,
            UnitTestsController unitTestsController,
            IVisualStudioConnection visualStudioConnection
            )
        {
            _mainWindowVm = mainWindowVm;

            //   var view = new ILMutationsView();
            //  var vm = new ILMutationsViewModel(view);
            _ilMutationsController = ilMutationsController;
            _unitTestsController = unitTestsController;
            _visualStudioConnection = visualStudioConnection;

            _mainWindowVm.ILMutationsView = _ilMutationsController.ILMutationsVm.View;
            _mainWindowVm.UnitTestsView = _unitTestsController.UnitTestsVm.View;

            _unitTestsController.Mutants = _ilMutationsController.GeneratedMutants;

            // _visualStudioConnection.SolutionEvents.Opened
        }

        public object Shell
        {
            get
            {
                return _mainWindowVm.View;
            }
        }

        public void Initialize()
        {
            _ilMutationsController.Initialize();
            _unitTestsController.Initialize();
        }
    }
}