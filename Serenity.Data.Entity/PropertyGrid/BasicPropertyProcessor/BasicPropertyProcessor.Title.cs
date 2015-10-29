using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetTitle(IPropertySource source, PropertyItem item)
        {
            if (source.Property != null)
            {
                var attr = source.Property.GetCustomAttribute<DisplayNameAttribute>(false);
                if (attr != null)
                    item.Title = attr.DisplayName;
            }

            if (item.Title == null)
            {
                var basedOnField = source.BasedOnField;

                if (!ReferenceEquals(null, basedOnField))
                {
                    item.Title = !object.ReferenceEquals(null, basedOnField.Caption) ?
                        basedOnField.Caption.Key : basedOnField.Title;
                }
                else
                    item.Title = item.Name;
            }
        }
    }
}