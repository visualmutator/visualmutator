namespace VisualMutator.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using log4net;

     using PostSharp.Aspects;
     using PostSharp.Extensibility;


     [Serializable] 
     public sealed class TraceAttribute : OnMethodBoundaryAspect
     {
       

         private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



         public override void OnEntry(MethodExecutionArgs args)
         {
            // Trace.Indent();
            // Trace.
            // Debug.Assert(args.Method.DeclaringType != null, "args.Method.DeclaringType != null");
             _log.Debug(string.Format("Entering {0}.{1}.{2}",
                 args.Method.DeclaringType.Name, args.Method.Name, args.ReturnValue));
         }

         public override void OnExit(MethodExecutionArgs args)
         {

             _log.Debug(string.Format("Leaving {0}.{1}.{2}",
                 args.Method.DeclaringType.Name, args.Method.Name, args.ReturnValue));
         } 

     } 
}