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

        protected IDictionary<string, IMutationElement> _mutationElements;

        protected internal MutationElementsContainer(MutationTarget mutationTarget)
        {
            _mutationTarget = mutationTarget;
            _mutationElements = new Dictionary<string, IMutationElement>();
        }

        public IMutationElement this[string key]
        {
            get
            {
                return _mutationElements[key];
            }
            set
            {
                _mutationElements[key] = value;
            }
        }

        public MutationElementType Type(string key)
        {
            return (MutationElementType)this[key];
        }
        public MutationElementMethod Method(string key)
        {
            return (MutationElementMethod)this[key];
        }
        public MethodAndInstructionMutationElement MethodAndInstruction(string key)
        {
            var methodElement = (MutationElementMethod)this[key];

            return new MethodAndInstructionMutationElement(methodElement);
        }
        public MutationElementProperty Property(string key)
        {
            return (MutationElementProperty)this[key];
        }
        public MutationElementField Field(string key)
        {
            return (MutationElementField)this[key];
        }
        public MutationTarget AddElement(string key, IMutationElement value)
        {
            _mutationElements.Add(key, value);
            return _mutationTarget;
        }
        public MutationTarget Add(string key, MethodDefinition method, int instructionIndex)
        {
            _mutationElements.Add(key, new MutationElementMethod(method, instructionIndex));
            return _mutationTarget;
        }
        public MutationTarget Add(string key, MethodDefinition method, Instruction instruction)
        {
            int index = method.Body.Instructions.IndexOf(instruction);
            _mutationElements.Add(key, new MutationElementMethod(method, index));
            return _mutationTarget;
        }
        public MutationTarget Add<TMemberDefinition>(string key, TMemberDefinition param) where TMemberDefinition : IMemberDefinition
        {

            IMutationElement element = Switch.Into<IMutationElement>().FromTypeOf(param)
                .Case<MethodDefinition>(method => new MutationElementMethod(method))
                .Case<TypeDefinition>(type => new MutationElementType(type))
                .Case<PropertyDefinition>(prop => new MutationElementProperty(prop))
                .Case<FieldDefinition>(field => new MutationElementField(field))
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

        public MutationElementsContainer Hidden
        {
            get
            {
                return _hidden;
            }
        }

        public virtual IList<IMutationElement> RetrieveElements()
        {
            return _mutationElements.Values.ToList();
        }


    }
}