namespace VisualMutator.Tests.Infrastructure
{
    using Model;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;

    [TestFixture]
    public class ParserTests
    {
        [Test]
         public void TestOptions()
        {
            var opt = new OptionsModel();
            opt.OtherParams = "--debugfiles true --nunitnetversion net40 --loglevel INFO";
            var parser = opt.ParsedParams;
            parser.DebugFiles.ShouldEqual(true);
            parser.LogLevel.ShouldEqual("INFO");
            parser.NUnitNetVersion.ShouldEqual("net40");
        }
    }
}