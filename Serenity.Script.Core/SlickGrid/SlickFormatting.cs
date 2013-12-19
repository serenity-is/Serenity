using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using jQueryApi;

namespace Serenity
{
    /// <summary>
    /// Helper for slick grid formatting
    /// </summary>
    public static class SlickFormatting
    {
        public static string GetEnumText(string enumKey, string enumValue)
        {
            if (Script.IsValue(enumValue))
                return Q.HtmlEncode(Q.Text("Enums." + enumKey + "." + enumValue));
            else
                return "";
        }

        public static SlickFormatter Enum(string enumKey)
        {
            return delegate(SlickFormatterContext ctx)
            {
                if (Script.IsValue(ctx.Value))
                    return Q.HtmlEncode(Q.Text("Enums." + enumKey + "." + ctx.Value));
                else
                    return "";
            };
        }

        public static SlickFormatter TreeToggle<TEntity>(Func<SlickRemoteView<TEntity>> getView, Func<TEntity, object> getId, 
            SlickFormatter formatter)
        {
            return delegate(SlickFormatterContext ctx)
            {
                var text = formatter(ctx);
                var view = getView();
                var indent = (((object)ctx.Item._indent).As<Int32?>() ?? 0);
                var spacer = "<span class=\"s-TreeIndent\" style=\"width:" + (15 * (indent)) + "px\"></span>";
                var id = getId(ctx.Item);
                var idx = view.GetIdxById(id);
                var next = view.GetItemByIdx(idx + 1);
                
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

        //public static string HtmlEncode(SlickFormatterContext ctx)
        //{
        //    if (!Script.IsValue(ctx.Value))
        //        return "";
        //    else
        //        return Q.HtmlEncode(ctx.Value.As<string>());
        //}

        //public static string YesNoNull(SlickFormatterContext ctx)
        //{
        //    if (ctx.Value == null)
        //        return "";
        //    else if (ctx.Value == (object)false)
        //        return "Hayır";
        //    else
        //        return "Evet";
        //}       

        //public static string DayHourAndMin(SlickFormatterContext ctx)
        //{
        //    return Pi.formatDayHourAndMin(ctx.Value);
        //}

        private static string FormatDate(object value, string format)
        {
            if (!Script.IsValue(value))
                return "";

            JsDate date;

            if (Script.TypeOf(value) == "date")
                date = value.As<JsDate>();
            else if (Script.TypeOf(value) == "string")
            {
                date = Q.Externals.ParseISODateTime(value.As<string>());
                if (date == null)
                    return Q.HtmlEncode(value.As<string>());
            }
            else
                return value.ToString();

            return Q.HtmlEncode(Q.FormatDate(date, format));

        }

        public static SlickFormatter Date(string format = null)
        {
            format = format ?? Q.Culture.DateFormat;

            return delegate(SlickFormatterContext ctx)
            {
                return Q.HtmlEncode(FormatDate(ctx.Value, format));
            };
        }

        public static SlickFormatter DateTime(string format = null)
        {
            format = format ?? Q.Culture.DateTimeFormat;

            return delegate(SlickFormatterContext ctx)
            {
                return Q.HtmlEncode(FormatDate(ctx.Value, format));
            };
        }

        public static SlickFormatter Number(string format)
        {
            return delegate(SlickFormatterContext ctx)
            {
                var value = ctx.Value;
                if (!Script.IsValue(ctx.Value) ||
                    Double.IsNaN(value.As<double>()))
                    return "";

                if (Script.TypeOf(value) == "number")
                    return Q.HtmlEncode(Q.FormatNumber(value.As<double>(), format));

                var dbl = Q.ParseDecimal(value.ToString());
                if (dbl == null)
                    return "";

                return Q.HtmlEncode(value.ToString());
            };
        }

        public static string GetItemType(jQueryObject link)
        {
            return GetItemType(link.GetAttribute("href"));
        }

        public static string GetItemType(string href)
        {
            if (href.IsEmptyOrNull())
                return null;

            if (href.StartsWith("#"))
                href = href.Substr(1);

            var idx = href.LastIndexOf('/');
            if (idx >= 0)
                href = href.Substr(0, idx);

            return href;
        }

        public static Int64? GetItemId(jQueryObject link)
        {
            return GetItemId(link.GetAttribute("href"));
        }

        public static Int64? GetItemId(string href)
        {
            if (href.IsEmptyOrNull())
                return null;

            if (href.StartsWith("#"))
                href = href.Substr(1);

            var idx = href.LastIndexOf('/');
            if (idx >= 0)
                href = href.Substr(idx + 1);

            return href.ConvertToId();
        }

        public static string ItemLinkText(string itemType, object id, object text, string extraClass)
        {
            return "<a" + (Script.IsValue(id) ? (" href=\"#" + itemType + "/" + id + "\"") : "") +
                " class=\"s-" + itemType + "Link" + (extraClass.IsEmptyOrNull() ? "" : (" " + extraClass)) + "\">" +
                Q.HtmlEncode(text ?? "") + "</a>";
        }

        public static SlickFormatter ItemLink(string itemType, string idField, 
            Func<SlickFormatterContext, string> getText, Func<SlickFormatterContext, string> cssClass = null)
        {
            return delegate(SlickFormatterContext ctx)
            {
                return ItemLinkText(itemType, ctx.Item[idField], 
                    getText == null ? ctx.Value : getText(ctx),
                    cssClass == null ? "" : cssClass(ctx));
            };
        }

        //public static string YesNo(SlickFormatterContext ctx)
        //{
        //    if (ctx.Value == null || ctx.Value == (object)false)
        //        return "Hayır";
        //    else
        //        return "Evet";
        //}
    }
}