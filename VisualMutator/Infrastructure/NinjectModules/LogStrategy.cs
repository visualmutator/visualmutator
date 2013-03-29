namespace VisualMutator.Infrastructure.NinjectModules
{
    using System.Reflection;
    using Ninject.Activation.Strategies;
    using log4net;

    public class MyMonitorActivationStrategy : ActivationStrategy
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void Activate(Ninject.Activation.IContext context, Ninject.Activation.InstanceReference reference)
        {
            _log.Debug("Ninject Activate: " + reference.Instance.GetType());
            base.Activate(context, reference);
        }

        public override void Deactivate(Ninject.Activation.IContext context, Ninject.Activation.InstanceReference reference)
        {
            _log.Debug("Ninject DeActivate: " + reference.Instance.GetType());
            base.Deactivate(context, reference);
        }
    }
}