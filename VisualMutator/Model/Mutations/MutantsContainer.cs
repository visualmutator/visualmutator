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
    using MutantsTree;
    using Operators;
    using StoringMutants;
    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Exceptions;
    using log4net;
    using Module = Microsoft.Cci.MutableCodeModel.Module;

    #endregion

    public interface IMutantsContainer
    {
        MutationTestingSession PrepareSession(MutationSessionChoices choices);

        void GenerateMutantsForOperators(MutationTestingSession session, ProgressCounter progress );

        Mutant CreateChangelessMutant(out ExecutedOperator executedOperator);

        void SaveMutantsToDisk(MutationTestingSession currentSession);

        AssembliesProvider ExecuteMutation( Mutant mutant, IList<IModule> modules, IList<TypeIdentifier> allowedTypes,
                        ProgressCounter percentCompleted);
    }

    public class MutantsContainer : IMutantsContainer
    {
        private readonly ICommonCompilerAssemblies _assembliesManager;
        private readonly IOperatorUtils _operatorUtils;
        private readonly IAssembliesManager _assembliesManagerOld;
        private bool _debugConfig ;

        public bool DebugConfig
        {
            get { return _debugConfig; }
            set { _debugConfig = value; }
        }

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MutantsContainer(ICommonCompilerAssemblies assembliesManager, 
            IOperatorUtils operatorUtils,
            IAssembliesManager assembliesManagerOld
            )
        {
            _assembliesManager = assembliesManager;
            _operatorUtils = operatorUtils;
            _assembliesManagerOld = assembliesManagerOld;
        }

        public MutationTestingSession PrepareSession(MutationSessionChoices choices)
        {
            var copiedModules = new StoredAssemblies(choices.Assemblies.Select(a => a.AssemblyDefinition)
                                                         .Select(_assembliesManager.Copy).Cast<IModule>().ToList());


            List<TypeIdentifier> copiedTypes = choices.SelectedTypes.Types.Select(t => new TypeIdentifier(t)).ToList();//
     
            return new MutationTestingSession
            {
                OriginalAssemblies = _assembliesManagerOld.Load(copiedModules.Modules),
                StoredSourceAssemblies = copiedModules,
                SelectedTypes = copiedTypes,
                Choices = choices,

            };
        }

        public Mutant CreateChangelessMutant(out ExecutedOperator executedOperator)
        {
            var op = new PreOperator();
          //  var targets = FindTargets(op, session.StoredSourceAssemblies.Modules, new List<TypeIdentifier>());
            executedOperator = new ExecutedOperator(op.Info.Id, op.Info.Name, op);
             
            var mutant = new Mutant("0", executedOperator, new MutationTarget("", -1, 0, "",""), new List<MutationTarget>());
            executedOperator.Children.Add(mutant);
        
         //   var copiedModules = session.StoredSourceAssemblies.Modules
         //                                                .Select(_assembliesManager.Copy).Cast<IModule>().ToList();
         //   mutant.MutatedModules = copiedModules;
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
            var subProgress = percentCompleted.CreateSubprogress();

            var sw = new Stopwatch();


            var operatorsWithTargets = new List<OperatorWithTargets>();
            foreach (var oper in session.Choices.SelectedOperators)
            {
                var executedOperator = new ExecutedOperator(oper.Info.Id, oper.Info.Name, oper);

                sw.Restart();

                OperatorWithTargets targets = FindTargets(oper, session.StoredSourceAssemblies.Modules, session.SelectedTypes.ToList());

                executedOperator.FindTargetsTimeMiliseconds = sw.ElapsedMilliseconds;

                operatorsWithTargets.Add(targets);

                //subProgress.Initialize(targets.MutationTargets.Count);
                foreach (MutationTarget mutationTarget in targets.MutationTargets.Values.SelectMany(v => v))
                {
                    var mutant = new Mutant(genId().ToString(), executedOperator, mutationTarget, targets.CommonTargets);
                    executedOperator.Children.Add(mutant);

                    //subProgress.Progress();
                }

                executedOperator.UpdateDisplayedText();
                session.MutantsGroupedByOperators.Add(executedOperator);
                executedOperator.Parent = root;
                root.Children.Add(executedOperator);

                percentCompleted.Progress();
            }
                
       
            root.State = MutantResultState.Untested;

   
        }

       

        public OperatorWithTargets FindTargets(IMutationOperator mutOperator, IList<IModule> modules, 
            IList<TypeIdentifier> allowedTypes)
        {
           

            _log.Info("Finding targets for mutation operator: " + mutOperator.Info);

            try
            {
                var commonTargets = new List<MutationTarget>();
                var map = new Dictionary<string, List<MutationTarget>>();
                var ded = mutOperator.FindTargets();
                IOperatorCodeVisitor operatorVisitor = ded;
                operatorVisitor.Host = _assembliesManager.Host;
                operatorVisitor.OperatorUtils = _operatorUtils;
                operatorVisitor.Initialize();
                foreach (var module in modules)
                {
                    var visitor = new VisualCodeVisitor(operatorVisitor);
 
                    var traverser = new VisualCodeTraverser(allowedTypes, visitor);
                  
                    traverser.Traverse(module);

                    map.Add(module.ModuleName.Value, visitor.MutationTargets);
                    commonTargets.AddRange(visitor.CommonTargets);
                }

       
                _log.Info("Got: " + map.Values.Flatten().Count()+" mutation targets.");
         
                return new OperatorWithTargets
                {
                    CommonTargets = commonTargets,
                    MutationTargets = map,
                    Operator = mutOperator,
              
                };

                
            }
            catch (Exception e)
            {
                if (!DebugConfig)
                {
                    throw new MutationException("FindTargets failed on operator: {0}.".Formatted(mutOperator.Info.Name), e);
                }
                else
                {
                    throw;
                }
            }
            

        }

        public AssembliesProvider ExecuteMutation(Mutant mutant, IList<IModule> sourceModules, IList<TypeIdentifier> allowedTypes, ProgressCounter percentCompleted)
        {
            try
            {
                _log.Info("Execute mutation of " + mutant.MutationTarget + " on " + sourceModules.Count + " modules. Allowed types: " + allowedTypes.Count);
                var copiedModules = sourceModules.Select(_assembliesManager.Copy).Cast<IModule>().ToList();
                var mutatedModules = new List<IModule>();
                foreach (var module in copiedModules)
                {
                    percentCompleted.Progress();
                    var visitor2 = new VisualCodeVisitorBack(mutant.MutationTarget.InList(), mutant.CommonTargets);
                    var traverser2 = new VisualCodeTraverser(allowedTypes, visitor2);
                    traverser2.Traverse(module);

                    var operatorCodeRewriter = mutant.ExecutedOperator.Operator.Mutate();
                    operatorCodeRewriter.MutationTarget = mutant.MutationTarget;
                    operatorCodeRewriter.NameTable = _assembliesManager.Host.NameTable;
                    operatorCodeRewriter.Host = _assembliesManager.Host;
                    operatorCodeRewriter.Module = (Module)module;
                    operatorCodeRewriter.OperatorUtils = _operatorUtils;

                    operatorCodeRewriter.Initialize();

                    var rewriter = new VisualCodeRewriter(_assembliesManager.Host, visitor2.MutationTargetsElements
                        , visitor2.CommonTargetsElements, allowedTypes, operatorCodeRewriter);
                    IModule rewrittenModule = rewriter.Rewrite(module);



                    mutatedModules.Add(rewrittenModule);
                }
                return new AssembliesProvider( mutatedModules);
            }
            catch (Exception e)
            {
                if (!DebugConfig)
                {
                    throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutant.ExecutedOperator.Operator.Info.Name), e);
                }
                else
                {
                    throw;
                }
                
            }
        }
        public class OperatorWithTargets
        {
            public IDictionary<string, List<MutationTarget>> MutationTargets
            {
                get;
                set;
            }

            public IMutationOperator Operator
            {
                get;
                set;
            }



            public List<MutationTarget> CommonTargets
            {
                get;
                set;
            }

        }


    }
}