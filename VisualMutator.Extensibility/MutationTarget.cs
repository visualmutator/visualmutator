namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using CommonUtilityInfrastructure;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using ICustomAttributeProvider = Mono.Cecil.ICustomAttributeProvider;

    public class MutationElementsContainer
    {
        protected MutationTarget _mutationTarget;

        protected readonly IDictionary<string, IMutationElement> _mutationElements;

        protected internal MutationElementsContainer(MutationTarget mutationTarget)
        {
            _mutationTarget = mutationTarget;
            _mutationElements = new Dictionary<string, IMutationElement>();
        }

   

  
        private void ThrowIfKeyExists(string key)
        {
            if (_mutationTarget._mutationElements.ContainsKey(key)
                || _mutationTarget.Hidden._mutationElements.ContainsKey(key))
            {
                throw new ArgumentException("Entry with this key already exists: " + key);
            }
        }
        public virtual IMutationElement this[string key]
        {
            get
            {
                return _mutationElements[key];
            }
        }
        public MutationTarget Add(string key, MethodDefinition method, Instruction instruction)
        {
            ThrowIfKeyExists(key);
            int index = method.Body.Instructions.IndexOf(instruction);
            _mutationElements.Add(key, new MutationElementMethodAndInstruction(method, index));
            return _mutationTarget;
        }
        public MutationTarget Add<TMemberDefinition>(string key, TMemberDefinition param) where TMemberDefinition : IMemberDefinition
        {
            ThrowIfKeyExists(key);
            IMutationElement element = Switch.Into<IMutationElement>().FromTypeOf(param)
                .Case<MethodDefinition>(method => new MutationElementMethod(method))
                .Case<TypeDefinition>(type => new MutationElementType(type))
                .Case<PropertyDefinition>(prop => new MutationElementProperty(prop))
                .Case<FieldDefinition>(field => new MutationElementField(field))
                .Case<EventDefinition>(field => new MutationElementEvent(field))
                .GetResult();

            _mutationElements.Add(key, element);
            return _mutationTarget;
        }
    }

    public class MutationTarget : MutationElementsContainer
    {

        private MutationElementsContainer _hidden;

        public MutationTarget()
            : base(null)
        {
            _mutationTarget = this;
   
            _hidden = new MutationElementsContainer(this);
     
        }
        public override IMutationElement this[string key]
        {
            get
            {
                IMutationElement val;
                return _mutationElements.TryGetValue(key, out val) ? val : _hidden[key];
            }
        }
        public MutationElementsContainer Hidden
        {
            get
            {
                return _hidden;
            }
        }

        public virtual IList<IMutationElement> RetrieveNonHidden()
        {
            return _mutationElements.Values.ToList();
        }


    }
}