namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model;
    using Model.Tests;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    using log4net;

    #endregion

    public class OnlyMutantsCreationController : CreationController<OnlyMutantsCreationViewModel, IOnlyMutantsCreationView>
    {
        public OnlyMutantsCreationController(OnlyMutantsCreationViewModel viewModel, ITypesManager typesManager, IOperatorsManager operatorsManager, IHostEnviromentConnection hostEnviroment, ITestsContainer testsContainer, SessionController sessionController, CommonServices svc) : base(viewModel, typesManager, operatorsManager, hostEnviroment, testsContainer, sessionController, svc)
        {
            _viewModel.CommandBrowseForMutantFolder = new BasicCommand(BrowseForMutantsFolder);
        }

        public void BrowseForMutantsFolder()
        {
            var dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                _viewModel.MutantsFolderPath = dlg.SelectedPath;

            }
        }

        protected override void AcceptChoices()
        {

            if (!string.IsNullOrEmpty(_viewModel.MutantsFolderPath) 
                && Path.IsPathRooted(_viewModel.MutantsFolderPath))
            {

                if(!_svc.FileSystem.Directory.Exists(_viewModel.MutantsFolderPath))
                {
                    try
                    {
                        _svc.FileSystem.Directory.CreateDirectory(_viewModel.MutantsFolderPath);
                    }
                    catch (Exception)
                    {
                        _svc.Logging.ShowError("Could not create directory.", view:_viewModel.View);
                        return;
                    }
                    
                }
                

                if (_svc.FileSystem.Directory.GetDirectories(_viewModel.MutantsFolderPath).Length == 0
                    && _svc.FileSystem.Directory.GetFiles(_viewModel.MutantsFolderPath).Length == 0)
                {
                    Result = new MutationSessionChoices
                    {
                        SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                                         .Where(oper => oper.IsLeafIncluded).Select(n => n.Operator).ToList(),
                        ProjectPaths = _typesManager.ProjectPaths.ToList(),
                        Assemblies = _viewModel.TypesTreeMutate.Assemblies,
                        SelectedTypes = _typesManager.GetIncludedTypes(_viewModel.TypesTreeMutate.Assemblies),
                        MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                        MutantsCreationFolderPath = _viewModel.MutantsFolderPath,
                    };
                    _viewModel.Close();
                }
                else
                {
                    _svc.Logging.ShowError("Selected directory is not empty. Select empty directory.", view: _viewModel.View);
                }
            }
            else
            {
                _svc.Logging.ShowError("Selected path is invalid", view: _viewModel.View);
            }
        }


    }
}