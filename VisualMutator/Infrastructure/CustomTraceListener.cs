namespace VisualMutator.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using log4net;
    using UsefulTools.Switches;
    using Switch = UsefulTools.Switches.Switch;

    public class CustomTraceListener : DefaultTraceListener
    {

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format,
            params object[] args)
        {
            base.TraceEvent(eventCache, source, eventType, id, format, args);
            Action a = () => _log.Error(format);
            if(args.Length > 0)
            {
                Exception e = args[0] as Exception;
                if (e != null)
                {
                    a = () => _log.Error(format, e);
                }
            }
           
            Switch.On(eventType)
                .Case(TraceEventType.Error, a)
                .Case(TraceEventType.Warning, ()=>_log.Warn(format))
                .Case(TraceEventType.Information, ()=>_log.Info(format))
                .Default(() => { });

        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {

            base.TraceEvent(eventCache, source, eventType, id, message);
           
            Switch.On(eventType)
                .Case(TraceEventType.Error, () => _log.Error(message))
                .Case(TraceEventType.Warning, () => _log.Warn(message))
                .Case(TraceEventType.Information, () => _log.Info(message))
                .Default(() =>
                {
                });
        }

      
    }
}