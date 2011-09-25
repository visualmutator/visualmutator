namespace VisualMutator.Tests
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    using Moq;

    using NUnit.Framework;

    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Tests.Util;

    #endregion

    [TestFixture]
    public class SolutionTypesTests
    {
    

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

            var mock = new Mock<IAssemblyReaderWriter>();

            var assembly = CecilUtils.CreateAssembly("TestAssembly", list);
            mock.Setup(_ => _.ReadAssembly(path)).Returns(assembly);

            var manager = new SolutionTypesManager(mock.Object);

            // Act
            manager.GetTypesFromAssemblies(new[] { path });
            
            // Assert
            CollectionAssert.AreEquivalent(manager.GetIncludedTypes(), list);
            manager.GetLoadedAssemblies().Single().ShouldEqual(assembly);

            var one = manager.AssemblyTreeNodes.Single().Children.Single();
            one.Children.Count.ShouldEqual(5);
            var two = one.Children.Single(_ => _.Name == "Two");
            two.Children.Count.ShouldEqual(3);
            var merged = one.Children.Single(_ => _.Name == "ZZZ.YYY.Four");
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