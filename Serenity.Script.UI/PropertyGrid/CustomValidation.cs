using jQueryApi;
using System;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static class CustomValidation
    {
        [InlineCode("this.optional({element})")]
        private static bool ValidatorIsOptional(Element element)
        {
            return false;
        }

        public static void RegisterValidationMethods()
        {
            if (jQueryValidator.Methods["customValidate"] == null)
                jQueryValidator.AddMethod("customValidate", (value, element) =>
                {
                    bool result = ValidatorIsOptional(element);
                    if (element == null || Q.IsTrue(result))
                        return result;

                    Widget w = null;
                    var data = jQuery.Select(element).GetData();
                    foreach (var k in data.Keys)
                        if (data[k] != null && 
                            ((dynamic)data[k])["widgetName"] == k &&
                            ((dynamic)data[k])["uniqueName"] != null)
                        {
                            w = data[k].As<Widget>();
                        }

                    if (w != null)
                    {
                        var customValidator = w as ICustomValidate;
                        string message = customValidator.CustomValidate();
                        jQuery.Select(element).Data("customValidationMessage", message);
                        return message == null;
                    }
                    else 
                        return true;
                }, new Func<object, Element, string>((o, e) => {
                    return jQuery.Select(e).GetDataValue("customValidationMessage").As<string>();
                }).As<string>());
        }
    }
}