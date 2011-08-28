namespace VisualMutator.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using NUnit.Framework;

    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;

    [TestFixture]
    public class SolutionTypesTests
    {

        [Test]
        public void Test1()
        {



            var manager = new SolutionTypesManager();




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
          //  IDictionary<
            var dict = new Dictionary<string, IEnumerable<TypeDefinition>>();
            dict.Add("proj", list);

            manager.BuildTypesTree(dict);


            var one = manager.Assemblies.Single().Children.Single();

            one.Children.Count.ShouldEqual(5);

            var two = one.Children.Single(_ => _.Name == "Two");

            two.Children.Count.ShouldEqual(3);


            var  merged = one.Children.Single(_ => _.Name == "ZZZ.YYY.Four");

            merged.Children.Count.ShouldEqual(2);

        }

        [Test]
        public void Test2()
        {

            var manager = new SolutionTypesManager();

           
            manager.ExtractNextNamespacePart("One.Two.Three.Four.Five", "").ShouldEqual("One");
            manager.ExtractNextNamespacePart("One.Two.Three", "").ShouldEqual("One");
            manager.ExtractNextNamespacePart("One", "").ShouldEqual("One");

            manager.ExtractNextNamespacePart("One.Two.Three.Four.Five", "One.Two").ShouldEqual("Three");
            manager.ExtractNextNamespacePart("One.Two.Three", "One.Two").ShouldEqual("Three");
         


        }
    }
}

