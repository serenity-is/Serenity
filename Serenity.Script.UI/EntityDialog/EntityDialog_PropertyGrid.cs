using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        protected PropertyGrid propertyGrid;

        [Obsolete("Prefer async version")]
        private void InitPropertyGrid()
        {
            var pgDiv = this.ById("PropertyGrid");
            if (pgDiv.Length <= 0)
                return;

            #pragma warning disable 618
            var pgOptions = GetPropertyGridOptions();
            #pragma warning restore 618

            propertyGrid = new PropertyGrid(pgDiv, pgOptions);
        }

        private void InitPropertyGrid(Action complete, Action<object> fail)
        {
            fail.TryCatch(delegate()
            {
                var pgDiv = this.ById("PropertyGrid");
                if (pgDiv.Length <= 0)
                {
                    complete();
                    return;
                }

                GetPropertyGridOptions(pgOptions =>
                {
                    fail.TryCatch(delegate()
                    {
                        propertyGrid = new PropertyGrid(pgDiv, pgOptions);
                        propertyGrid.Init(pg =>
                        {
                            complete();
                        }, fail);
                    })();
                }, fail);
            })();
        }

        [Obsolete("Prefer async version")]
        protected virtual List<PropertyItem> GetPropertyItems()
        {
            var formKey = GetFormKey();
            #pragma warning disable 618
            return Q.GetForm(formKey);
            #pragma warning restore 618
        }

        [Obsolete("Prefer async version")]
        protected virtual PropertyGridOptions GetPropertyGridOptions()
        {
            #pragma warning disable 618
            return new PropertyGridOptions
            {
                IdPrefix = this.idPrefix,
                Items = GetPropertyItems(),
                Mode = PropertyGridMode.Insert,
                LocalTextPrefix = "Forms." + GetFormKey() + "."
            };
            #pragma warning restore 618
        }

        protected virtual void GetPropertyGridOptions(Action<PropertyGridOptions> complete, Action<object> fail)
        {
            GetPropertyItems(propertyItems =>
            {
                fail.TryCatch(delegate() 
                {
                    complete(new PropertyGridOptions
                    {
                        IdPrefix = this.idPrefix,
                        Items = propertyItems,
                        Mode = PropertyGridMode.Insert
                    });
                })();
            }, fail);
        }

        protected virtual void GetPropertyItems(Action<List<PropertyItem>> complete, Action<object> fail)
        {
            fail.TryCatch(delegate()
            {
                var formKey = GetFormKey();
                Q.GetForm(formKey, complete, fail);
            })();
        }
    }
}