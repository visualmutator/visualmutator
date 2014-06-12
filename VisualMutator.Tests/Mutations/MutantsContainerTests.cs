namespace VisualMutator.Tests.Mutations
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using NUnit.Framework;
    using OperatorsStandard.Operators;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using Util;
    using MethodIdentifier = Extensibility.MethodIdentifier;

    #endregion

    [TestFixture]
    public class MutantsContainerTests
    {
        [Test]
        public void Test0()
        {

            var cci = new CciModuleSource(TestProjects.DsaPath);
            var cci2 = new CciModuleSource(TestProjects.DsaPath);
            var type = cci.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Deque") as NamedTypeDefinition;
            var method = type.Methods.Single(m => m.Name.Value == "EnqueueFront");
            var choices = new MutationSessionChoices
                          {
                              Filter = new MutationFilter(
                                  new List<TypeIdentifier>(), 
                                  //new TypeIdentifier(type).InList(), 
                                  new MethodIdentifier(method).InList()),
                              SelectedOperators = new AOR_ArithmeticOperatorReplacement().InList<IMutationOperator>(),
                              WhiteSource = cci,
                              MutantsCreationOptions = new MutantsCreationOptions
                                                       {
                                                           MaxNumerOfMutantPerOperator = 100
                                                       }
                          };
            var container = new MutantsContainer(choices, new OperatorUtils());
            IList<AssemblyNode> assemblies = container.InitMutantsForOperators(ProgressCounter.Inactive());

            var mut = assemblies.Cast<CheckedNode>()
                .SelectManyRecursive(n => n.Children?? new NotifyingCollection<CheckedNode>())
                .OfType<Mutant>().First();

            var sourceMethod = type.Methods.Single(m => m.Name.Value == "EnqueueFront");


            MutationResult executeMutation = container.ExecuteMutation(mut, cci2);


     //       var viss = new Viss(cci2.Host, sourceMethod);
       //     IModule newMod = viss.Rewrite(executeMutation.MutatedModules.Modules.Single().Module);

            cci2.ReplaceWith(executeMutation.MutatedModules.Modules.Single().Module);

            MutationResult executeMutation2 = container.ExecuteMutation(mut, cci2);
        }
        private class Viss : CodeRewriter
        {
            private readonly IMethodDefinition _sourceMethod;

            public Viss(IMetadataHost host, IMethodDefinition sourceMethod) 
                : base(host, false)
            {
                _sourceMethod = sourceMethod;
            }

            public override IMethodDefinition Rewrite(IMethodDefinition method)
            {
                if (method.Name.Value == _sourceMethod.Name.Value)
                {
                    return _sourceMethod;
                }
                return method;
            }
        }

        [Test]
        public void Test1()
        {
            //TODO: ReadAssembly does not work on this artificial assembly

            /*
            var t1 = CecilUtils.CreateTypeDefinition("ns1", "Type1");
            var t2 = CecilUtils.CreateTypeDefinition("ns1", "Type2");
            var t3 = CecilUtils.CreateTypeDefinition("ns1", "Type3");
            
            var types = new List<TypeDefinition>
            {
                t1,t2,t3
            };

            var assembly = CecilUtils.CreateAssembly("ass", types);
            var assemblies = new[] { assembly };

            t1.Methods.Add(CecilUtils.CreateMethodDefinition("Method1", t1));
            t3.Methods.Add(CecilUtils.CreateMethodDefinition("Method2", t3));

            var assembliesManager = new AssembliesManager();

            var mutantsContainer = new MutantsContainer(assembliesManager);

            var choices = new MutationSessionChoices
            {
                Assemblies = assemblies,
                SelectedTypes = types, 
                SelectedOperators = new[] { new TestOperator() }
            };
            // Act
            var mutationTestingSession = mutantsContainer.Initialize(choices);
            mutantsContainer.InitMutantsForOperators(mutationTestingSession);
            var executedOperator = mutationTestingSession.MutantsGroupedByOperators.Single();

            // Assert
            executedOperator.Name.ShouldEqual("TestOperatorName");
            executedOperator.Mutants.Count().ShouldEqual(2);

            assembliesManager.Load(executedOperator.Mutants.First().StoredAssemblies).Single()
                .MainModule.Types.Single(t => t.Name == "Type1").Methods.Single().Name.ShouldEqual("MutatedMethodName0");
*/
        }
    }
}