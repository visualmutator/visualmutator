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
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Waf.Applications;

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

        private readonly UnitTestsViewModel _unitTestsVm;

        private readonly IVisualStudioConnection _visualStudioConnection;

        private BasicCommand _reloadTestList;
        private BasicCommand _showTestDetails;


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

            _unitTestsVm = new UnitTestsViewModel(view, _mutantsContainer.GeneratedMutants);
            InitViewModel();

        }



        private void InitViewModel()
        {
            _commandRunTests = new BasicCommand(RunTests, () =>
               _unitTestsVm.SelectedMutant != null && !_unitTestsVm.AreTestsRunning);
            _commandRunTests.UpdateOnChanged(_unitTestsVm, () => _unitTestsVm.SelectedMutant);
            _commandRunTests.UpdateOnChanged(_unitTestsVm, () => _unitTestsVm.AreTestsRunning);


            _reloadTestList = new BasicCommand(RefreshTestList);
            _reloadTestList.ExecuteOnChanged(_unitTestsVm, () => _unitTestsVm.SelectedMutant);

            _showTestDetails = new BasicCommand(ShowTestDetails);
            _showTestDetails.ExecuteOnChanged(_unitTestsVm, () => _unitTestsVm.SelectedTestItem);

            _unitTestsVm.CommandRunTests = _commandRunTests;


        }



        public UnitTestsViewModel UnitTestsVm
        {
            get
            {
                return _unitTestsVm;
            }
        }


        public IEnumerable<TestTreeNode> TestsToRun { get; set; }


        public void ShowTestDetails()
        {
            var method = _unitTestsVm.SelectedTestItem as TestNodeMethod;
            if (method != null && method.HasResults)
            {
                _unitTestsVm.ResultText = method.Message;
            }
            else
            {
                _unitTestsVm.ResultText = "";
            }
        }

        public void Initialize()
        {
           
        }
        public void RefreshTestList()
        {
            _unitTestsVm.AreTestsLoading = true;
            _unitTestsVm.TestNamespaces.Clear();
            Task.Factory.StartNew(() =>
            {
                return _testsContainer.LoadTests(_unitTestsVm.SelectedMutant);

            }).ContinueWith(prev =>
            {
                _unitTestsVm.AreTestsLoading = false;
                if (prev.Exception != null)
                {
                    _messageBoxService.ShowError(prev.Exception);
                }
                _unitTestsVm.TestNamespaces.ReplaceRange(prev.Result);

            }, _execute.WpfScheduler);


        }
       

        public void RunTests()
        {
            _unitTestsVm.AreTestsRunning = true;

            
            Task.Factory.StartNew(() =>
            {
                _testsContainer.RunTests();

            }).ContinueWith( prev =>
            {
                if (prev.Exception != null)
                {
                    _messageBoxService.ShowError(prev.Exception);
                }
                _unitTestsVm.AreTestsRunning = false;

            }, _execute.WpfScheduler);


        }


        
    }
}