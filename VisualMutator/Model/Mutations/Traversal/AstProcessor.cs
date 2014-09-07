namespace VisualMutator.Model.Mutations.Traversal
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using UsefulTools.ExtensionMethods;

    public class AstProcessor
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        protected readonly IDictionary<object, AstDescriptor> AllAstIndices;
        public IDictionary<AstDescriptor, object> AllAstObjects;
        protected readonly IList<AstNode> AllNodes;
        private AstNode _currentNode;
        private AstNode _currentMethodNode;
        private AstNode _currentTypeNode;
        private readonly IModule _traversedModule;
        private AstDescriptor _currentDescriptor;
        private ISourceMethodBody _currentBody;

        public AstProcessor(IModule module)
        {
            AllAstIndices = new Dictionary<object, AstDescriptor>();
            AllAstObjects = new Dictionary<AstDescriptor, object>();
            AllNodes = new List<AstNode>();
            _traversedModule = module;
            _currentDescriptor = new AstDescriptor(0);
        }
        public bool IsCurrentlyProcessed(object obj)
        {
            return obj == _currentNode.Object;
        }
        public void Process<T>(T obj)
        {
            
            _currentNode = new AstNode(CreateProcessingContext<T>(),obj);//new AstNode(new AstDescriptor(TreeObjectsCounter), obj);
            if (!AllAstIndices.ContainsKey(obj))
            {
                AllAstIndices.Add(obj, GetDescriptorForCurrent());
               
            }
            AllAstObjects.Add(GetDescriptorForCurrent(), obj);
            AllNodes.Add(_currentNode);
            _currentDescriptor = _currentDescriptor.Increment();

        }

        public void TypeEnter(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            
            _currentDescriptor = _currentDescriptor.GoDown();
         //   _log.Debug("Going down on "+new TypeIdentifier(namespaceTypeDefinition)+" - "+_currentDescriptor);
            _currentTypeNode = new AstNode(_currentNode.Context, namespaceTypeDefinition);
            
        }

        public void TypeExit(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            _currentTypeNode = null;
            _currentDescriptor = _currentDescriptor.GoUp();
        }
        public void MethodEnter(IMethodDefinition method)
        {
            _currentDescriptor = _currentDescriptor.GoDown();
         //   _log.Debug("Going down on " + new MethodIdentifier(method) + " - " + _currentDescriptor);
            _currentMethodNode = new AstNode(_currentNode.Context, method);
          
        }

        public void MethodExit(IMethodDefinition method)
        {
            _currentMethodNode = null;
              _currentDescriptor = _currentDescriptor.GoUp();
        }
        public void MethodBodyEnter(ISourceMethodBody method)
        {
            _currentBody = method;
            _currentDescriptor = _currentDescriptor.GoDown();
        }
        public AstDescriptor MethodBodyExit(ISourceMethodBody method)
        {
            _currentBody = null;
            _currentDescriptor = _currentDescriptor.GoUp();
            return _currentDescriptor;
        }
        public AstDescriptor GetDescriptorForCurrent()
        {
            return _currentDescriptor; //_currentNode.Context.Descriptor;
        }

        public void PostProcess(MutationTarget mutationTarget)
        {
            //just unspecialize all stored objects
            mutationTarget.Variant.AstObjects = mutationTarget.Variant.AstObjects
                .ToDictionary(pair => pair.Key, pair => Unspecialize(pair.Value));

            //translate objects to their indices that identify them
            mutationTarget.Variant.ObjectsIndices = mutationTarget.Variant
                .AstObjects.MapValues((key, val) => AllAstIndices[val]);

            mutationTarget.Variant.AstObjects = null; //TODO: refactor

            if (mutationTarget.ProcessingContext != null &&
                mutationTarget.ProcessingContext.ModuleName == _traversedModule.Name.Value)
            {
                var type = (INamespaceTypeDefinition) AllAstObjects[
                    mutationTarget.ProcessingContext.Type.Context.Descriptor];
                mutationTarget.NamespaceName = type.ContainingUnitNamespace.Name.Value;
                mutationTarget.TypeName = type.Name.Value;
            }
        }

        public AstNode PostProcessBack(MutationTarget mutationTarget)
        {
            if( mutationTarget.ProcessingContext.ModuleName != _traversedModule.Name.Value)
            {
                return null;
            }
            //Are we processing an object corresponding to any mutation target?
        //    var target = _mutationTargets.SingleOrDefault(t => t.CounterValue == TreeObjectsCounter);
            AstNode node;
            if(mutationTarget.ProcessingContext != null)
            {if(!AllAstObjects.ContainsKey(mutationTarget.ProcessingContext.Descriptor))
                {
                    _log.Error("No object: "+ mutationTarget.ProcessingContext.Descriptor);
                    _log.Error("All objects: "+ AllAstObjects.Keys.Select(_=>_.ToString()).Aggregate((a,b) => a + "\n"+b));
                    Debugger.Break();
                }
                node = new AstNode(mutationTarget.ProcessingContext,
                    AllAstObjects[mutationTarget.ProcessingContext.Descriptor]);
            }
            else
            {
                node = new AstNode(new ProcessingContext(), new AstDescriptor());
            }
            
            if (mutationTarget.ProcessingContext != null && mutationTarget.ProcessingContext.ModuleName == _traversedModule.Name.Value)
            {
                //TODO: do better. now they can be null for changeless mutant
                if (mutationTarget.Variant.ObjectsIndices != null && AllAstObjects != null)
                {
                    mutationTarget.Variant.AstObjects = mutationTarget.Variant.ObjectsIndices
                        .MapValues((key, val) => AllAstObjects[val]);

                    mutationTarget.MethodMutated = (IMethodDefinition)AllAstObjects[
                        mutationTarget.ProcessingContext.Method.Context.Descriptor];
                }
            }
            return node;
        }

        public string ModuleName
        {
            get
            {
                return _traversedModule.Name.Value;
            }
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
                       Descriptor = _currentDescriptor,
                       Method = _currentMethodNode,
                       Type = _currentTypeNode,
                       CallTypeName = typeof(T).Name,
                       ModuleName = _traversedModule.Name.Value,
                   };
        }


    }
}