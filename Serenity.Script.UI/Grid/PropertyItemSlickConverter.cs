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
            result.Field = item.Name;
            result.Title = Q.TryGetText(item.Title) ?? item.Title;
            result.CssClass = item.CssClass;
            result.Width = Script.IsValue(item.Width) ? item.Width : 80;
            result.MinWidth = (!Script.IsValue(item.MinWidth) || item.MinWidth == 0) ? 30 : item.MinWidth;
            result.MaxWidth = (!Script.IsValue(item.MaxWidth) || item.MaxWidth == 0) ? null : (int?)item.MaxWidth;
            result.Resizable = !Script.IsValue(item.Resizable) || item.Resizable;

            if (Script.IsValue(item.FormatterType) && item.FormatterType.Length > 0)
            {
                var formatter = (ISlickFormatter)Activator.CreateInstance(FormatterTypeRegistry.Get(item.FormatterType));

                if (item.FormatterParams != null)
                    ReflectionOptionsSetter.Set(formatter, item.FormatterParams);

                result.Format = formatter.Format;
            }

            return result;
        }
    }
}