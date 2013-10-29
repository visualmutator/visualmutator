namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;
    using Infrastructure;
    using Model;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.Tests;
    using ViewModels;
    using Views;

    #endregion

    public class OnlyMutantsCreationController :
        CreationController<OnlyMutantsCreationViewModel, IOnlyMutantsCreationView>
    {
        public OnlyMutantsCreationController(OnlyMutantsCreationViewModel viewModel, ITypesManager typesManager,
                                             IOperatorsManager operatorsManager,
                                             IHostEnviromentConnection hostEnviroment, ITestsContainer testsContainer,
                                             CommonServices svc)
            : base(viewModel, typesManager, operatorsManager, hostEnviroment, testsContainer, svc)
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
                if (!_svc.FileSystem.Directory.Exists(_viewModel.MutantsFolderPath))
                {
                    try
                    {
                        _svc.FileSystem.Directory.CreateDirectory(_viewModel.MutantsFolderPath);
                    }
                    catch (Exception)
                    {
                        _svc.Logging.ShowError("Could not create directory.", view: _viewModel.View);
                        return;
                    }
                }


                if (_svc.FileSystem.Directory.GetDirectories(_viewModel.MutantsFolderPath).Length == 0
                    && _svc.FileSystem.Directory.GetFiles(_viewModel.MutantsFolderPath).Length == 0)
                {
                    Result = new MutationSessionChoices
                        {
                            SelectedOperators =
                                _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                                .Where(oper => (bool) oper.IsIncluded).Select(n => n.Operator).ToList(),
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
                    _svc.Logging.ShowError("Selected directory is not empty. Select empty directory.",
                                           view: _viewModel.View);
                }
            }
            else
            {
                _svc.Logging.ShowError("Selected path is invalid", view: _viewModel.View);
            }
        }
    }
}