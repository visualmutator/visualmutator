namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

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

        public TMutationElement Of<TMutationElement>(string key) where TMutationElement : IMutationElement
        {
            return (TMutationElement)this[key];
        }

        public MutationElementType Type(string key)
        {
            return (MutationElementType)this[key];
        }
        public MutationElementMethod Method(string key)
        {
            return (MutationElementMethod)this[key];
        }
        public MutationElementProperty Property(string key)
        {
            return (MutationElementProperty)this[key];
        }
        public MutationElementField Field(string key)
        {
            return (MutationElementField)this[key];
        }
        public MutationTarget Add(string key, IMutationElement value)
        {
            _mutationElements.Add(key, value);
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