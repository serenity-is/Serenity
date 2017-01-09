using System;
using System.Collections.Generic;

namespace Serenity
{
    public static class PropertyItemSlickConverter
    {
        public static List<SlickColumn> ToSlickColumns(List<PropertyItem> items)
        {
            var result = new List<SlickColumn>();

            if (items == null)
                return result;

            for (var i = 0; i < items.Count; i++)
                result.Add(ToSlickColumn(items[i]));

            return result;
        }

        public static SlickColumn ToSlickColumn(PropertyItem item)
        {
            var result = new SlickColumn();

            result.SourceItem = item;
            result.Visible = item.Visible != false && item.FilterOnly != true &&
                (item.ReadPermission == null || Q.Authorization.HasPermission(item.ReadPermission));
            result.Field = item.Name;
            result.Title = Q.TryGetText(item.Title) ?? item.Title;
            result.CssClass = item.CssClass;
            result.HeaderCssClass = item.HeaderCssClass;
            result.Sortable = item.Sortable != false;
            result.SortOrder = item.SortOrder ?? 0;
            
            if (Script.IsValue(item.Alignment) && item.Alignment.Length > 0)
            {
                if (!result.CssClass.IsEmptyOrNull())
                    result.CssClass += " align-" + item.Alignment;
                else
                    result.CssClass = "align-" + item.Alignment;
            }

            result.Width = Script.IsValue(item.Width) ? item.Width : 80;
            result.MinWidth = item.MinWidth ?? 30;
            result.MaxWidth = (item.MaxWidth == null || item.MaxWidth == 0) ? null : item.MaxWidth;
            result.Resizable = item.Resizable == null || item.Resizable.Value;

            if (Script.IsValue(item.FormatterType) && item.FormatterType.Length > 0)
            {
                var formatter = (ISlickFormatter)Activator.CreateInstance(FormatterTypeRegistry.Get(item.FormatterType));

                if (item.FormatterParams != null)
                    ReflectionOptionsSetter.Set(formatter, item.FormatterParams);

                var initializer = formatter as IInitializeColumn;

                if (initializer != null)
                    initializer.InitializeColumn(result);

                result.Format = formatter.Format;
            }

            return result;
        }
    }
}