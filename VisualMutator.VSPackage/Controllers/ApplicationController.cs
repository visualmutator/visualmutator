namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using EnvDTE;

    using Microsoft.VisualStudio.Shell;

    using Ninject;

    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;

    using VisualMutator.Domain;

    public class ApplicationController
    {
        private IKernel _kernel;

        private readonly MainWindowViewModel _mainWindowVm;

        private ILMutationsController _ilMutationsController;

        private readonly UnitTestsController _unitTestsController;

        public ApplicationController(IKernel kernel, 
            MainWindowViewModel mainWindowVm,
            ILMutationsController ilMutationsController,
            UnitTestsController unitTestsController
            )
        {
            _kernel = kernel;
 
            _mainWindowVm = mainWindowVm;


         //   var view = new ILMutationsView();
          //  var vm = new ILMutationsViewModel(view);
            _ilMutationsController = ilMutationsController;
            _unitTestsController = unitTestsController;

            _mainWindowVm.ILMutationsView = _ilMutationsController.ILMutationsVm.View;
            _mainWindowVm.UnitTestsView = _unitTestsController.UnitTestsVm.View;

            _unitTestsController.Mutants = _ilMutationsController.GeneratedMutants;



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
            
        }
    }
}