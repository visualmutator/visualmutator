namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
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
        
        private readonly LinkedList<Mutant> _mutantsToTest;
        private readonly int _allMutantsCount;
        private int _testedNonEquivalentMutantsCount;
        private bool _requestedStop;
        private int _mutantsKilledCount;
        private SemaphoreSlim _semaphore;

        public TestingProcess(
            IFactory<TestingMutant> mutantTestingFactory,
            Subject<SessionEventArgs> sessionEventsSubject,
            ICollection<Mutant> allMutants)
        {
            _mutantTestingFactory = mutantTestingFactory;
            _sessionEventsSubject = sessionEventsSubject;

            _mutantsToTest = new LinkedList<Mutant>(allMutants);
            _allMutantsCount = allMutants.Count;
            _testedNonEquivalentMutantsCount = 0;

            _semaphore = new SemaphoreSlim(1);
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
            _requestedStop = true;
        }

        public void Start()
        {
            _requestedStop = false;
            
            bool emptyStop = false;
            while (!emptyStop && !_requestedStop)
            {
                _semaphore.Wait();
                Mutant mutant = LockingRemoveFirst(_mutantsToTest);
                if(mutant != null)
                {
                    TestOneMutant(mutant)
                        .ContinueWith(t => _semaphore.Release());
                }
                else
                {
                    emptyStop = true;
                    _semaphore.Release();
                }
            }
        }

        public Task TestOneMutant(Mutant mutant)
        {
            mutant.State = MutantResultState.Creating;

            TestingMutant testingMutant = _mutantTestingFactory.CreateWithParams(_sessionEventsSubject, mutant);

            return testingMutant.RunAsync().ContinueWith(t =>
            {
                if(t.Exception != null)
                {
                    _log.Error(t.Exception);
                }
                else
                {
                    lock (this)
                    {
                        double mutationScore = UpdateMetricsAfterMutantTesting(mutant.State);
                        RaiseTestingProgress(mutationScore);
                    }
                }
            });
           
        }

        private double UpdateMetricsAfterMutantTesting(MutantResultState state)
        {
            _testedNonEquivalentMutantsCount++;

            _mutantsKilledCount = _mutantsKilledCount.IncrementedIf(state == MutantResultState.Killed);

            return ((double)_mutantsKilledCount) / _testedNonEquivalentMutantsCount;

        }


        public void TestWithHighPriority(Mutant mutant)
        {
            LockingMoveToFront(_mutantsToTest, mutant);
        }


        private Mutant LockingRemoveFirst(LinkedList<Mutant> mutantsToTest)
        {
            lock(mutantsToTest)
            {
                Mutant toReturn = mutantsToTest.Count != 0 ? mutantsToTest.First.Value : null;
                if (toReturn != null)
                {
                    mutantsToTest.RemoveFirst();
                }
                return toReturn;
            }
        }

        private void LockingMoveToFront(LinkedList<Mutant> mutantsToTest, Mutant mutant)
        {
            lock (mutantsToTest)
            {
                if (mutantsToTest.Remove(mutant))
                {
                    mutantsToTest.AddFirst(mutant);
                }
            }
        }   
    }
}