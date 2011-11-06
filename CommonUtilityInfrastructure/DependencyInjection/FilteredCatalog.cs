namespace CommonUtilityInfrastructure.DependencyInjection
{
    using System;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Linq.Expressions;

    public class FilteredCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly ComposablePartCatalog _inner;
        private readonly INotifyComposablePartCatalogChanged _innerNotifyChange;
        private readonly IQueryable<ComposablePartDefinition> _partsQuery;

        public FilteredCatalog(ComposablePartCatalog inner,
                               Expression<Func<ComposablePartDefinition, bool>> expression)
        {
            _inner = inner;
            _innerNotifyChange = inner as INotifyComposablePartCatalogChanged;
            _partsQuery = inner.Parts.Where(expression);
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return _partsQuery;
            }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add
            {
                if (_innerNotifyChange != null)
                    _innerNotifyChange.Changed += value;
            }
            remove
            {
                if (_innerNotifyChange != null)
                    _innerNotifyChange.Changed -= value;
            }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add
            {
                if (_innerNotifyChange != null)
                    _innerNotifyChange.Changing += value;
            }
            remove
            {
                if (_innerNotifyChange != null)
                    _innerNotifyChange.Changing -= value;
            }
        }
    }
}