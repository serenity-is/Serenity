using System;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private Lazy<string> entityType;
        private Lazy<string> localTextPrefix;
        private Lazy<string> entitySingular;
        private Lazy<string> entityIdField;
        private Lazy<string> entityNameField;
        private Lazy<string> entityIsActiveField;

        protected void InitInferences()
        {
            var self = this;
            entityType = new Lazy<string>(() => self.InferEntityType());
            localTextPrefix = new Lazy<string>(() => self.InferLocalTextPrefix());
            entitySingular = new Lazy<string>(() => self.InferEntitySingular());
            entityIdField = new Lazy<string>(() => self.InferEntityIdField());
            entityNameField = new Lazy<string>(() => self.InferEntityNameField());
            entityIsActiveField = new Lazy<string>(() => self.InferEntityIsActiveField());
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

        protected virtual string InferLocalTextPrefix()
        {
            return "Db." + entityType.Value + ".";
        }

        protected virtual string InferEntitySingular()
        {
            return Q.TryGetText(localTextPrefix.Value + "EntitySingular") ?? entityType.Value;
        }

        protected virtual string InferEntityNameField()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(NamePropertyAttribute), true);
            if (attributes.Length == 1)
                return attributes[0].As<NamePropertyAttribute>().NameProperty;

            return "Name";
        }

        protected virtual string InferEntityIdField()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(IdPropertyAttribute), true);
            if (attributes.Length == 1)
                return attributes[0].As<IdPropertyAttribute>().IdProperty;

            return "ID";
        }

        protected virtual string InferEntityIsActiveField()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(IsActivePropertyAttribute), true);
            if (attributes.Length == 1)
                return attributes[0].As<IsActivePropertyAttribute>().IsActiveProperty;

            return "IsActive";
        }
    }
}