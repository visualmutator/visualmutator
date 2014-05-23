namespace VisualMutator.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using log4net;
    using Microsoft.Cci;
    using Mutations.Types;

    public class CoveringTestsFinder
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<MethodIdentifier> FindCoveringTests(IModule module, ICodePartsMatcher targetsMatcher)
        {
            _log.Debug("Scanning " + module.Name.Value + " for selected covering tests. ");
            var visitor = new CoveringTestsVisitor(targetsMatcher);

            var traverser = new CodeTraverser
            {
                PreorderVisitor = visitor
            };

            traverser.Traverse(module);
            _log.Debug("Finished scanning module"+module.Name.Value+". Found " + visitor.FoundTests.Count+
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
}