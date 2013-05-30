namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Microsoft.Cci;


    public class UserMutationTarget
    {
 
        private readonly string _passInfo;
        private readonly IDictionary<string, object> _operatorObjects;

        /// <summary>
        /// Information about current mutation pass
        /// </summary>
        public string PassInfo
        {
            get
            {
                return _passInfo;
            }
        }

        /// <summary>
        /// Object from AST needed by operator in rewriting phase
        /// </summary>
        public IDictionary<string, object> OperatorObjects
        {
            get { return _operatorObjects; }
        }


        public UserMutationTarget(string passInfo, IDictionary<string, object> operatorObjects)
        {
            _passInfo = passInfo;
            _operatorObjects = operatorObjects;
        }
    }
}