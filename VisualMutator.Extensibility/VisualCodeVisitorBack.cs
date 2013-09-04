namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CommonUtilityInfrastructure;
    using log4net;

    public class VisualCodeVisitorBack : VisualCodeVisitor
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICollection<MutationTarget> _mutationTargets;
        private readonly List<MutationTarget> _sharedTargets;
        private readonly List<Tuple<object, MutationTarget>> _targetAstObjects;
        private readonly List<object> _sharedAstObjects;

        public List<object> SharedAstObjects
        {
            get { return _sharedAstObjects; }
        }

        public List<Tuple<object, MutationTarget>> TargetAstObjects
        {
            get { return _targetAstObjects; }
        }

        public VisualCodeVisitorBack(ICollection<MutationTarget> mutationTargets, List<MutationTarget> sharedTargets)
            : base(new OperatorCodeVisitor())
        {
            _mutationTargets = mutationTargets;
            _sharedTargets = sharedTargets;
            _targetAstObjects = new List<Tuple<object, MutationTarget>>();
            _sharedAstObjects = new List<object>();
        }


        protected override bool Process(object obj)
        {
            base.Process(obj);
            _log.Warn("Process back: " + TreeObjectsCounter + " " + Formatter.Format(obj) + " : " + obj.GetHashCode());
            var target = _mutationTargets.FirstOrDefault(t => t.CounterValue == TreeObjectsCounter);
            if (target != null)
            {
                _log.Warn("Creating pair: " + TreeObjectsCounter +" "+ Formatter.Format(obj) + " <===> " + target);
                _targetAstObjects.Add(Tuple.Create(obj, target));
            }
            if (_sharedTargets.Any(t => t.CounterValue == TreeObjectsCounter))
            {
                _sharedAstObjects.Add(obj);
            }
            return false;
        }


        public override void PostProcess()
        {
            foreach (var mutationTarget in MutationTargets.Select(t => t.Item2).Flatten())
            {
                mutationTarget.Variant.AstObjects = mutationTarget.VariantObjectsIndices
                    .MapValues((key, val) => AllAstObjects[val]);
                
            }
        }
    }
}