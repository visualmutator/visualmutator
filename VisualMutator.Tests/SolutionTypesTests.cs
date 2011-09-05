namespace VisualMutator.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using Moq;

    using NUnit.Framework;

    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;

    using VisualMutator.Tests.Util;

    [TestFixture]
    public class SolutionTypesTests
    {
        public Mock<IAssemblyReaderWriter> MockAssemblyReaderWriter(IEnumerable<KeyValuePair<string,AssemblyDefinition>> assemblies)
        {
            var mock = new Mock<IAssemblyReaderWriter>();
            foreach (var pair in assemblies)
            {
                var pair1 = pair;
                mock.Setup(_ => _.ReadAssembly(pair1.Key)).Returns(pair.Value);
            }
            return mock;
        }
        public AssemblyDefinition CreateAssembly(string assemblyName, IEnumerable<TypeDefinition> types)
        {
            var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(assemblyName, new Version()),
               "TestModule", ModuleKind.Console);
            foreach (var typeDefinition in types)
            {
                assembly.MainModule.Types.Add(typeDefinition);
            }
            return assembly;
        }


        [Test]
        public void Test1()
        {
            
            var a = TypeAttributes.Public;
            var list = new List<TypeDefinition>
            {
                new TypeDefinition("One.Two.Three.Four.Five", "Type1", a),
                new TypeDefinition("One.Two.Three.Four.Five", "Type2", a),
                new TypeDefinition("One.Two.Three", "Type3", a),
                new TypeDefinition("One", "Type4", a),
                new TypeDefinition("One", "Type5", a),
                new TypeDefinition("One.Two", "Typy6", a),
                new TypeDefinition("One.Two.Three.Four", "Type7", a),
                new TypeDefinition("One.Two.Three.Four", "Type8", a),
                new TypeDefinition("One.Two.Three.Four", "Type9", a),
                new TypeDefinition("One.XXX.Three.Four", "Type10", a),
                new TypeDefinition("One.XXX.Three.Four", "Type11", a),
                new TypeDefinition("One.Two.YYY.Four", "Type12", a),
                new TypeDefinition("One.ZZZ.YYY.Four", "Type13", a),
                new TypeDefinition("One.ZZZ.YYY.Four", "Type14", a),

            };

            string path = @"C:\TestAssembly.dll";
            var dict = new Dictionary<string, AssemblyDefinition>();
            dict.Add(path, CreateAssembly("TestAssembly", list));

            var mock = MockAssemblyReaderWriter(dict);
            var manager = new SolutionTypesManager(mock.Object);

            manager.BuildTypesTree(new[] { path });


            var one = manager.AssemblyTreeNodes.Single().Children.Single();

            one.Children.Count.ShouldEqual(5);

            var two = one.Children.Single(_ => _.Name == "Two");

            two.Children.Count.ShouldEqual(3);


            var  merged = one.Children.Single(_ => _.Name == "ZZZ.YYY.Four");

            merged.Children.Count.ShouldEqual(2);

        }

        [Test]
        public void Test2()
        {

            var manager = new SolutionTypesManager(null);

           
            manager.ExtractNextNamespacePart("One.Two.Three.Four.Five", "").ShouldEqual("One");
            manager.ExtractNextNamespacePart("One.Two.Three", "").ShouldEqual("One");
            manager.ExtractNextNamespacePart("One", "").ShouldEqual("One");

            manager.ExtractNextNamespacePart("One.Two.Three.Four.Five", "One.Two").ShouldEqual("Three");
            manager.ExtractNextNamespacePart("One.Two.Three", "One.Two").ShouldEqual("Three");
         


        }
    }
}

