using System;

namespace Serenity
{
    public abstract partial class PropertyDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private Lazy<string> entityType;

        protected void InitInferences()
        {
            var self = this;
            entityType = new Lazy<string>(() => self.InferEntityType());
        }

        protected virtual string InferEntityType()
        {
            var typeAttributes = this.GetType().GetCustomAttributes(typeof(EntityTypeAttribute), true);
            if (typeAttributes.Length == 1)
                return typeAttributes[0].As<EntityTypeAttribute>().EntityType;

            // typeof(TEntity).Name'i kullanamayız, TEntity genelde Serializable ve Imported olduğundan dolayı tipi Object e karşılık geliyor!

            // remove global namespace
            var name = this.GetType().FullName;
            var px = name.IndexOf(".");
            if (px >= 0)
                name = name.Substring(px + 1);

            if (name.EndsWith("Dialog"))
                name = name.Substr(0, name.Length - 6);

            return name;
        }
    }
}