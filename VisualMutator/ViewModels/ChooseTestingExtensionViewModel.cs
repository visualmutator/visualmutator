namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Paths;
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Tests.Custom;
    using VisualMutator.Views;

    public class ChooseTestingExtensionViewModel : ViewModel<IChooseTestingExtensionView>
    {
        private readonly CommonServices _svc;

        public ChooseTestingExtensionViewModel(
            IChooseTestingExtensionView view,
            CommonServices svc) :base(view)
        {
            _svc = svc;
            CommandOk = new BasicCommand(Ok, () => SelectedTestingProcessExtension != null)
                .UpdateOnChanged(this, _ => _.SelectedTestingProcessExtension);
            CommandCancel = new BasicCommand(Cancel);

        }

        public void Run(IWindow owner, TestingProcessExtensionOptions previous)
        {
            TestingProcessExtensionParameter = previous == null ? "" : previous.Parameter;
            _svc.Threading.ScheduleAsync(LoadTestingExtensions, 
                res =>
                {
                    TestingProcessExtensions = new NotifyingCollection<ITestingProcessExtension>(res);
                    var noneExt = TestingProcessExtensions.First(e => e.Name == DefaultTestingProcessExtension.ConstName);
                    SelectedTestingProcessExtension = previous == null
                        ? noneExt : TestingProcessExtensions.FirstOrDefault(e => e.Name
                            == previous.TestingProcessExtension.Name) ?? noneExt;
                    
                });
            View.ShowDialog(owner);
        }

        [ImportMany]
        public IEnumerable<ITestingProcessExtension> TestingProcessExtensionsMef { get; set; }

        public IEnumerable<ITestingProcessExtension> LoadTestingExtensions()
        {
            var extensionDirectory = new Uri(Assembly.GetExecutingAssembly().CodeBase)
                .LocalPath.ToFilePathAbs().ParentDirectoryPath;

            var catalog = new DirectoryCatalog(extensionDirectory.Path);
            var container = new CompositionContainer(catalog);

            container.ComposeParts(this);

            return TestingProcessExtensionsMef;
        }
        public TestingProcessExtensionOptions Result
        {
            get;
            set;
        }

        public bool HasResult
        {
            get
            {
                return Result != null;
            }
        }
        private NotifyingCollection<ITestingProcessExtension> _testingProcessExtensions;

        public NotifyingCollection<ITestingProcessExtension> TestingProcessExtensions
        {
            get
            {
                return _testingProcessExtensions;
            }
            set
            {
                SetAndRise(ref _testingProcessExtensions, value, () => TestingProcessExtensions);
            }
        }

        private ITestingProcessExtension _selectedTestingProcessExtension;

        public ITestingProcessExtension SelectedTestingProcessExtension
        {
            get
            {
                return _selectedTestingProcessExtension;
            }
            set
            {
                SetAndRise(ref _selectedTestingProcessExtension, value, () => SelectedTestingProcessExtension);
            }
        }

        private string _testingProcessExtensionParameter;

        public string TestingProcessExtensionParameter
        {
            get
            {
                return _testingProcessExtensionParameter;
            }
            set
            {
                SetAndRise(ref _testingProcessExtensionParameter, value, () => TestingProcessExtensionParameter);
            }
        }

        public void Ok()
        {
            Result = new TestingProcessExtensionOptions
            {
                Parameter = TestingProcessExtensionParameter,
                TestingProcessExtension = SelectedTestingProcessExtension,
            };
            View.Close();
        }

        public void Cancel()
        {
            View.Close();
        }
        private BasicCommand _commandOk;

        public BasicCommand CommandOk
        {
            get
            {
                return _commandOk;
            }
            set
            {
                SetAndRise(ref _commandOk, value, () => CommandOk);
            }
        }


        private BasicCommand _commandCancel;

        public BasicCommand CommandCancel
        {
            get
            {
                return _commandCancel;
            }
            set
            {
                SetAndRise(ref _commandCancel, value, () => CommandCancel);
            }
        }
    }
}