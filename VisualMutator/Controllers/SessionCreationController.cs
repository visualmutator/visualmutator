namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model;
    using Model.Tests;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    using log4net;

    #endregion

    public class SessionCreationController : CreationController<SessionCreationViewModel, ISessionCreationView>
    {
        public SessionCreationController(SessionCreationViewModel viewModel, ITypesManager typesManager, IOperatorsManager operatorsManager, IVisualStudioConnection visualStudio, ITestsContainer testsContainer, CommonServices svc) : base(viewModel, typesManager, operatorsManager, visualStudio, testsContainer, svc)
        {
        }

        protected override void AcceptChoices()
        {
            Result = new MutationSessionChoices
            {
                SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                                 .Where(oper => oper.IsLeafIncluded).Select(n=>n.Operator).ToList(),
                Assemblies = _viewModel.TypesTreeMutate.Assemblies,
                ProjectPaths = _typesManager.ProjectPaths.ToList(),
                SelectedTypes = _typesManager.GetIncludedTypes(_viewModel.TypesTreeMutate.Assemblies),
                SelectedTests = _testsContainer.GetIncludedTests(_viewModel.TypesTreeToTest.Namespaces),
                MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                MutantsTestingOptions = _viewModel.MutantsTesting.Options,
            };
            _viewModel.Close();
        }


    }
}