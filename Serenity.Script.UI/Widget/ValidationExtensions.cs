using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public delegate string CustomValidationRule(jQueryObject element);

    [Imported, ScriptName("VX")]
    public static class ValidationExtensions
    {
        static ValidationExtensions()
        {
            typeof(Widget).Prototype["addValidationRule"] = 
                new Func<string, Func<jQueryObject, string>, jQueryObject>(
                    (eventClass, rule) =>
                    {
                        return AddValidationRule(Script.This.As<Widget>().Element, eventClass, rule);
                    }
            );
        }

        [InlineCode("{widget}.addValidationRule({eventClass}, {rule})")]
        public static jQueryObject AddValidationRule(this Widget widget, string eventClass,
            Func<jQueryObject, string> rule)
        {
            return null;
        }

        public static jQueryObject AddValidationRule(this jQueryObject element, string eventClass,
            Func<jQueryObject, string> rule)
        {
            if (element.Length == 0)
                return element;

            if (rule == null)
                throw new Exception("rule is null!");

            element.AddClass("customValidate")
                .Bind("customValidate." + eventClass, rule.As<jQueryEventHandler>());

            return element;
        }

        public static jQueryObject RemoveValidationRule(this jQueryObject element, string eventClass)
        {
            element.Unbind("customValidate." + eventClass);
            return element;
        }

        public static bool ValidateElement(this jQueryValidator validator, Widget widget)
        {
            return validator.ValidateElement(widget.Element[0]);
        }
    }
}
