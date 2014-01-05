namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Extensibility;
    using Microsoft.Cci;
    using UsefulTools.ExtensionMethods;

    public class AstProcessor
    {
        protected readonly IDictionary<object, AstDescriptor> AllAstIndices;
        protected readonly IDictionary<AstDescriptor, object> AllAstObjects;
        protected readonly IList<AstNode> AllNodes;
        private AstNode _currentNode;
        private AstNode _currentMethod;
        private AstDescriptor _currentType;
        private IModule _traversedModule;
        protected int TreeObjectsCounter { get; set; }

        public AstProcessor(IModule module)
        {
            AllAstIndices = new Dictionary<object, AstDescriptor>();
            AllAstObjects = new Dictionary<AstDescriptor, object>();
            AllNodes = new List<AstNode>();
            _traversedModule = module;
        }
        public bool IsCurrentlyProcessed(object obj)
        {
            return obj == _currentNode.Object;
        }
        public void Process<T>(T obj)
        {
            TreeObjectsCounter++;
            _currentNode = new AstNode(CreateProcessingContext<T>(),obj);//new AstNode(new AstDescriptor(TreeObjectsCounter), obj);
            if (!AllAstIndices.ContainsKey(obj))
            {
                AllAstIndices.Add(obj, GetDescriptorForCurrent());
                AllAstObjects.Add(GetDescriptorForCurrent(), obj);
                AllNodes.Add(_currentNode);
            }
        }
        public void MethodEnter(IMethodDefinition method)
        {
            _currentMethod = new AstNode(_currentNode.Context, method);
        }

        public void MethodExit(IMethodDefinition method)
        {
            _currentMethod = null;
        }

        public AstDescriptor GetDescriptorForCurrent()
        {
            return _currentNode.Context.Descriptor;
        }

        public void PostProcess(MutationTarget mutationTarget)
        {
            //just unspecialize all stored objects
            mutationTarget.Variant.AstObjects = mutationTarget.Variant.AstObjects
                .ToDictionary(pair => pair.Key, pair => Unspecialize(pair.Value));

            //translate objects to their indices that identify them
            mutationTarget.Variant.ObjectsIndices = mutationTarget.Variant
                .AstObjects.MapValues((key, val) => AllAstIndices[val]);
            //REMOVE: mutationTarget.MethodIndex = AllAstIndices[mutationTarget.MethodRaw];
        }

        public AstNode PostProcessBack(MutationTarget mutationTarget)
        {
            //Are we processing an object corresponding to any mutation target?
        //    var target = _mutationTargets.SingleOrDefault(t => t.CounterValue == TreeObjectsCounter);

            var node = new AstNode(mutationTarget.ProcessingContext, 
                AllAstObjects[mutationTarget.ProcessingContext.Descriptor]);
            //    _log.Debug("Creating pair: " + TreeObjectsCounter + " " + Formatter.Format(obj) + " <===> " + target);
         /*   if (target != null)
            {
                _targetAstObjects.Add(Tuple.Create(obj, target));
            }

            if (_sharedTargets.Any(t => t.CounterValue == TreeObjectsCounter))
            {
                _sharedAstObjects.Add(obj);
            }
              */
            if (mutationTarget.ProcessingContext.ModuleName == _traversedModule.Name.Value)
            {
                //TODO: do better. now they can be null for changeless mutant
                if (mutationTarget.Variant.ObjectsIndices != null && AllAstObjects != null)
                {
                    mutationTarget.Variant.AstObjects = mutationTarget.Variant.ObjectsIndices
                    .MapValues((key, val) => AllAstObjects[val]);
                    
                    mutationTarget.MethodMutated = (IMethodDefinition)AllAstObjects[new AstDescriptor(mutationTarget.MethodIndex)];
                }
            }
              return node;
        }

        private object Unspecialize(object value)
        {
            var methodDefinition = value as IMethodDefinition;
            if (methodDefinition != null)
            {
                return MemberHelper.UninstantiateAndUnspecialize(methodDefinition);
            }
            var fieldDefinition = value as IFieldDefinition;
            if (fieldDefinition != null)
            {
                return MemberHelper.Unspecialize(fieldDefinition);
            }
            var eventDefinition = value as IEventDefinition;
            if (eventDefinition != null)
            {
                return MemberHelper.Unspecialize(eventDefinition);
            }
            var fieldReference = value as IFieldReference;
            if (fieldReference != null)
            {
                return MemberHelper.Unspecialize(fieldReference);
            }
            return value;
        }

        public ProcessingContext CreateProcessingContext<T>()
        {
            return new ProcessingContext()
                   {
                       Descriptor = GetDescriptorForCurrent(),
                       Method = _currentMethod,
                       Type = _currentType,
                       CallTypeName = typeof(T).Name,
                       ModuleName = _traversedModule.Name.Value,
                    
                   };
        }
    }
}