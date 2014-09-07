namespace VisualMutator.Model.CoverageFinder
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Exceptions;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Mutations.Types;
    using UsefulTools.ExtensionMethods;

    public class CoveringTestsFinder : ICoveringTestsFinder
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Task<List<MethodIdentifier>> FindCoveringTests(List<CciModuleSource> modules, ICodePartsMatcher matcher)
        {
            var tt = modules.Select(module => Task.Run(() => FindCoveringTests(module, matcher)));
            return Task.WhenAll(tt)
                        .ContinueWith(t => t.Result.Flatten().ToList());
        }

        private List<MethodIdentifier> FindCoveringTests(CciModuleSource module, ICodePartsMatcher targetsMatcher)
        {
            _log.Debug("Scanning " + module.Module.Name + " for selected covering tests. ");
            var visitor = new CoveringTestsVisitor(targetsMatcher);

            var traverser = new CodeTraverser
            {
                PreorderVisitor = visitor
            };
            
            traverser.Traverse(module.Decompile(module.Module));
            _log.Debug("Finished scanning module"+ module.Module.Name + ". Found " + visitor.FoundTests.Count+
                ". Scanned total: " + visitor.ScannedMethods + " methods and "+
                visitor.ScannedMethodCalls+" method calls.");

            _log.Debug("Listing found tests: ");
            foreach (var methodIdentifier in visitor.FoundTests)
            {
                _log.Debug("Test: "+ methodIdentifier);
            }

            if (visitor.IsChoiceError)
            {
                throw new TestWasSelectedToMutateException();
            }
            return visitor.FoundTests.ToList();
        } 
    }

    public interface ICoveringTestsFinder
    {
        Task<List<MethodIdentifier>> FindCoveringTests(List<CciModuleSource> modules, ICodePartsMatcher matcher);
    }
}