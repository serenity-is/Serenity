using jQueryApi;
using System;

namespace Serenity
{
    public abstract class FilterWidgetBase<TOptions> : TemplatedWidget<TOptions>
        where TOptions: class, new()
    {
        private static IFilterableSource emptySource;

        private IFilterableSource source;
        private FilterStore store;

        public FilterWidgetBase(jQueryObject div, TOptions opt)
            : base(div, opt)
        {
            emptySource = emptySource ?? new EmptyFilterableSource();
            
            this.source = emptySource;
            this.store = new FilterStore(new EmptyFilterableSource());
            this.store.Changed += OnFilterStoreChanged;
        }

        public override void Destroy()
        {
            if (store != null)
            {
                store.Changed -= OnFilterStoreChanged;
                store = null;
            }

            source = null;

            base.Destroy();
        }

        private void OnFilterStoreChanged(object sender, EventArgs e)
        {
            FilterStoreChanged();
        }

        protected virtual void FilterStoreChanged()
        {
        }

        public FilterStore Store
        {
            get { return store; }
            set
            {
                if (store != value)
                {
                    if (this.store != null)
                        this.store.Changed -= OnFilterStoreChanged;

                    store = value ?? new FilterStore(new EmptyFilterableSource());
                    store.Changed += OnFilterStoreChanged;
                    FilterStoreChanged();
                }
            }
        }
    }
}