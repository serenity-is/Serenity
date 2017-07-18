using jQueryApi;
using System;
using System.Collections;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static class EditorUtils
    {
        private static PropertyItem dummy = new PropertyItem { Name = "_" };

        public static object GetValue(Widget editor)
        {
            var target = new JsDictionary();
            SaveValue(editor, dummy, target);
            return target["_"];
        }

        public static void SaveValue(Widget editor, PropertyItem item, dynamic target)
        {
            var getEditValue = editor as IGetEditValue;
            if (getEditValue != null)
            {
                getEditValue.GetEditValue(item, target);
                return;
            }

            var stringValue = editor as IStringValue;
            if (stringValue != null)
            {
                target[item.Name] = stringValue.Value;
                return;
            }

            var booleanValue = editor as IBooleanValue;
            if (booleanValue != null)
            {
                target[item.Name] = booleanValue.Value;
                return;
            }

            var doubleValue = editor as IDoubleValue;
            if (doubleValue != null)
            {
                var value = doubleValue.Value;
                target[item.Name] = Double.IsNaN(value.As<double>()) ? null : value;
                return;
            }

            if (Script.IsValue((editor as dynamic).getEditValue))
            {
                (editor as dynamic).getEditValue(item, target);
                return;
            }

            if (editor.Element.Is(":input"))
            {
                target[item.Name] = editor.Element.GetValue();
                return;
            }
        }

        public static void SetValue(Widget editor, object value)
        {
            var source = new { _ = value };
            LoadValue(editor, dummy, source);
        }

        public static void LoadValue(Widget editor, PropertyItem item, dynamic source)
        {
            var setEditValue = editor as ISetEditValue;
            if (setEditValue != null)
            {
                setEditValue.SetEditValue(source, item);
                return;
            }

            var stringValue = editor as IStringValue;
            if (stringValue != null)
            {
                var value = source[item.Name];
                if (value != null)
                    value = value.toString();
                stringValue.Value = value;
                return;
            }

            var booleanValue = editor as IBooleanValue;
            if (booleanValue != null)
            {
                object value = source[item.Name];
                if (Script.TypeOf(value) == "number")
                    booleanValue.Value = value.As<double>() > 0;
                else
                    booleanValue.Value = Q.IsTrue(value);

                return;
            }

            var doubleValue = editor as IDoubleValue;
            if (doubleValue != null)
            {
                var d = source[item.Name];
                if (d == null || (d is string && Q.IsTrimmedEmpty(d)))
                    doubleValue.Value = null;
                else if (d is string)
                    doubleValue.Value = Q.ParseDecimal(d);
                else if (d is Boolean)
                    doubleValue.Value = (Boolean)d ? 1 : 0;
                else
                    doubleValue.Value = d;

                return;
            }

            if (Script.IsValue((editor as dynamic).setEditValue))
            {
                (editor as dynamic).setEditValue(source, item);
                return;
            }

            if (editor.Element.Is(":input"))
            {
                var v = source[item.Name];
                if (!Script.IsValue(v))
                    editor.Element.Value("");
                else
                    editor.Element.Value(((object)v).As<string>());

                return;
            }
        }

        [ScriptName("setReadonly")]
        public static jQueryObject SetReadOnly(jQueryObject elements, bool isReadOnly)
        {
            elements.Each(delegate (int index, Element el)
            {
                jQueryObject elx = jQuery.FromElement(el);

                string type = elx.GetAttribute("type");

                if (elx.Is("select") || (type == "radio") || (type == "checkbox"))
                {
                    if (isReadOnly)
                    {
                        elx.AddClass("readonly").Attribute("disabled", "disabled");
                    }
                    else
                    {
                        elx.RemoveClass("readonly").RemoveAttr("disabled");
                    }
                }
                else
                {
                    if (isReadOnly)
                        elx.AddClass("readonly").Attribute("readonly", "readonly");
                    else
                        elx.RemoveClass("readonly").RemoveAttr("readonly");
                }

                return true;
            });

            return elements;
        }

        public static void SetReadOnly(Widget widget, bool isReadOnly)
        {
            var readOnly = widget as IReadOnly;
            if (readOnly != null)
                readOnly.ReadOnly = isReadOnly;
            else if (widget.Element.Is(":input"))
                SetReadOnly(widget.Element, isReadOnly);
        }

        public static void SetRequired(Widget widget, bool isRequired)
        {
            var req = widget as IValidateRequired;
            if (req != null)
                req.Required = isRequired;
            else if (widget.Element.Is(":input"))
                widget.Element.ToggleClass("required", Q.IsTrue(isRequired));

            var gridField = widget.GetGridField();
            var hasSupItem = gridField.Find("sup").GetItems().Length > 0;
            if (isRequired && !hasSupItem)
            {
                jQuery.FromHtml("<sup>*</sup>")
                    .Attribute("title", Q.Text("Controls.PropertyGrid.RequiredHint"))
                    .PrependTo(gridField.Find(".caption")[0]);
            }
            else if (!isRequired && hasSupItem)
            {
                jQuery.FromElement(gridField.Find("sup")[0]).Remove();
            }
        }
    }
}