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

        private async Task InitPropertyGridAsync()
        {
            var pgDiv = this.ById("PropertyGrid");
            if (pgDiv.Length <= 0)
                return;

            var pgOptions = await GetPropertyGridOptionsAsync();

            propertyGrid = new PropertyGrid(pgDiv, pgOptions);
        }

        protected async virtual Task<List<PropertyItem>> GetPropertyItemsAsync()
        {
            var formKey = GetFormKey();
            return await Q.GetFormAsync(formKey);
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

        protected async virtual Task<PropertyGridOptions> GetPropertyGridOptionsAsync()
        {
            return new PropertyGridOptions
            {
                IdPrefix = this.idPrefix,
                Items = await GetPropertyItemsAsync(),
                Mode = PropertyGridMode.Insert
            };
        }
    }
}