namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using Controllers;
    using Infrastructure;
    using log4net;
    using Mutations.MutantsTree;
    using Tests;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;

    public class TestingProcess
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IFactory<TestingMutant> _mutantTestingFactory;
        private readonly Subject<SessionEventArgs> _sessionEventsSubject;

        private readonly int _allMutantsCount;
        private int _testedNonEquivalentMutantsCount;
        private int _mutantsKilledCount;
        private readonly WorkerCollection<Mutant> _mutantsWorkers;

        public TestingProcess(
            IFactory<TestingMutant> mutantTestingFactory,
            MutationSessionChoices choices,
            // ------- on creation
            Subject<SessionEventArgs> sessionEventsSubject,
            ICollection<Mutant> allMutants)
        {
            _mutantTestingFactory = mutantTestingFactory;
            _sessionEventsSubject = sessionEventsSubject;

            _allMutantsCount = allMutants.Count;
            _testedNonEquivalentMutantsCount = 0;

            _mutantsWorkers = new WorkerCollection<Mutant>(allMutants,
                choices.MainOptions.ProcessingThreadsCount, TestOneMutant);
        }

        public void RaiseTestingProgress(double mutationScore)
        {
            _sessionEventsSubject.OnNext(new TestingProgressEventArgs(OperationsState.Testing)
            {
                NumberOfAllMutants = _allMutantsCount,
                NumberOfMutantsKilled = _mutantsKilledCount,
                NumberOfAllMutantsTested = _testedNonEquivalentMutantsCount,
                MutationScore = mutationScore,
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

        public async void TestOneMutant(Mutant mutant)
        {
            mutant.State = MutantResultState.Creating;

            TestingMutant testingMutant = _mutantTestingFactory.CreateWithParams(_sessionEventsSubject, mutant);

            try
            {
                await testingMutant.RunAsync();
                lock (this)
                {
                    double mutationScore = UpdateMetricsAfterMutantTesting(mutant.State);
                    RaiseTestingProgress(mutationScore);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        private double UpdateMetricsAfterMutantTesting(MutantResultState state)
        {
            _testedNonEquivalentMutantsCount++;

            _mutantsKilledCount = _mutantsKilledCount.IncrementedIf(state == MutantResultState.Killed);

            return ((double)_mutantsKilledCount) / _testedNonEquivalentMutantsCount;
        }

        public void TestWithHighPriority(Mutant mutant)
        {
           _mutantsWorkers.LockingMoveToFront(mutant);
        }
    }
}