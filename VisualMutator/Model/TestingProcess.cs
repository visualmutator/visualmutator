namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Controllers;
    using log4net;
    using Mutations.MutantsTree;
    using Tests;
    using UsefulTools.ExtensionMethods;

    public class TestingProcess
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MutationSessionChoices _choices;
        private readonly TestsContainer _testsContainer;
        private readonly Subject<SessionEventArgs> _sessionEventsSubject;
        
        private readonly LinkedList<Mutant> _mutantsToTest;
        private readonly int _allMutantsCount;
        private int _testedNonEquivalentMutantsCount;
        private bool _requestedStop;
        private int _mutantsKilledCount;

        public TestingProcess(
            MutationSessionChoices choices,
            TestsContainer testsContainer,
            Subject<SessionEventArgs> sessionEventsSubject,
            ICollection<Mutant> allMutants)
        {
            _choices = choices;
            _testsContainer = testsContainer;
            _sessionEventsSubject = sessionEventsSubject;

            _mutantsToTest = new LinkedList<Mutant>(allMutants);
            _allMutantsCount = allMutants.Count;
            _testedNonEquivalentMutantsCount = 0;
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
            Mutant mutant;
            while ((mutant = LockingRemoveFirst(_mutantsToTest)) != null && !_requestedStop)
            {

                TestOneMutant(mutant);
            }
        }

        public void TestOneMutant(Mutant mutant)
        {
                //RaiseTestingProgress();

                mutant.State = MutantResultState.Creating;

                try
                {
                    var storedMutantInfo = _testsContainer.StoreMutant( mutant);

                    if (_choices.MutantsCreationOptions.IsMutantVerificationEnabled)
                    {
                        _testsContainer.VerifyMutant(storedMutantInfo, mutant);
                    }
                    _sessionEventsSubject.OnNext(new MutantStoredEventArgs(storedMutantInfo));
                    
//                    CodeWithDifference diff = _codeDifferenceCreator.CreateDifferenceListing(
//                        CodeLanguage.IL, mutant);
//
//                    if (diff.LineChanges.Count == 0)
//                    {
//                        mutant.IsEquivalent = true;
//                    }
                    if (!mutant.IsEquivalent)
                    {
                        _testsContainer.RunTestsForMutant(_choices.MutantsTestingOptions,
                            storedMutantInfo, mutant);

                        _testedNonEquivalentMutantsCount++;

                        _mutantsKilledCount = _mutantsKilledCount.IncrementedIf(mutant.State == MutantResultState.Killed);
                    }
                    storedMutantInfo.Dispose();

                    double mutationScore = ((double)_mutantsKilledCount) / _testedNonEquivalentMutantsCount;

                    RaiseTestingProgress(mutationScore);
                }
                catch (Exception e)
                {
                    _log.Error(e);
                    mutant.MutantTestSession.ErrorDescription = e.Message;
                    mutant.MutantTestSession.ErrorMessage = e.Message;
                    mutant.MutantTestSession.Exception = e;
                    mutant.State = MutantResultState.Error;
                }
        }


        public void TestWithHighPriority(Mutant mutant)
        {
            LockingMoveToFront(_mutantsToTest, mutant);
        }


        private Mutant LockingRemoveFirst(LinkedList<Mutant> mutantsToTest)
        {
            Monitor.Enter(mutantsToTest);
            Mutant toReturn = mutantsToTest.Count != 0 ? mutantsToTest.First.Value : null;
            if (toReturn != null)
            {
                mutantsToTest.RemoveFirst();
            }
            Monitor.Exit(mutantsToTest);
            return toReturn;
        }

        private void LockingMoveToFront(LinkedList<Mutant> mutantsToTest, Mutant mutant)
        {
            Monitor.Enter(mutantsToTest);
            if (mutantsToTest.Remove(mutant))
            {
                mutantsToTest.AddFirst(mutant);
            }
            Monitor.Exit(mutantsToTest);
        }   
    }
}