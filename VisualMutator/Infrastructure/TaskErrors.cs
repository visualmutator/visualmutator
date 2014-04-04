namespace VisualMutator.Infrastructure
{
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;

    public static class TaskErrors
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool Check(Task task)
        {
            if(task.Exception != null)
            {
                _log.Error(task.Exception);
                return true;
            }
            return false;
        }
        public static Task LogErrors(this Task task)
        {
            return task.ContinueWith(r =>
            {
                if (r.Exception != null)
                {
                    _log.Error(r.Exception);
                }
            });
        }
        public static Task<T> LogErrors<T>(this Task<T> task)
        {
            return task.ContinueWith(r =>
            {
                if (r.Exception != null)
                {
                    _log.Error(r.Exception);
                }
                return r.Result;
            });
        }
    }
}