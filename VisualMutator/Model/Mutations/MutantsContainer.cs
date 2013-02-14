namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
 
    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Exceptions;
    using VisualMutator.Model.Mutations.Structure;

    using log4net;


    #endregion

    public interface IMutantsContainer
    {
        MutationTestingSession PrepareSession(MutationSessionChoices choices);

        void GenerateMutantsForOperators(MutationTestingSession session, ProgressCounter progress );

        Mutant CreateChangelessMutant(MutationTestingSession session);

        void SaveMutantsToDisk(MutationTestingSession currentSession);

        void ExecuteMutation( Mutant mutant, IList<IModule> modules,
                        ICommonCompilerAssemblies env, IList<TypeIdentifier> allowedTypes,
                        ProgressCounter percentCompleted);
    }

    public class MutantsContainer : IMutantsContainer
    {
        private readonly ICommonCompilerAssemblies _assembliesManager;
        private readonly IAssembliesManager _assembliesManagerOld;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MutantsContainer(ICommonCompilerAssemblies assembliesManager, IAssembliesManager assembliesManagerOld
            )
        {
            _assembliesManager = assembliesManager;
            _assembliesManagerOld = assembliesManagerOld;
        }

        public MutationTestingSession PrepareSession(MutationSessionChoices choices)
        {
            var copiedModules = new StoredAssemblies(choices.Assemblies.Select(a => a.AssemblyDefinition)
                                                         .Select(_assembliesManager.Copy).Cast<IModule>().ToList());


            List<TypeIdentifier> copiedTypes = choices.SelectedTypes.Types.Select(t => new TypeIdentifier(t)).ToList();//
     
            return new MutationTestingSession
            {
                OriginalAssemblies = _assembliesManagerOld.Load(copiedModules.Modules),//null,//TODO: something 
                StoredSourceAssemblies = copiedModules,
                SelectedTypes = copiedTypes,
                Choices = choices,

            };
        }

        public Mutant CreateChangelessMutant(MutationTestingSession session)
        {
    
            var op = new PreOperator();
            var targets = FindTargets(op, session.StoredSourceAssemblies.Modules, new List<TypeIdentifier>());
          //  CreateMutantsForOperator(targets,  () => 0, ProgressCounter.Inactive());
            var mutant = new Mutant(0, targets.ExecutedOperator, new MutationTarget(-1));
            targets.ExecutedOperator.Children.Add(mutant);
           // var mutant = targets.ExecutedOperator.Mutants.First();
            var copiedModules = session.StoredSourceAssemblies.Modules
                                                         .Select(_assembliesManager.Copy).Cast<IModule>().ToList();
            mutant.MutatedModules = copiedModules;
            return mutant;
        }

        public void SaveMutantsToDisk(MutationTestingSession currentSession)
        {
            
        }

       
        public void GenerateMutantsForOperators(MutationTestingSession session, ProgressCounter percentCompleted )
        {
            session.MutantsGroupedByOperators = new List<ExecutedOperator>();
            var root = new MutationRootNode();

            int[] id = { 1 };
            Func<int> genId = () => id[0]++;


            percentCompleted.Initialize(session.Choices.SelectedOperators.Count);

            var sw = new Stopwatch();
        

            List<OperatorWithTargets> operatorsWithTargets = session.Choices.SelectedOperators
                .Select(oper =>
                {
                    percentCompleted.Progress();
                    sw.Restart();
                    var targets = FindTargets(oper, session.StoredSourceAssemblies.Modules, session.SelectedTypes.ToList());
                    targets.ExecutedOperator.FindTargetsTimeMiliseconds =  sw.ElapsedMilliseconds;
                    return targets;

                }).ToList();


            int times = session.Choices.MutantsCreationOptions.CreateMoreMutants ? 20 : 1;
            int allMutantsCount = operatorsWithTargets.Sum(op => op.MutationTargets.Count) * times;
            percentCompleted.Initialize(allMutantsCount);

            foreach (var op in operatorsWithTargets)
            {

                ExecutedOperator executedOperator = op.ExecutedOperator;
                sw.Restart();
                for (int i = 0; i < times; i++)
                {
                    CreateMutantsForOperator(op,  genId, percentCompleted);
                }
                sw.Stop();
                executedOperator.MutationTimeMiliseconds = sw.ElapsedMilliseconds;

                executedOperator.UpdateDisplayedText();

                session.MutantsGroupedByOperators.Add(executedOperator);
                executedOperator.Parent = root;
                root.Children.Add(executedOperator);
                
        
            }
            root.State = MutantResultState.Untested;

         //   _assembliesManager.SessionEnded();


        
        }

        public class OperatorWithTargets
        {
            public IDictionary<string, List<MutationTarget>> MutationTargets
            {
                get;
                set;
            }

            public IMutationOperator Operator { get; set; }

            public ExecutedOperator ExecutedOperator { get; set; }
        }


        public OperatorWithTargets FindTargets(IMutationOperator mutOperator, IList<IModule> modules, IList<TypeIdentifier> allowedTypes)
        {
            var result = new ExecutedOperator(mutOperator.Identificator, mutOperator.Name, mutOperator);

            try
            {
                var map = new Dictionary<string, List<MutationTarget>>();
                OperatorCodeVisitor operatorVisitor = mutOperator.FindTargets();
                foreach (var module in modules)
                {
             
                    var visitor = new VisualCodeVisitor(operatorVisitor);
                    operatorVisitor.MarkMutationTarget = visitor.MarkMutationTarget;
                    var traverser = new VisualCodeTraverser(allowedTypes, visitor);
                       // .Where(i => i.ModuleName == module.PersistentIdentifier).ToList())
                  
                    traverser.Traverse(module);

                    map.Add(module.ModuleName.Value, visitor.MutationTargets);
                }
                
           

                return new OperatorWithTargets
                {
                    MutationTargets = map,
                    Operator = mutOperator,
                    ExecutedOperator = result,
                };

                
            }
            catch (Exception e)
            {
                throw new MutationException("FindTargets failed on operator: {0}.".Formatted(mutOperator.Name), e);
            }
            

        }

        public void ExecuteMutation(Mutant mutant, IList<IModule> sourceModules, 
            ICommonCompilerAssemblies env, IList<TypeIdentifier> allowedTypes, ProgressCounter percentCompleted)
        {
            try
            {
                var copiedModules = sourceModules.Select(_assembliesManager.Copy).Cast<IModule>().ToList();
                mutant.MutatedModules = new List<IModule>();
                foreach (var module in copiedModules)
                {
                    percentCompleted.Progress();
                    var visitor2 = new VisualCodeVisitorBack(mutant.MutationTarget.InList());
                    var traverser2 = new VisualCodeTraverser(allowedTypes, visitor2);
                    traverser2.Traverse(module);

                    var operatorCodeRewriter = mutant.ExecutedOperator.Operator.Mutate();
                    operatorCodeRewriter.MutationTarget = mutant.MutationTarget;
                    var rewriter = new VisualCodeRewriter(env.Host, visitor2.MutationTargetsElements, allowedTypes, operatorCodeRewriter);
                    IModule rewrittenModule = rewriter.Rewrite(module);
                    mutant.MutatedModules.Add(rewrittenModule);
                }
                
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutant.ExecutedOperator.Operator.Name), e);
            }
        }


        private void CreateMutantsForOperator(OperatorWithTargets oper,
            Func<int> generateId, ProgressCounter percentCompleted)
        {
            foreach (MutationTarget mutationTarget in oper.MutationTargets.Values.SelectMany(v => v))
            {//TODO:progress
                //percentCompleted.Progress();
                var mutant = new Mutant(generateId(), oper.ExecutedOperator, mutationTarget);
                oper.ExecutedOperator.Children.Add(mutant);

            }
            /*

            var results = new List<MutationContext>();
            try
            {
                var visitor2 = new VisualCodeVisitorBack(visitor.MutationTargets);
                var traverser2 = new VisualCodeTraverser(allowed)
                {
                    PreorderVisitor = visitor2,
                };
                traverser2.Traverse(mutableModule);

                var stringList = myvisitor.AllElements.Select(elem => elem.MethodType + " ==== "
                    + elem.Obj.GetType().ToString() + " --- " + elem.Obj.ToString());
                //var builder = new StringBuilder();
                foreach (var str in stringList)
                {
                    Console.WriteLine(str);
                }

                Assert.AreEqual(visitor2.MutationTargetsElements.Count, 1);
                //Rewrite the mutable Code Model. In a real application CodeRewriter would be a subclass that actually does something.
                //(This is why decompiled source method bodies must recompile themselves, rather than just use the IL from which they were decompiled.)
                var rewriter = new VisualCodeRewriter(host, visitor2.MutationTargetsElements, allowed, new MyRewriter());
                IModule rewrittenModule = rewriter.Rewrite(mutableModule);


                
                foreach (MutationTarget mutationTarget in oper.MutationTargets)
                {
                    percentCompleted.Progress();
                    var assembliesToMutate = _assembliesManager.Load(sourceAssemblies);
                    var context = new MutationContext(mutationTarget, assembliesToMutate);
                    oper.Operator.Mutate(context);
                    results.Add(context);

                }
                
               
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(oper.Operator.Name), e);
            }

            foreach (MutationContext mutationResult in results)
            {
                var mutant = new Mutant(generateId(), oper.ExecutedOperator, mutationResult.MutationTarget);
                oper.ExecutedOperator.Children.Add(mutant);
            }
            */
       
        }
    }
}