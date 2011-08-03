namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;
   

    public class SessionsManager
    {
        


        public SessionsManager()
        {
            //_sessions = new List<MutationSession>();
        }

        public MutationSession CreateSession(IEnumerable<MutationOperator> operators, IEnumerable<TypeDefinition> types)
        {
            return new MutationSession(operators, types);
        }

        public void SaveSession(MutationSession session)
        {
            

        }
    }
}