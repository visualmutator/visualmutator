using System.Threading.Tasks;

namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.Ast;
    using Mutations.Types;

    public class DisabledWhiteCache : IWhiteCache
    {
        private IList<string> _assembliesPaths;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DisabledWhiteCache()
        {
        }

        public void Initialize(IList<string> assembliesPaths)
        {
            _assembliesPaths = assembliesPaths;
           
        }

        public CciModuleSource GetWhiteModules()
        {
            return CreateSource(_assembliesPaths);

        }

        public void Reinitialize(List<string> assembliesPaths)
        {
            
        }

        public CciModuleSource CreateSource(IList<string> assembliesPaths)
        {
            var moduleSource = new CciModuleSource();
            foreach (var assembliesPath in assembliesPaths)
            {
                try
                {
                    moduleSource.AppendFromFile(assembliesPath);

                }
                catch (AssemblyReadException e)
                {
                    _log.Warn("ReadAssembly failed. ", e);
                }
                catch (Exception e)
                {
                    _log.Warn("ReadAssembly failed. ", e);
                }

            }
            return moduleSource;
        }
    }
}