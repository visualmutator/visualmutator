namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    public class MutationSession
    {
        private readonly string _name;

        private readonly IEnumerable<MutationOperator> _operators;

        private readonly IEnumerable<TypeDefinition> _types;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public IEnumerable<TypeDefinition> Types
        {
            get
            {
                return _types;
            }
        }

  
        public MutationSession(string name, IEnumerable<MutationOperator> operators, IEnumerable<TypeDefinition> types)
        {
            _name = name;
            _operators = operators;
            _types = types;
        }

        public void Run()
        {
            foreach (var mutationOperator in _operators)
            {
                mutationOperator.Operator.Mutate(_types);
            }
        }
    }
}