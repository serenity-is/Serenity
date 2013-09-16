using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public abstract partial class PropertyDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        protected PropertyGrid propertyGrid;

        private void InitPropertyGrid()
        {
            var pgDiv = this.ById("PropertyGrid");
            if (pgDiv.Length <= 0)
                return;

            var pgOptions = GetPropertyGridOptions();

            propertyGrid = new PropertyGrid(pgDiv, pgOptions);
        }

        protected virtual string GetFormKey()
        {
            return this.entityType.Value;
        }

        protected virtual List<PropertyItem> GetPropertyItems()
        {
            var formKey = GetFormKey();
            return Q.GetForm(formKey);
        }

        protected virtual PropertyGridOptions GetPropertyGridOptions()
        {
            return new PropertyGridOptions
            {
                IdPrefix = this.idPrefix,
                Items = GetPropertyItems(),
                Mode = PropertyGridMode.Insert,
                UseCategories = false
            };
        }
    }
}