namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Cci;

    [Serializable]
    public class UserMutationTarget
    {
 
        private readonly string _passInfo;
        private readonly IDictionary<string, object> _storedObjects;

        public UserMutationTarget(string passInfo, IDictionary<string, object> storedObjects)
        {
            _passInfo = passInfo;
            _storedObjects = storedObjects;
        }

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
        public IDictionary<string, object> StoredObjects
        {
            get { return _storedObjects; }
        }


       
    }
}