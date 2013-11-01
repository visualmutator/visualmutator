namespace VisualMutator.Infrastructure.NinjectModules
{
    #region

    using System.Reflection;
    using log4net;
    using Ninject.Activation;
    using Ninject.Activation.Strategies;

    #endregion

    public class MyMonitorActivationStrategy : ActivationStrategy
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void Activate(IContext context, InstanceReference reference)
        {
            _log.Debug("Ninject Activate: " + reference.Instance.GetType());
            base.Activate(context, reference);
        }

        public override void Deactivate(IContext context, InstanceReference reference)
        {
            _log.Debug("Ninject DeActivate: " + reference.Instance.GetType());
            base.Deactivate(context, reference);
        }
    }
}