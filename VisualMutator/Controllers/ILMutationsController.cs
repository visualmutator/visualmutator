namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

   
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;

    using VisualMutator.Infrastructure;

    using log4net;

    #endregion

    public class ILMutationsController : Controller
    {
        private readonly IExecute _execute;

        private readonly IMessageService _messageService;

        private readonly IMutantsContainer _mutantsContainer;

        private readonly IOperatorsManager _operatorsManager;

        private readonly ITypesManager _typesManager;

        private readonly ILMutationsViewModel _viewModel;

        private readonly IVisualStudioConnection _visualStudio;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ILMutationsController(
            ILMutationsViewModel viewModel,
            IVisualStudioConnection visualStudio,
            IMutantsContainer mutantsContainer,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            IMessageService messageService,
            IExecute execute
            )
        {
            _viewModel = viewModel;
            _visualStudio = visualStudio;
            _mutantsContainer = mutantsContainer;
            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _messageService = messageService;
            _execute = execute;

            _viewModel.CommandMutate = new BasicCommand(Mutate, CanExecute);
            _viewModel.CommandMutate.UpdateOnCollectionChanged(_viewModel, _viewModel.Assemblies);
            _viewModel.CommandMutate.UpdateOnChanged(_viewModel, () => _viewModel.IsMutationOngoing);
            _viewModel.CommandMutate.UpdateOnChanged(_viewModel, () => _viewModel.AreTypesLoading);

            _viewModel.CommandRefresh = new BasicCommand(Refresh, CanExecute);
            _viewModel.CommandMutate.UpdateOnCollectionChanged(_viewModel, _viewModel.Assemblies);
            _viewModel.CommandMutate.UpdateOnChanged(_viewModel, () => _viewModel.IsMutationOngoing);
            _viewModel.CommandMutate.UpdateOnChanged(_viewModel, () => _viewModel.AreTypesLoading);

            _viewModel.Assemblies = new BetterObservableCollection<AssemblyNode>();
            _viewModel.MutationPackages = _operatorsManager.OperatorPackages;
        }

        public ILMutationsViewModel ILMutationsVm
        {
            get
            {
                return _viewModel;
            }
        }

        private bool CanExecute()
        {
            return _viewModel.Assemblies.Count != 0 && !_viewModel.IsMutationOngoing && !_viewModel.AreTypesLoading;
        }

        public void Initialize()
        {
            //  _viewModel.IsVisible = true;
            _visualStudio.OnBuildBegin += BuildEvents_OnBuildBegin;
            _visualStudio.OnBuildDone += BuildEvents_OnBuildDone;

            _viewModel.IsVisible = true;
            _mutantsContainer.LoadSessions();
            Refresh();
        }

        private void BuildEvents_OnBuildBegin()
        {
            _viewModel.Assemblies.Clear();
        }

        private void BuildEvents_OnBuildDone()
        {
            RefreshTypes();
        }

        public void Deactivate()
        {
            _viewModel.IsVisible = false;
            _viewModel.Assemblies.Clear();
            _mutantsContainer.Clear();
        }

        public void Mutate()
        {
            //Refresh();
            _viewModel.IsMutationOngoing = true;
            _viewModel.ClearMutationLog();
            _viewModel.MutationLog("Starting mutation.");

            Action<string> mutationLog = text =>
            {
                _execute.OnUIThread(() =>
                {
                    _viewModel.MutationLog(text);
                });
            };

            Task.Factory.StartNew(() =>
            {
                return _mutantsContainer.GenerateMutant("Mutant", mutationLog);
            }).ContinueWith(prev =>
            {
                try
                {
                    if (prev.Exception != null)
                    {
                        _messageService.ShowError(prev.Exception, _log);
                    }
                    else
                    {
                        _mutantsContainer.GeneratedMutants.Add(prev.Result);
                        _mutantsContainer.SaveSettingsFile();

                        _viewModel.MutationLog("Mutation complete.");
                    }
                }
                catch (Exception e)
                {
                    _messageService.ShowError(e, _log);
                }
                finally
                {
                    _viewModel.IsMutationOngoing = false;
                }
            }, _execute.GuiScheduler);
        }

        public void Refresh()
        {
            RefreshTypes();
            _operatorsManager.LoadOperators();
        }

        public void RefreshTypes()
        {
            _viewModel.AreTypesLoading = true;
            _viewModel.Assemblies.Clear();
            Task.Factory.StartNew(() =>
            {
                IEnumerable<string> paths = _visualStudio.GetProjectPaths();
                return _typesManager.BuildTypesTree(paths);
            }).ContinueWith(prev =>
            {
                try
                {
                    if (prev.Exception != null)
                    {
                        _messageService.ShowError(prev.Exception, _log);
                    }
                    else
                    {
                        _viewModel.Assemblies.ReplaceRange(prev.Result);
                    }
                }
                catch (Exception e)
                {
                    _messageService.ShowError(e, _log);
                }
                finally
                {
                    _viewModel.AreTypesLoading = false;
                }
            }, _execute.GuiScheduler);
        }
    }
}