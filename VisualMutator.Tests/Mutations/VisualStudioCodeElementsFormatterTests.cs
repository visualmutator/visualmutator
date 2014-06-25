namespace VisualMutator.Tests.Mutations
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using Model.CoverageFinder;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using VisualMutator.Infrastructure;

    #endregion

    [TestFixture("Ns.Class.Method1", "System.String", "Ns.Class.Method1(System.String)")]
    [TestFixture("Ns.Class.Method1<T>", "", "Ns.Class.Method1<T>()")]
    [TestFixture("Ns.Class.InnerClass<K, Y>.Met2<R>", "K; System.Int32", "Ns.Class.InnerClass<K, Y>.Met2<R>(K, System.Int32)")]
    [TestFixture("Ns.Class<S>.Inner.Inner2.Met2<R,L>", "", "Ns.Class<S>.Inner.Inner2.Met2<R,L>()")]
    [TestFixture("Ns.Class<S>.Inner.Inner2.Inner2", "", "Ns.Class<S>.Inner.Inner2..ctor()")]
    [TestFixture("Ns.Class<S>.Class", "System.Int32", "Ns.Class<S>..ctor(System.Int32)")]
    public class VisualStudioCodeElementsFormatterTests
    {
        private readonly string _methodName;
        private readonly string _expected;
        private readonly List<string> _params;

        public VisualStudioCodeElementsFormatterTests(string methodName, string paramsEncoded, string expected)
        {
            _methodName = methodName;
            _expected = expected;
            
            _params = paramsEncoded.Length != 0 ? paramsEncoded.Split(';').ToList() : new List<string>();
        }

        [Test]
        public void Test1()
        {
            var formatter = new VisualStudioCodeElementsFormatter();

            MethodIdentifier methodIdentifier = formatter.CreateIdentifier(_methodName, _params);

            methodIdentifier.ToString().ShouldEqual(_expected);
        }
    }
}