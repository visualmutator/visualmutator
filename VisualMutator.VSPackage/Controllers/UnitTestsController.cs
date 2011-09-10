namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;


    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Util;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Tests;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    using log4net;

    using Controller = PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Controller;

    #endregion

    public class UnitTestsController : Controller
    {
        private BasicCommand _commandRunTests;

        private readonly IExecute _execute;

        private readonly IMessageService _messageBoxService;

        private readonly IMutantsContainer _mutantsContainer;

        private readonly ITestsContainer _testsContainer;

        private readonly Dictionary<string, TestTreeNode> _testMap;

        private readonly TestLoader _tl;

        private readonly UnitTestsViewModel _viewModel;

        private readonly IVisualStudioConnection _visualStudioConnection;

        private BasicCommand _reloadTestList;
        private BasicCommand _showTestDetails;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public UnitTestsController(
            IUnitTestsView view,
            IVisualStudioConnection visualStudioConnection,
            IMessageService messageBoxService,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            IExecute execute

            )
        {
   
            _visualStudioConnection = visualStudioConnection;
            _messageBoxService = messageBoxService;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _execute = execute;

             _testMap = new Dictionary<string, TestTreeNode>();

            _viewModel = new UnitTestsViewModel(view, _mutantsContainer.GeneratedMutants);
            InitViewModel();

        }



        private void InitViewModel()
        {
            _commandRunTests = new BasicCommand(RunTests, () =>
               _viewModel.SelectedMutant != null && !_viewModel.AreTestsRunning);
            _commandRunTests.UpdateOnChanged(_viewModel, () => _viewModel.SelectedMutant);
            _commandRunTests.UpdateOnChanged(_viewModel, () => _viewModel.AreTestsRunning);


            _reloadTestList = new BasicCommand(RefreshTestList);
            _reloadTestList.ExecuteOnChanged(_viewModel, () => _viewModel.SelectedMutant);

            _showTestDetails = new BasicCommand(ShowTestDetails);
            _showTestDetails.ExecuteOnChanged(_viewModel, () => _viewModel.SelectedTestItem);

            _viewModel.CommandDeleteMutant = new BasicCommand(DeleteMutant, () => _viewModel.SelectedMutant != null);
            _viewModel.CommandDeleteMutant.UpdateOnChanged(_viewModel, () => _viewModel.SelectedMutant);



            _viewModel.CommandRunTests = _commandRunTests;


        }



        public UnitTestsViewModel UnitTestsVm
        {
            get
            {
                return _viewModel;
            }
        }
        public void DeleteMutant()
        {
            _mutantsContainer.DeleteMutant(_viewModel.SelectedMutant);
            _viewModel.SelectedMutant = _viewModel.Mutants.Last();

        }

        public void ShowTestDetails()
        {
            var method = _viewModel.SelectedTestItem as TestNodeMethod;
            if (method != null && method.HasResults)
            {
                _viewModel.ResultText = method.Message;
            }
            else
            {
                _viewModel.ResultText = "";
            }
        }

        public void Initialize()
        {
           
        }
        public void Deactivate()
        {
          
        }
        public void RefreshTestList()
        {
            if (_viewModel.SelectedMutant == null)
            {
                return;
            }
            _viewModel.AreTestsLoading = true;
            _viewModel.TestNamespaces.Clear();
            Task.Factory.StartNew(() =>
            {
               
                return _testsContainer.LoadTests(_viewModel.SelectedMutant.Assemblies);
                

            }).ContinueWith(prev =>
            {
                try
                {
                    _viewModel.AreTestsLoading = false;
                    if (prev.Exception != null)
                    {
                        _messageBoxService.ShowError(prev.Exception, _log);
                    }
                    else
                    {
                        _viewModel.TestNamespaces.ReplaceRange(prev.Result);
                    }  
                }
                catch (Exception e)
                {
                    _messageBoxService.ShowError(e, _log);
                }
            }, _execute.GuiScheduler);


        }
       

        public void RunTests()
        {
            _viewModel.AreTestsRunning = true;

            
            Task.Factory.StartNew(() =>
            {
                _testsContainer.RunTests();

            }).ContinueWith( prev =>
            {
                if (prev.Exception != null)
                {
                    _messageBoxService.ShowError(prev.Exception, _log);
                }
                _viewModel.AreTestsRunning = false;

            }, _execute.GuiScheduler);


        }

       
    }
}