using jQueryApi;
using System;

namespace Serenity
{
    public abstract partial class PropertyDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions : class, new()
    {
        protected virtual bool ValidateBeforeSave()
        {
            return true;
        }

        protected virtual TEntity GetSaveEntity()
        {
            var entity = new TEntity();

            if (this.propertyGrid != null)
                this.propertyGrid.Save(entity);

            return entity;
        }
    }
}