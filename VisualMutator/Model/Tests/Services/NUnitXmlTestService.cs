namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using log4net;
    using NUnit.Core;
    using RunProcessAsTask;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    #endregion

    public class NUnitXmlTestService : NUnitTestService
    {

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IList<string> _assemblies;
        private ICollection<TestId> _selectedTests;

        public IList<string> Assemblies
        {
            get { return _assemblies; }
            set { _assemblies = value; }
        }

        public string NUnitConsolePath { get; set; }
        public NUnitXmlTestService(INUnitWrapper nUnitWrapper, IMessageService messageService)
            : base(nUnitWrapper, messageService)
        {
            NUnitConsolePath = "nunit-console-x86.exe";
        }

        public override IEnumerable<TestNodeClass> LoadTests(IList<string> assemblies, MutantTestSession mutantTestSession)
        {
            _assemblies = assemblies;
            IEnumerable<TestNodeClass> testNodeClasses = base.LoadTests(assemblies, mutantTestSession);

            UnloadTests();
            return testNodeClasses;
        }
        
        public override Task<List<TestNodeMethod>> RunTests(MutantTestSession mutantTestSession)
        {

            var list = new List<TestNodeMethod>();

            var sw = new Stopwatch();
            sw.Start();

            var ext = new NUnitExternal(NUnitConsolePath);

            Task<ProcessResults> results = ext.RunNUnitConsole(_assemblies, "muttest-results.xml", _selectedTests);

            return results.ContinueWith(testResult =>
            {
                if(testResult.Exception != null)
                {
                    _log.Error(testResult.Exception);
                    return null;
                }
                else
                {
                    Dictionary<string, MyTestResult> tresults = ext.ProcessResultFile("muttest-results.xml");
                    foreach (var myTestResult in tresults)
                    {
                        TestNodeMethod testNodeMethod = mutantTestSession.TestMap[myTestResult.Key];
                        testNodeMethod.Message = myTestResult.Value.Message + "\n" + myTestResult.Value.StackTrace;
                        testNodeMethod.State = myTestResult.Value.Success
                            ? TestNodeState.Success
                            : TestNodeState.Failure;
                    }
                    sw.Stop();
                    mutantTestSession.RunTestsTimeRawMiliseconds = sw.ElapsedMilliseconds;
                    return list;
                }
            });
        }

        public override void CreateTestFilter(ICollection<TestId> selectedTests)
        {
            _selectedTests = selectedTests;
            base.CreateTestFilter(selectedTests);
        }


        


    }
}