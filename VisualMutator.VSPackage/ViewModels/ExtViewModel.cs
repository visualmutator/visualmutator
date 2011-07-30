namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System;
    using System.Linq.Expressions;
    using System.Waf.Applications;

    public abstract class ExtViewModel<TView> : ViewModel<TView>
        where TView : IView
    {
        protected ExtViewModel(TView view)
            : base(view)
        {
        }

        protected ExtViewModel(TView view, bool isChild)
            : base(view, isChild)
        {
        }


        protected void RaisePropertyChangedExt<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            this.RaisePropertyChanged(memberExpression.Member.Name);
        }

    }
}