namespace VisualMutator.Domain
{
    using System;
    using System.Linq.Expressions;
    using System.Waf.Foundation;

    public class ExtModel : Model
    {
        protected void RaisePropertyChangedExt<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            this.RaisePropertyChanged(memberExpression.Member.Name);
        }
    }
}