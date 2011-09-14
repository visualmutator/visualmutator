namespace VisualMutator.Tests.UnitTesting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    using Moq;

    using NUnit.Core;
    using NUnit.Util;

    using PiotrTrzpil.VisualMutator_VSPackage.Model.Tests;

    public static class TestWrapperMocking
    {
        public static ITest MockTest(string name, ITest parentFixture)
        {
            var m = new Mock<ITest>();

            var tn = new TestName
            {
                FullName = name,
                Name = name
            };
            var ptn = new TestName
            {
                FullName = parentFixture.TestName.FullName,
                Name = parentFixture.TestName.Name,
            };


            m.Setup(_ => _.TestName).Returns(tn);
            m.Setup(_ => _.Parent.TestName).Returns(ptn);
            m.Setup(_ => _.TestType).Returns("Test");
            return m.Object;
        }
        public static ITest MockTestFixture(string name, string namespaceName)
        {
            var m = new Mock<ITest>();

            var tn = new TestName
            {
                FullName = name,
                Name = name
            };
            var ptn = new TestName
            {
                FullName = namespaceName,
                Name = namespaceName
            };


            m.Setup(_ => _.TestName).Returns(tn);
            m.Setup(_ => _.Parent.TestName).Returns(ptn);
            m.Setup(_ => _.Tests).Returns(new ArrayList());
            m.Setup(_ => _.TestType).Returns("TestFixture");
            return m.Object;
        }

        public static Mock<INUnitWrapper> MockNUnitWrapperForLoad(out List<ITest> testClasses)
        {
            var wrapperMock = new Mock<INUnitWrapper>();

            var list = new List<ITest>();
            wrapperMock.Setup(_ => _.TestLoaded).Returns(list.ToObservable());
            wrapperMock.Setup(_ => _.TestLoadFailed).Returns(new List<TestEventArgs>().ToObservable());
            testClasses = list;
            return wrapperMock;
        }


      
        public static MethodDefinition CreateMethodDefinition(string name, TypeDefinition type)
        {
            var s = new TypeReference("System", "Void", null, null);
            return new MethodDefinition(name, MethodAttributes.Public, s)
            {
                DeclaringType = type
            };
        }

        public static AssemblyDefinition CreateAssembly(string assemblyName, IEnumerable<TypeDefinition> types)
        {
            var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(assemblyName, new Version()),
               "TestModule", ModuleKind.Console);
            foreach (var typeDefinition in types)
            {
                assembly.MainModule.Types.Add(typeDefinition);
            }
            return assembly;
        }
    }
}

