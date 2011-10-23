namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;

    using Mono.Cecil;


    using VisualMutator.Extensibility;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    using log4net;

    #endregion

    public class MutationSessionChoices
    {
        public IList<IMutationOperator> SelectedOperators
        {
            get;
            set;
        }

        public IList<AssemblyDefinition> Assemblies
        {
            get; set;
        }

        public IList<TypeDefinition> SelectedTypes
        {
            get;
            set;
        }
    }
    public class MutantsCreationController : Controller
    {
      
     
        private readonly IMutantsContainer _mutantsContainer;

        private readonly IOperatorsManager _operatorsManager;

        private readonly CommonServices _commonServices;

        private readonly ITypesManager _typesManager;

        private readonly MutantsCreationViewModel _viewModel;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MutationSessionChoices Result { get; set; }

        public bool HasResults
        {
            get
            {
                return Result != null;
            }
        }

        public MutantsCreationController(
            MutantsCreationViewModel viewModel,
            IMutantsContainer mutantsContainer,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            CommonServices commonServices)
        {
            _viewModel = viewModel;
    
            _mutantsContainer = mutantsContainer;
            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _commonServices = commonServices;


            _viewModel.CommandCreateMutants = new BasicCommand(StoreChoicesResults, () => _viewModel.Assemblies != null
                && _viewModel.MutationPackages != null);
            _viewModel.CommandCreateMutants.UpdateOnChanged(_viewModel, () => _viewModel.Assemblies);
            _viewModel.CommandCreateMutants.UpdateOnChanged(_viewModel, () => _viewModel.MutationPackages);
      

           // _viewModel.Assemblies = new BetterObservableCollection<AssemblyNode>();
          //  _viewModel.MutationPackages = _operatorsManager.OperatorPackages;

           
        }
        /*
        public void LoadLastMutant()
        {
            _services.EventPassing.Publish(new LoadLastCreatedMutantEventArgs());
            _services.EventPassing.Publish(new SwitchToUnitTestsTabEventArgs());


        }
        */
       

        public void Run()
        {
            
            _commonServices.Threading.ScheduleAsync(()=> _operatorsManager.LoadOperators(),
                packages => _viewModel.MutationPackages = new ReadOnlyCollection<PackageNode>(packages));

            _commonServices.Threading.ScheduleAsync(() => _typesManager.GetTypesFromAssemblies(),
                assemblies => _viewModel.Assemblies = new ReadOnlyCollection<AssemblyNode>(assemblies));

            _viewModel.ShowDialog();
        }
        public void StoreChoicesResults()
        {
            Result = new MutationSessionChoices
            {
                SelectedOperators = _viewModel.MutationPackages.SelectMany(pack => pack.Operators)
                                 .Where(oper => oper.IsLeafIncluded).Select(n=>n.Operator).ToList(),
                Assemblies = _viewModel.Assemblies.Select(a => a.AssemblyDefinition).ToList(),
                SelectedTypes = _typesManager.GetIncludedTypes(_viewModel.Assemblies)
            };
            _viewModel.Close();
        }

        /*
        public void Mutate()
        {
            
            _viewModel.IsMutationOngoing = true;
            _viewModel.ClearMutationLog();
            _viewModel.MutationLog("Starting mutation.");


            Action<string> mutationLog = _services.Threading.ActionOnGui<string>(text => _viewModel.MutationLog(text));

            _services.Threading.ScheduleAsync(
                () =>
                {
                    try
                    {
                        return _mutantsContainer.GenerateMutant("Mutant", mutationLog);
                    }
                    catch (MutationException e)
                    {
                        
                        throw new NonFatalWrappedException(e);
                    }
                    
                },
                onGui: result =>
                {
                    _mutantsContainer.AddMutant(result);
                    _viewModel.MutationLog("Mutation complete.");
                },
                onException: () =>
                {
                    _viewModel.MutationLog("Mutation failed.");
                },
                onFinally: () => _viewModel.IsMutationOngoing = false);

           
        }*/

       
        /*
        public void RefreshTypes()
        {
            _viewModel.AreTypesLoading = true;
            _viewModel.Assemblies.Clear();

            _services.Threading.ScheduleAsync(
              () =>
              {
                  return _typesManager.GetTypesFromAssemblies();
              },
              onGui: result =>
              {
                  _viewModel.Assemblies.ReplaceRange(result);
              },
              onFinally: () =>_viewModel.AreTypesLoading = false);

        }
        */

       
    }
}