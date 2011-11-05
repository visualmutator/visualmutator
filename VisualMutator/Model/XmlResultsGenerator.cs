namespace VisualMutator.Model
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure;

    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests.TestsTree;

    #endregion

    public class XmlResultsGenerator
    {
        public XDocument GenerateResults(MutationTestingSession session, bool includeDetailedTestResults)
        {
            List<Mutant> mutants = session.MutantsGroupedByOperators.SelectMany(op => op.Mutants).ToList();
            List<Mutant> testedMutants = mutants.Where(m => m.TestSession != null).ToList();
            List<Mutant> live = testedMutants.Where(m => m.State == MutantResultState.Live).ToList();

            var mutantsNode = new XElement("Mutants",
                new XAttribute("Total", mutants.Count),
                new XAttribute("Live", live.Count),
                new XAttribute("Killed", testedMutants.Count - live.Count),
                new XAttribute("Untested", mutants.Count - testedMutants.Count),
                from oper in session.MutantsGroupedByOperators
                select new XElement("Operator",
                    new XAttribute("Name", oper.Name),
                    new XAttribute("NumberOfMutants", oper.Children.Count),
                    new XAttribute("MutationTimeMiliseconds", oper.MutationTimeMiliseconds),
                    from mutant in oper.Mutants
                    select new XElement("Mutant",
                        new XAttribute("Id", mutant.Id),
                        new XAttribute("State", Functional.ValuedSwitch<MutantResultState, string>(mutant.State)
                        .Case(MutantResultState.Killed, "Killed")
                        .Case(MutantResultState.Live, "Live")
                        .Case(MutantResultState.Untested, "Untested")
                        .Case(MutantResultState.Error, "WithError")
                        .GetResult()
                        )
                        )
                    )
                );

            if (includeDetailedTestResults)
            {
                return
                    new XDocument(
                        new XElement("MutationTestingSession",
                            new XAttribute("MutationScore", session.MutationScore),
                            mutantsNode,
                            CreateDetailedTestingResults(mutants)));
            }
            else
            {
                return
                    new XDocument(
                        new XElement("MutationTestingSession",
                            new XAttribute("MutationScore", session.MutationScore),
                            mutantsNode));
            }
        }

        public XElement CreateDetailedTestingResults(List<Mutant> mutants)
        {
            return new XElement("DetailedTestingResults",
                from mutant in mutants
                where mutant.TestSession != null
                let groupedTests = mutant.TestSession.TestMap.Values.GroupBy(m => m.State).ToList()
                select new XElement("TestedMutant",
                    new XAttribute("Id", mutant.Id),
                    new XAttribute("TestingTimeMiliseconds", mutant.TestSession.TestingTimeMiliseconds),
                    new XAttribute("LoadTestsTimeRawMiliseconds", mutant.TestSession.LoadTestsTimeRawMiliseconds),
                    new XAttribute("RunTestsTimeRawMiliseconds", mutant.TestSession.RunTestsTimeRawMiliseconds),
                    new XElement("Tests",
                    new XAttribute("NumberOfFailedTests", groupedTests.SingleOrDefault(g => g.Key == TestNodeState.Failure).ToEmptyIfNull().Count()),
                    new XAttribute("NumberOfPassedTests", groupedTests.SingleOrDefault(g => g.Key == TestNodeState.Success).ToEmptyIfNull().Count()),
                    new XAttribute("NumberOfInconlusiveTests", groupedTests.SingleOrDefault(g => g.Key == TestNodeState.Inconclusive).ToEmptyIfNull().Count()),
                    from testClass in mutant.TestSession.TestClassses
                    select new XElement("TestClass",
                        new XAttribute("Name", testClass.Name),
                        new XAttribute("FullName", testClass.FullName),
                        from testMethod in testClass.Children.Cast<TestNodeMethod>()
                        where testMethod.State == TestNodeState.Failure
                        select new XElement("TestMethod",
                            new XAttribute("Name", testMethod.Name),
                            new XAttribute("Outcome", "Failed"),
                            new XElement("Message", testMethod.Message)),
                        from testMethod in testClass.Children.Cast<TestNodeMethod>()
                        where testMethod.State.IsIn(TestNodeState.Success, TestNodeState.Inconclusive)
                        select new XElement("TestMethod",
                            new XAttribute("Name", testMethod.Name),
                            new XAttribute("Outcome", Functional.ValuedSwitch<TestNodeState, string>(testMethod.State)
                            .Case(TestNodeState.Success, "Passed")
                            .Case(TestNodeState.Inconclusive, "Inconclusive")
                            .GetResult())
                            )
                        )
                    )
                ));
        }
    }
}