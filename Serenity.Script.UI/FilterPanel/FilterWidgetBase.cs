using jQueryApi;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public abstract class FilterWidgetBase<TOptions> : TemplatedWidget<TOptions>
        where TOptions: class, new()
    {
        private FilterStore store;

        public FilterWidgetBase(jQueryObject div, TOptions opt)
            : base(div, opt)
        {
            this.store = new FilterStore(new List<PropertyItem>());
            this.store.Changed += OnFilterStoreChanged;
        }

        public override void Destroy()
        {
            if (store != null)
            {
                store.Changed -= OnFilterStoreChanged;
                store = null;
            }

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

                    store = value ?? new FilterStore(new PropertyItem[0]);
                    store.Changed += OnFilterStoreChanged;
                    FilterStoreChanged();
                }
            }
        }
    }
}