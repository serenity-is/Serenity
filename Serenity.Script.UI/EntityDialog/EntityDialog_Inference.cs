using System;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private string entityType;
        private string entitySingular;
        private string formKey;
        private string idProperty;
        private string isActiveProperty;
        private string localTextDbPrefix;
        private string nameProperty;
        private string service;

        protected virtual string GetEntityType()
        {
            if (entityType == null)
            {
                var typeAttributes = this.GetType().GetCustomAttributes(typeof(EntityTypeAttribute), true);
                if (typeAttributes.Length == 1)
                {
                    entityType = typeAttributes[0].As<EntityTypeAttribute>().Value;
                    return entityType;
                }

                // remove global namespace
                var name = this.GetType().FullName;
                var px = name.IndexOf(".");
                if (px >= 0)
                    name = name.Substring(px + 1);

                // don't like this kind of convention, make it obsolete soon...
                if (name.EndsWith("Dialog") || name.EndsWith("Control"))
                    name = name.Substr(0, name.Length - 6);
                else if (name.EndsWith("Panel"))
                    name = name.Substr(0, name.Length - 5);

                entityType = name;
            }

            return entityType;
        }

        protected virtual string GetFormKey()
        {
            if (formKey == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(FormKeyAttribute), true);
                if (attributes.Length >= 1)
                    formKey = attributes[0].As<FormKeyAttribute>().Value;
                else
                    formKey = GetEntityType();
            }

            return formKey;
        }

        protected virtual string GetLocalTextDbPrefix()
        {
            if (localTextDbPrefix == null)
            {
                localTextDbPrefix = GetLocalTextPrefix() ?? "";
                if (localTextDbPrefix.Length > 0 && !localTextDbPrefix.EndsWith("."))
                    localTextDbPrefix = "Db." + localTextDbPrefix + ".";
            }

            return localTextDbPrefix;
        }

        protected virtual string GetLocalTextPrefix()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(LocalTextPrefixAttribute), true);
            if (attributes.Length >= 1)
                return attributes[0].As<LocalTextPrefixAttribute>().Value;

            return GetEntityType();
        }

        protected virtual string GetEntitySingular()
        {
            if (entitySingular == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(ItemNameAttribute), true);
                if (attributes.Length >= 1)
                {
                    entitySingular = attributes[0].As<ItemNameAttribute>().Value;
                    entitySingular = LocalText.GetDefault(entitySingular, entitySingular);
                }
                else
                    entitySingular = (Q.TryGetText(GetLocalTextDbPrefix() + "EntitySingular") ?? GetEntityType());
            }

            return entitySingular;
        }

        protected virtual string GetNameProperty()
        {
            if (nameProperty == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(NamePropertyAttribute), true);
                if (attributes.Length >= 1)
                    nameProperty = attributes[0].As<NamePropertyAttribute>().Value;
                else
                    nameProperty = "Name";
            }

            return nameProperty;
        }

        protected virtual string GetIdProperty()
        {
            if (idProperty == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IdPropertyAttribute), true);
                if (attributes.Length >= 1)
                    idProperty = attributes[0].As<IdPropertyAttribute>().Value;
                else
                    idProperty = "ID";
            }

            return idProperty;
        }

        protected virtual string GetIsActiveProperty()
        {
            if (isActiveProperty == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IsActivePropertyAttribute), true);
                if (attributes.Length >= 1)
                    isActiveProperty = attributes[0].As<IsActivePropertyAttribute>().Value;
                else
                    isActiveProperty = "IsActive";
            }

            return isActiveProperty;
        }

        protected virtual string GetService()
        {
            if (service == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(ServiceAttribute), true);
                if (attributes.Length >= 1)
                    service = attributes[0].As<ServiceAttribute>().Value;
                else
                    service = this.GetEntityType().Replace('.', '/');
            }

            return service;
        }
    }
}