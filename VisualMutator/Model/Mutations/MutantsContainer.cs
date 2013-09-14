namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using CommonUtilityInfrastructure;
    using LinqLib.Sequence;
    using Microsoft.Cci;
    using MutantsTree;
    using Operators;
    using OperatorsStandard;
    using StoringMutants;
    using Types;
    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Exceptions;
    using log4net;
    using Module = Microsoft.Cci.MutableCodeModel.Module;

    #endregion

    public interface IMutantsContainer
    {
        MutationTestingSession PrepareSession(MutationSessionChoices choices);

       
        Mutant CreateChangelessMutant(out ExecutedOperator executedOperator);

        void SaveMutantsToDisk(MutationTestingSession currentSession);

        ModulesProvider ExecuteMutation(Mutant mutant, IList<AssemblyNode> modules, IList<TypeIdentifier> allowedTypes, ProgressCounter percentCompleted);

        MutantsContainer.OperatorWithTargets FindTargets(IMutationOperator oper, IList<IModule> assemblies, IList<TypeIdentifier> toList);

        List<ExecutedOperator> InitMutantsForOperators(ICollection<IMutationOperator> operators,
                                                                           ICollection<TypeIdentifier> allowedTypes, ModulesProvider originalModules, ProgressCounter percentCompleted);
    }

    public class MutantsContainer : IMutantsContainer
    {
        private readonly ICommonCompilerAssemblies _assembliesManager;
        private readonly IOperatorUtils _operatorUtils;

        private bool _debugConfig ;

        public bool DebugConfig
        {
            get { return _debugConfig; }
            set { _debugConfig = value; }
        }

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MutationSessionChoices _choices;

        public MutantsContainer(ICommonCompilerAssemblies assembliesManager, 
            IOperatorUtils operatorUtils
        
            )
        {
            _assembliesManager = assembliesManager;
            _operatorUtils = operatorUtils;

        }

        public MutationTestingSession PrepareSession(MutationSessionChoices choices)
        {
            _choices = choices;
         //   var copiedModules = new StoredAssemblies(choices.Assemblies.Select(a => a.AssemblyDefinition)
         //                                                .Select(_assembliesManager.Copy).Cast<IModule>().ToList());


            List<TypeIdentifier> copiedTypes = choices.SelectedTypes.Types.Select(t => new TypeIdentifier(t)).ToList();//
     
            return new MutationTestingSession
            {
                OriginalAssemblies = choices.Assemblies,
    
                SelectedTypes = copiedTypes,
                Choices = choices,

            };
        }

        public Mutant CreateChangelessMutant(out ExecutedOperator executedOperator)
        {
            var op = new PreOperator();
          
            executedOperator = new ExecutedOperator(op.Info.Id, op.Info.Name, op);
            var group = new MutantGroup("Testing original program", executedOperator);
            var mutant = new Mutant("0", group, new MutationTarget("Original program", -1, "", new MutationVariant()), new List<MutationTarget>());
           
            group.Children.Add(mutant);
            executedOperator.Children.Add(group);
        
            return mutant;
        }

        public void SaveMutantsToDisk(MutationTestingSession currentSession)
        {
            
        }


        public List<ExecutedOperator> InitMutantsForOperators(ICollection<IMutationOperator> operators,
            ICollection<TypeIdentifier> allowedTypes, ModulesProvider originalModules, ProgressCounter percentCompleted)
        {
            var mutantsGroupedByOperators = new List<ExecutedOperator>();
            var root = new MutationRootNode();

            int[] id = { 1 };
            Func<int> genId = () => id[0]++;


            percentCompleted.Initialize(operators.Count);
            var subProgress = percentCompleted.CreateSubprogress();

            var sw = new Stopwatch();


            var operatorsWithTargets = new List<OperatorWithTargets>();
            foreach (var oper in operators)
            {
                var executedOperator = new ExecutedOperator(oper.Info.Id, oper.Info.Name, oper);

                sw.Restart();

                OperatorWithTargets operatorResult = FindTargets(oper, originalModules.Assemblies, allowedTypes.ToList());

                executedOperator.FindTargetsTimeMiliseconds = sw.ElapsedMilliseconds;

                operatorsWithTargets.Add(operatorResult);
                var mutations = LimitMutationTargets(operatorResult);
                //subProgress.Initialize(targets.MutationTargets.Count);
                foreach (var grouping in mutations)
                {
                    var group = new MutantGroup(grouping.Key, executedOperator);
                    foreach (var mutationTarget in grouping)
                    {
                        var mutant = new Mutant(genId().ToString(), group, mutationTarget, operatorResult.CommonTargets);
                        group.Children.Add(mutant);

                    }
                    executedOperator.Children.Add(group);
                    group.UpdateDisplayedText();
                    //subProgress.Progress();
                }

                executedOperator.UpdateDisplayedText();
                mutantsGroupedByOperators.Add(executedOperator);
                executedOperator.Parent = root;
                root.Children.Add(executedOperator);

                percentCompleted.Progress();
            }
                
       
            root.State = MutantResultState.Untested;

            return mutantsGroupedByOperators;
        }

        private ILookup<string, MutationTarget> LimitMutationTargets(OperatorWithTargets operatorResult)
        {
            var mapping = operatorResult.MutationTargets
                .SelectMany(pair => pair.Item2.Select(t => Tuple.Create(pair.Item1, t))).Shuffle()
                .Take(_choices.MutantsCreationOptions.MaxNumerOfMutantPerOperator).ToList();
            return mapping.ToLookup(pair => pair.Item1, pair => pair.Item2);
        }


        public OperatorWithTargets FindTargets(IMutationOperator mutOperator, IList<IModule> modules, 
            IList<TypeIdentifier> allowedTypes)
        {
           

            _log.Info("Finding targets for mutation operator: " + mutOperator.Info);

            try
            {
                var commonTargets = new List<MutationTarget>();
              
                var ded = mutOperator.CreateVisitor();
                IOperatorCodeVisitor operatorVisitor = ded;
                operatorVisitor.Host = _assembliesManager.Host;
                operatorVisitor.OperatorUtils = _operatorUtils;
                operatorVisitor.Initialize();
                var mergedTargets = new List<Tuple<string /*GroupName*/, List<MutationTarget>>>();
                foreach (var module in modules)
                {
                    var visitor = new VisualCodeVisitor(operatorVisitor);
 
                    var traverser = new VisualCodeTraverser(allowedTypes, visitor);
                  
                    traverser.Traverse(module);
                    visitor.PostProcess();
                    IEnumerable<Tuple<string, List<MutationTarget>>> s = visitor.MutationTargets.AsEnumerable();
                    mergedTargets.AddRange(s);
                  
                    commonTargets.AddRange(visitor.SharedTargets);
                }


                _log.Info("Got: " + mergedTargets.Select(i => i.Item2).Flatten().Count() + " mutation targets.");
         
                return new OperatorWithTargets
                {
                    CommonTargets = commonTargets,
                    MutationTargets = mergedTargets,
                    Operator = mutOperator,
              
                };

                
            }
            catch (Exception e)
            {
                if (!DebugConfig)
                {
                    throw new MutationException("Finding targets operation failed in operator: {0}.".Formatted(mutOperator.Info.Name), e);
                }
                else
                {
                    throw;
                }
            }
            

        }

        public ModulesProvider ExecuteMutation(Mutant mutant, IList<AssemblyNode> sourceModules, IList<TypeIdentifier> allowedTypes, ProgressCounter percentCompleted)
        {
            try
            {
                _log.Info("Execute mutation of " + mutant.MutationTarget + " contained in " + mutant.MutationTarget.MethodRaw + " on " + sourceModules.Count + " modules. Allowed types: " + allowedTypes.Count);
                var cci = new CommonCompilerAssemblies();
                var mutatedModules = new List<IModule>();
                foreach (var sourceModule in sourceModules)
                {
                    IModule module = cci.AppendFromFile(sourceModule.AssemblyPath.ToString());
                    //  var copiedModules = sourceModules.Assemblies.Select(module => _assembliesManager.DecompileCopy(module)).ToList();
                    
                    percentCompleted.Progress();
                    var visitorBack = new VisualCodeVisitorBack(mutant.MutationTarget.InList(), mutant.CommonTargets);
                    var traverser2 = new VisualCodeTraverser(allowedTypes, visitorBack);
                    traverser2.Traverse(module);
                    visitorBack.PostProcess();
                    var operatorCodeRewriter = mutant.ExecutedOperator.Operator.CreateRewriter();

                    var rewriter = new VisualCodeRewriter(_assembliesManager.Host, visitorBack.TargetAstObjects, 
                        visitorBack.SharedAstObjects, allowedTypes, operatorCodeRewriter);

                    operatorCodeRewriter.MutationTarget =
                        new UserMutationTarget(mutant.MutationTarget.Variant.Signature, mutant.MutationTarget.Variant.AstObjects);
                    
                    operatorCodeRewriter.NameTable = _assembliesManager.Host.NameTable;
                    operatorCodeRewriter.Host = _assembliesManager.Host;
                    operatorCodeRewriter.Module = (Module)module;
                    operatorCodeRewriter.OperatorUtils = _operatorUtils;

                    operatorCodeRewriter.Initialize();

                    IModule rewrittenModule = rewriter.Rewrite(module);
                    mutatedModules.Add(rewrittenModule);
                }
                return new ModulesProvider(mutatedModules);
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
            public List<Tuple<string, List<MutationTarget>>> MutationTargets
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