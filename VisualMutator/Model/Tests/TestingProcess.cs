namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Controllers;
    using Infrastructure;
    using log4net;
    using Mutations.MutantsTree;
    using UsefulTools.ExtensionMethods;

    public class TestingProcess
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRootFactory<TestingMutant> _mutantTestingFactory;
        private readonly Subject<SessionEventArgs> _sessionEventsSubject;

        private readonly int _allMutantsCount;
        private int _testedNonEquivalentMutantsCount;
        private int _mutantsKilledCount;
        private readonly WorkerCollection<Mutant> _mutantsWorkers;
        private double _mutationScore;

        public double MutationScore
        {
            get { return _mutationScore; }
        }

        public TestingProcess(
            IRootFactory<TestingMutant> mutantTestingFactory,
            MutationSessionChoices choices,
            // ------- on creation
            Subject<SessionEventArgs> sessionEventsSubject,
            ICollection<Mutant> allMutants)
        {
        
            _mutantTestingFactory = mutantTestingFactory;
            _sessionEventsSubject = sessionEventsSubject;

            _allMutantsCount = allMutants.Count;
            _testedNonEquivalentMutantsCount = 0;

            _log.Info("Testing process: all:" + _allMutantsCount);

            _mutantsWorkers = new WorkerCollection<Mutant>(allMutants,
                choices.MainOptions.ProcessingThreadsCount, TestOneMutant);
        }

        public void RaiseTestingProgress()
        {
            _log.Info("Testing progress: all:"+ _allMutantsCount +
                ", tested: "+ _testedNonEquivalentMutantsCount
                +"killed: "+ _mutantsKilledCount);

            _sessionEventsSubject.OnNext(new TestingProgressEventArgs(OperationsState.Testing)
            {
                NumberOfAllMutants = _allMutantsCount,
                NumberOfMutantsKilled = _mutantsKilledCount,
                NumberOfAllMutantsTested = _testedNonEquivalentMutantsCount,
                MutationScore = _mutationScore,
            });
        }

        public void Stop()
        {
            _mutantsWorkers.Stop();
        }

        public void Start(Action endCallback)
        {
            _mutantsWorkers.Start(endCallback);
        }

        public async Task TestOneMutant(Mutant mutant)
        {
            
            try
            {
                IObjectRoot<TestingMutant> testingMutant = _mutantTestingFactory.CreateWithParams(_sessionEventsSubject, mutant);

                await testingMutant.Get.RunAsync();
                lock (this)
                {
                    UpdateMetricsAfterMutantTesting(mutant.State);
                    RaiseTestingProgress();
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
                mutant.State = MutantResultState.Error;
            }
        }

        private void UpdateMetricsAfterMutantTesting(MutantResultState state)
        {
            _testedNonEquivalentMutantsCount++;

            _mutantsKilledCount = _mutantsKilledCount.IncrementedIf(state == MutantResultState.Killed);

            _mutationScore =((double)_mutantsKilledCount) / _testedNonEquivalentMutantsCount;
        }

        public void TestWithHighPriority(Mutant mutant)
        {
           _mutantsWorkers.LockingMoveToFront(mutant);
        }
        public void MarkedAsEqivalent(bool equivalent)
        {
            lock (this)
            {
                if (equivalent)
                {
                    _testedNonEquivalentMutantsCount--;
                }
                else
                {
                    _testedNonEquivalentMutantsCount++;
                }
                _mutationScore = ((double)_mutantsKilledCount) / _testedNonEquivalentMutantsCount;
                RaiseTestingProgress();
            }
        }
    }
}