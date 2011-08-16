namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using EnvDTE;

    using Ninject;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;

    public class ApplicationController
    {
       

        private readonly MainWindowViewModel _mainWindowVm;

        private ILMutationsController _ilMutationsController;

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