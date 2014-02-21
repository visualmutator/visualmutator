namespace VisualMutator.Model
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Decompilation;
    using Decompilation.CodeDifference;
    using Mutations.MutantsTree;
    using Mutations.Types;
    using Tests.TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    #endregion

    public class XmlResultsGenerator
    {
        private readonly ICodeDifferenceCreator _codeDifferenceCreator;


        public XmlResultsGenerator(
            ICodeDifferenceCreator codeDifferenceCreator)
        {
            _codeDifferenceCreator = codeDifferenceCreator;
 
        }

        public XDocument GenerateResults(MutationTestingSession session, 
            bool includeDetailedTestResults,
            bool includeCodeDifferenceListings)
        {
            List<Mutant> mutants = session.MutantsGrouped.Cast<CheckedNode>()
                .SelectManyRecursive(n => n.Children, leafsOnly:true).OfType<Mutant>().ToList();
            List<Mutant> mutantsWithErrors = mutants.Where(m => m.State == MutantResultState.Error).ToList();
            List<Mutant> testedMutants = mutants.Where(m => m.MutantTestSession.IsComplete).ToList();
            List<Mutant> live = testedMutants.Where(m => m.State == MutantResultState.Live).ToList();

            var mutantsNode = new XElement("Mutants",
                new XAttribute("Total", mutants.Count),
                new XAttribute("Live", live.Count),
                new XAttribute("Killed", testedMutants.Count - live.Count),
                new XAttribute("Untested", mutants.Count - testedMutants.Count),
                new XAttribute("WithError", mutantsWithErrors.Count),
                new XAttribute("TotalSizeKilobytes", -1),//mutants.Sum(mut => mut.StoredAssemblies.SizeInKilobytes())),
                new XAttribute("AverageSizeKilobytes", -1),//mutants.AverageOrZero(mut => mut.StoredAssemblies.SizeInKilobytes())),
              //  new XAttribute("FindingMutationTargetsTotalTimeMiliseconds", session.MutantsGrouped
             //       .Sum(oper => oper.FindTargetsTimeMiliseconds)),
               // new XAttribute("TotalMutationTimeMiliseconds", session.MutantsGrouped
               //     .Sum(oper => oper.MutationTimeMiliseconds)),
              //  new XAttribute("AverageMutationTimePerOperatorMiliseconds", session.MutantsGrouped
              //      .AverageOrZero(oper => oper.MutationTimeMiliseconds)),
                new XAttribute("TotalTestingTimeMiliseconds", testedMutants
                    .Sum(mut => mut.MutantTestSession.TestingTimeMiliseconds)),
                new XAttribute("AverageTestingTimeMiliseconds", testedMutants
                    .AverageOrZero(mut => mut.MutantTestSession.TestingTimeMiliseconds)),
                from assemblyNode in session.MutantsGrouped
                select new XElement("Assembly",
                  //  new XAttribute("Identificator", oper.Identificator),
                    new XAttribute("Name", assemblyNode.Name),
                   // new XAttribute("NumberOfMutants", oper.Children.Count),
                  //  new XAttribute("NumberOfKilledMutants", oper.MutantGroups.SelectMany(m=>m.Mutants).Count(m=>m.State == MutantResultState.Killed)),
                  //  new XAttribute("NumberOfLiveMutants", oper.MutantGroups.SelectMany(m => m.Mutants).Count(m => m.State == MutantResultState.Live)),
                  //  new XAttribute("FindingMutationTargetsTimeMiliseconds", oper.FindTargetsTimeMiliseconds),
                //    new XAttribute("TotalMutantsCreationTimeMiliseconds", oper.MutationTimeMiliseconds),
                //    new XAttribute("AverageMutantCreationTimeMiliseconds", oper.Children.Count == 0 ? 0 :
                 //       oper.MutationTimeMiliseconds / oper.Children.Count),
                    from type in assemblyNode.Children.SelectManyRecursive(
                        n => n.Children?? new NotifyingCollection<CheckedNode>())
                        .OfType<TypeNode>()
                    select new XElement("Type",
                        new XAttribute("Name", type.Name),
                        new XAttribute("Namespace", type.Namespace),
                            from method in type.Children.Cast<MethodNode>()//TODO: what if nested type?
                            select new XElement("Method",
                            new XAttribute("Name", method.Name),
                            from mutGroup in method.Children.Cast<MutantGroup>()
                            select new XElement("MutantGroup",
                                new XAttribute("Name", mutGroup.Name),
                                from mutant in mutGroup.Mutants
                                select new XElement("Mutant",
                                    new XAttribute("Id", mutant.Id),
                                    new XAttribute("Description", mutant.Description),
                                    //    new XAttribute("SizeInKilobytes", mutant.StoredAssemblies.SizeInKilobytes()),
                                    new XAttribute("State", mutant.State.ToString()
                                    ),
                                    new XElement("ErrorInfo",
                                            new XElement("Description", mutant.MutantTestSession.ErrorDescription),
                                            new XElement("ExceptionMessage", mutant.MutantTestSession.ErrorMessage)
                                            ).InArrayIf(mutant.State == MutantResultState.Error)
                                    )
                                )
                            )
                        )
                    )
                );


            var optionalElements = new List<XElement>();

            if (includeCodeDifferenceListings)
            {

                optionalElements.Add(CreateCodeDifferenceListings(mutants));
            }

            if (includeDetailedTestResults)
            {
                optionalElements.Add(CreateDetailedTestingResults(mutants));
            }

            long totalTimeMs = 0;//session.MutantsGrouped.Sum(oper => oper.MutationTimeMiliseconds)
                         //   + mutants.Sum(m => m.MutantTestSession.TestingTimeMiliseconds);
            return
                new XDocument(
                    new XElement("MutationTestingSession",
                        new XAttribute("MutationScore", session.MutationScore),
                        new XAttribute("TotalTimeSeconds", totalTimeMs/1000),
                        mutantsNode,
                        optionalElements));
            
        }
        public XElement CreateCodeDifferenceListings(List<Mutant> mutants)
        {

            return new XElement("CodeDifferenceListings",
                from mutant in mutants
                select new XElement("MutantCodeListing",
                    new XAttribute("MutantId", mutant.Id),
                    new XElement("Code", 
                        Environment.NewLine+_codeDifferenceCreator.CreateDifferenceListing(CodeLanguage.CSharp, 
                        mutant).Code)
                        )
                    );

        }

        public XElement CreateDetailedTestingResults(List<Mutant> mutants)
        {
            return new XElement("DetailedTestingResults",   
                from mutant in mutants
                where mutant.MutantTestSession.IsComplete
                let groupedTests = mutant.MutantTestSession.TestsByAssembly.Values
                .Select(c => c.TestNodeAssembly).SelectMany(n => n.TestNamespaces)
                .GroupBy(m => m.State).ToList()
                select new XElement("TestedMutant",
                    new XAttribute("MutantId", mutant.Id),
                    new XAttribute("TestingTimeMiliseconds", mutant.MutantTestSession.TestingTimeMiliseconds),
                    new XAttribute("LoadTestsTimeRawMiliseconds", mutant.MutantTestSession.LoadTestsTimeRawMiliseconds),
                    new XAttribute("RunTestsTimeRawMiliseconds", mutant.MutantTestSession.RunTestsTimeRawMiliseconds),
                    new XElement("Tests",
                    new XAttribute("NumberOfFailedTests", groupedTests.SingleOrDefault(g => g.Key == TestNodeState.Failure).ToEmptyIfNull().Count()),
                    new XAttribute("NumberOfPassedTests", groupedTests.SingleOrDefault(g => g.Key == TestNodeState.Success).ToEmptyIfNull().Count()),
                    new XAttribute("NumberOfInconlusiveTests", groupedTests.SingleOrDefault(g => g.Key == TestNodeState.Inconclusive).ToEmptyIfNull().Count()),
                    from testClass in mutant.MutantTestSession.TestsRootNode.TestNodeAssemblies
                        .Cast<CheckedNode>().SelectManyRecursive(n => n.Children).OfType<TestNodeClass>()
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
                            new XAttribute("Outcome", FunctionalExt.ValuedSwitch<TestNodeState, string>(testMethod.State)
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