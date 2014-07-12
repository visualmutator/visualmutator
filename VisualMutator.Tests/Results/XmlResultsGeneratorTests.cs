namespace VisualMutator.Tests.Results
{
    using System;
    using System.Xml.Linq;
    using Extensibility;
    using Model;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Types;
    using NUnit.Framework;


    [TestFixture]
    public class XmlResultsGeneratorTests
    {
        [Test]
        public void Test()
        {
             var gen = new XmlResultsGenerator(null,null,null, null);

            var muSession = new MutationTestingSession();
            
            var mutar = new MutationTarget(new MutationVariant());


            var ass = new AssemblyNode("Assembly");
            muSession.MutantsGrouped.Add(ass);
            var nodeNamespace = new TypeNamespaceNode(ass, "Namespace");
            ass.Children.Add(nodeNamespace);
            var nodeType = new TypeNode(nodeNamespace, "Type");
            nodeNamespace.Children.Add(nodeType);
            var nodeMethod = new MethodNode(nodeType, "Method", null, true);
            nodeType.Children.Add(nodeMethod);
            var nodeGroup = new MutantGroup("Gr1", nodeMethod);
            nodeMethod.Children.Add(nodeGroup);
            var nodeMutant = new Mutant("m1", nodeGroup, mutar);
            nodeGroup.Children.Add(nodeMutant);

            XDocument generateResults = gen.GenerateResults(muSession, false, false);

            Console.WriteLine(generateResults.ToString());
            //gen.
        }
    }
}