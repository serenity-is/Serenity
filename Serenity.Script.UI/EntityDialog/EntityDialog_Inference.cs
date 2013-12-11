using System;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private string entityType;
        private string localTextPrefix;
        private string entitySingular;
        private string entityIdField;
        private string entityNameField;
        private string entityIsActiveField;

        protected virtual string GetEntityType()
        {
            if (entityType == null)
            {
                var typeAttributes = this.GetType().GetCustomAttributes(typeof(EntityTypeAttribute), true);
                if (typeAttributes.Length == 1)
                {
                    entityType = typeAttributes[0].As<EntityTypeAttribute>().EntityType;
                    return null;
                }

                // typeof(TEntity).Name'i kullanamayız, TEntity genelde Serializable ve Imported olduğundan dolayı tipi Object e karşılık geliyor!

                // remove global namespace
                var name = this.GetType().FullName;
                var px = name.IndexOf(".");
                if (px >= 0)
                    name = name.Substring(px + 1);

                if (name.EndsWith("Dialog"))
                    name = name.Substr(0, name.Length - 6);

                entityType = name;
            }

            return entityType;
        }

        protected virtual string GetLocalTextPrefix()
        {
            localTextPrefix = localTextPrefix ?? ("Db." + GetEntityType() + ".");
            return localTextPrefix;
        }

        protected virtual string GetEntitySingular()
        {
            entitySingular = entitySingular ?? (Q.TryGetText(GetLocalTextPrefix() + "EntitySingular") ?? GetEntityType());
            return entitySingular;
        }

        protected virtual string GetEntityNameField()
        {
            if (entityNameField == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(NamePropertyAttribute), true);
                if (attributes.Length == 1)
                    entityNameField = attributes[0].As<NamePropertyAttribute>().NameProperty;
                else
                    entityNameField = "Name";
            }

            return entityNameField;
        }

        protected virtual string GetEntityIdField()
        {
            if (entityIdField == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IdPropertyAttribute), true);
                if (attributes.Length == 1)
                    entityIdField = attributes[0].As<IdPropertyAttribute>().IdProperty;
                else
                    entityIdField = "ID";
            }

            return entityIdField;
        }

        protected virtual string GetEntityIsActiveField()
        {
            if (entityIsActiveField == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IsActivePropertyAttribute), true);
                if (attributes.Length == 1)
                    entityIsActiveField = attributes[0].As<IsActivePropertyAttribute>().IsActiveProperty;
                else
                    entityIsActiveField = "IsActive";
            }

            return entityIsActiveField;
        }
    }
}