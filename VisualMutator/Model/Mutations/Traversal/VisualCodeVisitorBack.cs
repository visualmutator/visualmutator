namespace VisualMutator.Model.Mutations.Traversal
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.UtilityDataStructures;

    #endregion

    public class VisualCodeVisitorBack : VisualCodeVisitor
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICollection<MutationTarget> _mutationTargets;//TODO: remove, its ambigius with base class field of the same name
        private readonly ICollection<MutationTarget> _sharedTargets;
        private readonly Hashtable<object, object> _copyFor;
        //List of objects found in this model tree corresponding to mutation targets
        private List<AstNode> _targetAstObjects;
        private List<AstNode> _sharedAstObjects;

        public List<AstNode> SharedAstObjects
        {
            get { return _sharedAstObjects; }
        }

        public List<AstNode> TargetAstObjects
        {
            get { return _targetAstObjects; }
        }

        public VisualCodeVisitorBack(ICollection<MutationTarget> mutationTargets, 
            ICollection<MutationTarget> sharedTargets, IModule module, string id, Hashtable<object, object> copyFor)
            : base(id, new OperatorCodeVisitor(), module)
        {
            _mutationTargets = mutationTargets;
            _sharedTargets = sharedTargets;
            _copyFor = copyFor;
            _targetAstObjects = new List<AstNode>();
            _sharedAstObjects = new List<AstNode>();
        }


        protected override bool Process(object obj)
        {
            base.Process(obj);
         //   _log.Debug("Process back: " + TreeObjectsCounter + " " + Formatter.Format(obj) + " : " + obj.GetHashCode());
            return false;
        }


        public override void PostProcess()
        {
          //  Processor.AllAstObjects = Processor.AllAstObjects.MapValues((key,obj) => _copyFor[obj])
             //   .Where(pair => pair.Value != null).ToDictionary(pair => pair.Key, pair => pair.Value);
            _targetAstObjects = _mutationTargets
                .Where(t => t.ProcessingContext != null && t.ProcessingContext.ModuleName == Processor.ModuleName)
                .Select(Processor.PostProcessBack).ToList();
            _sharedAstObjects = _sharedTargets
                .Where(t => t.ProcessingContext != null && t.ProcessingContext.ModuleName == Processor.ModuleName)
                .Select(Processor.PostProcessBack).ToList();

        }
    }
}