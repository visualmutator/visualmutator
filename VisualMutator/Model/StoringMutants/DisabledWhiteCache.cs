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
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.Ast;
    using Mutations.Types;

    public class DisabledWhiteCache : IWhiteCache
    {
        private readonly IProjectClonesManager _fileManager;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ProjectFilesClone _assemblies;

        public DisabledWhiteCache(IProjectClonesManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void Initialize()
        {
            _assemblies = _fileManager.CreateClone("WhiteCache-");
        }


        public CciModuleSource GetWhiteModules()
        {
            return new CciModuleSource(_assemblies);

        }

   
        public async Task<CciModuleSource> GetWhiteModulesAsync()
        {
            try
            {
                return await Task.Run(() => GetWhiteModules());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            _assemblies.Dispose();
        }

        public void Pause(bool paused)
        {
            
        }

        public CciModuleSource CreateSource(IList<string> assembliesPaths)
        {
            try
            {
                var moduleSource = new CciModuleSource();
                foreach (var assembliesPath in assembliesPaths)
                {
                    moduleSource.AppendFromFile(assembliesPath);
                }
                return moduleSource;
            }
            catch(Exception e)
            {
                _log.Error(e);
                throw;

            }
        }
    }
}