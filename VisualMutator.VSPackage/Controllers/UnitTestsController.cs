namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Waf.Applications;

    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;

    using VisualMutator.Domain;

    public class UnitTestsController : Controller
    {
        private readonly UnitTestsViewModel _unitTestsVm;

        private ObservableCollection<MutationSession> _mutants;

        public UnitTestsViewModel UnitTestsVm
        {
            get
            {
                return _unitTestsVm;
            }
        }

        public ObservableCollection<MutationSession> Mutants
        {
            set
            {
                _mutants = value;
                _unitTestsVm.Mutants = value;
            }
            get
            {
                return _mutants;
            }
        }

        public UnitTestsController(UnitTestsViewModel unitTestsVm)
        {
            _unitTestsVm = unitTestsVm;


        }
    }
}