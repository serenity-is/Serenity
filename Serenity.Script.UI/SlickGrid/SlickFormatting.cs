using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public static class SlickFormatting
    {
        [IncludeGenericArguments(false), InlineCode("Serenity.EnumFormatter.format({TEnum}, {value})")]
        public static string GetEnumText<TEnum>(this TEnum? value)
            where TEnum: struct
        {
            return null;
        }

        [IncludeGenericArguments(false), InlineCode("Serenity.EnumFormatter.format({TEnum}, {value})")]
        public static string GetEnumText<TEnum>(this TEnum value)
            where TEnum : struct
        {
            return null;
        }

        public static string GetEnumText(string enumKey, string name)
        {
            return EnumFormatter.GetText(enumKey, name);
        }

        [IncludeGenericArguments(false)]
        public static SlickFormatter TreeToggle<TEntity>(Func<SlickRemoteView<TEntity>> getView, Func<TEntity, object> getId, 
            SlickFormatter formatter)
        {
            return delegate(SlickFormatterContext ctx)
            {
                var text = formatter(ctx);
                var view = getView();
                var indent = (((object)ctx.Item._indent).As<Int32?>() ?? 0);
                var spacer = "<span class=\"s-TreeIndent\" style=\"width:" + (15 * (indent)) + "px\"></span>";
                var id = getId(((object)ctx.Item).As<TEntity>());
                var idx = view.GetIdxById(id);
                var next = view.GetItemByIdx(idx.Value + 1);
                
                if (next != null)
                {
                    var nextIndent = ((object)(((dynamic)next)._indent)).As<Int32?>() ?? 0;
                    if (nextIndent > indent)
                    {
                        if (Q.IsTrue(ctx.Item._collapsed))
                            return spacer + "<span class=\"s-TreeToggle s-TreeExpand\"></span>" + text;
                        else
                            return spacer + "<span class=\"s-TreeToggle s-TreeCollapse\"></span>" + text;
                    }
                }

                return spacer + "<span class=\"s-TreeToggle\"></span>" + text;
            };
        }

        public static SlickFormatter Date(string format = null)
        {
            format = format ?? Q.Culture.DateFormat;

            return delegate(SlickFormatterContext ctx)
            {
                return Q.HtmlEncode(DateFormatter.Format(ctx.Value, format));
            };
        }

        public static SlickFormatter DateTime(string format = null)
        {
            format = format ?? Q.Culture.DateTimeFormat;

            return delegate(SlickFormatterContext ctx)
            {
                return Q.HtmlEncode(DateFormatter.Format(ctx.Value, format));
            };
        }

        public static SlickFormatter CheckBox()
        {
            return (ctx) => "<span class=\"check-box no-float " + (Q.IsTrue(ctx.Value) ? " checked" : "") + "\"></span>";
        }

        public static SlickFormatter Number(string format)
        {
            return delegate(SlickFormatterContext ctx)
            {
                return NumberFormatter.Format(ctx.Value, format);
            };
        }

        public static string GetItemType(jQueryObject link)
        {
            return link.GetDataValue("item-type") as string;
        }

        public static string GetItemId(jQueryObject link)
        {
            var value = link.GetDataValue("item-id");
            return value == null ? null : value.ToString();
        }

        public static string ItemLinkText(string itemType, object id, object text, string extraClass, bool encode)
        {
            return "<a" + (Script.IsValue(id) ? (" href=\"#" + itemType.Replace(".", "-") + "/" + id + "\"") : "") +
                " data-item-type=\"" + Q.AttrEncode(itemType) + "\"" +
                " data-item-id=\"" + Q.AttrEncode(id) + "\"" +
                " class=\"s-EditLink s-" + itemType.Replace(".", "-") + "Link" + (extraClass.IsEmptyOrNull() ? "" : (" " + extraClass)) + "\">" +
                (encode ? Q.HtmlEncode(text ?? "") : (text ?? "")) + "</a>";
        }

        public static SlickFormatter ItemLink(string itemType, string idField, 
            Func<SlickFormatterContext, string> getText, Func<SlickFormatterContext, string> cssClass = null, bool encode = false)
        {
            return delegate(SlickFormatterContext ctx)
            {
                return ItemLinkText(itemType, ctx.Item[idField], 
                    getText == null ? ctx.Value : getText(ctx),
                    cssClass == null ? "" : cssClass(ctx), encode);
            };
        }
    }
}