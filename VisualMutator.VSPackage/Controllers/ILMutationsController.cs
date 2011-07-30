namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Waf.Applications;

    using Ninject;

    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;

    using VisualMutator.Domain;

    public class ILMutationsController : Controller
    {
        private readonly ILMutationsViewModel _viewModel;

        private readonly IVisualStudioConnection _visualStudio;

        public ILMutationsController(
            ILMutationsViewModel viewModel,
            IVisualStudioConnection visualStudio
            )
        {
            _viewModel = viewModel;
            _visualStudio = visualStudio;
            _viewModel.CommandRefresh = new DelegateCommand(Refresh);
        }


        public void Refresh()
        {

            var paths = _visualStudio.GetProjectPaths();

            var c = new Class1();
            c.CreateTree(paths, _viewModel.Assemblies);

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
            Refresh();
        }
    }
}