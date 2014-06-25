namespace VisualMutator.Model
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Controllers;
    using Decompilation;
    using Decompilation.CodeDifference;
    using log4net;
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
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly AutoCreationController _creationController;
        private readonly SessionController _sessionController;
        private readonly ICodeDifferenceCreator _codeDifferenceCreator;


        public XmlResultsGenerator(
            AutoCreationController creationController,
            SessionController sessionController,
            ICodeDifferenceCreator codeDifferenceCreator)
        {
            _creationController = creationController;
            _sessionController = sessionController;
            _codeDifferenceCreator = codeDifferenceCreator;
        }

        public XDocument GenerateResults(MutationTestingSession session, 
            bool includeDetailedTestResults,
            bool includeCodeDifferenceListings)
        {
            _log.Info("Generating session results to file.");

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
              //  new XAttribute("FindingMutationTargetsTotalTimeMiliseconds", session.MutantsGrouped
              //      .Sum(oper => oper.FindTargetsTimeMiliseconds)),

                
                new XAttribute("AverageCreationTimeMiliseconds", testedMutants
                    .AverageOrZero(mut => mut.CreationTimeMilis)),
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
                                    new XAttribute("State", mutant.State),
                                    new XAttribute("CreationTimeMiliseconds", mutant.CreationTimeMilis),
                                    new XAttribute("TestingTimeMiliseconds", mutant.MutantTestSession.TestingTimeMiliseconds),
                                    new XAttribute("TestingEndRelativeSeconds", mutant.MutantTestSession.TestingEndRelative.TotalSeconds),
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

                optionalElements.Add(CreateCodeDifferenceListings(mutants).Result);
            }

            if (includeDetailedTestResults)
            {
                optionalElements.Add(CreateDetailedTestingResults(mutants));
            }

            return
                new XDocument(
                    new XElement("MutationTestingSession",
                        new XAttribute("SessionCreationWindowShowTime", _creationController.SessionCreationWindowShowTime),
                        new XAttribute("SessionStartTime", _sessionController.SessionStartTime),
                        new XAttribute("SessionEndTime", _sessionController.SessionEndTime),
                        new XAttribute("SessionRunTimeSeconds", (_sessionController.SessionEndTime
                        - _sessionController.SessionStartTime).TotalSeconds),
                        new XAttribute("MutationScore", session.MutationScore),
                        mutantsNode,
                        optionalElements));
            
        }
        public async Task<XElement> CreateCodeDifferenceListings(List<Mutant> mutants)
        {
            var list = new List<XElement>();
            foreach (var mutant in mutants)
            {
                var diff = await _codeDifferenceCreator.CreateDifferenceListing(CodeLanguage.CSharp,
                    mutant);
                var el = new XElement("MutantCodeListing",
                    new XAttribute("MutantId", mutant.Id),

                    new XElement("Code",
                        Environment.NewLine + diff.Code));
                list.Add(el);
            }
            return new XElement("CodeDifferenceListings", list);


        }

        public XElement CreateDetailedTestingResults(List<Mutant> mutants)
        {
            return new XElement("DetailedTestingResults");/*,   
                from mutant in mutants
                where mutant.MutantTestSession.IsComplete
                let groupedTests = mutant.MutantTestSession.TestsRootNode.TestNodeAssemblies 
                .SelectMany(n => n.TestNamespaces)
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
                )); */
        }
           
    }
}