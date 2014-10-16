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
            this.store = new FilterStore();
        }

        public override void Destroy()
        {
            if (store != null)
            {
                store.Changed -= FilterStoreChanged;
                store = null;
            }

            source = null;

            base.Destroy();
        }

        protected virtual void FilterStoreChanged(object sender, EventArgs e)
        {

        }

        protected virtual void FilterSourceChanged()
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
                        this.store.Changed -= FilterStoreChanged;

                    store = value ?? new FilterStore();
                    store.Changed += FilterStoreChanged;
                }
            }
        }

        public IFilterableSource Source
        {
            get 
            { 
                return source; 
            }
            set
            {
                if (source != value)
                {
                    source = value ?? emptySource;
                    FilterSourceChanged();
                }
            }
        }
    }
}