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

                    var events = jQuery.Instance._data(element, "events");
                    if (events == null)
                        return true;

                    var handlers = events.customValidate;
                    if (handlers == null || handlers.length == 0)
                        return true;

                    var el = jQuery.Select(element);

                    for (var i = 0; i < handlers.length; i++)
                    {
                        var handler = (handlers[i].handler as CustomValidationRule);
                        if (handler != null)
                        {
                            var message = handler(el);
                            if (message != null)
                            {
                                el.Data("customValidationMessage", message);
                                return false;
                            }
                        }
                    }
                    return true;
                }, new Func<object, Element, string>((o, e) => {
                    return jQuery.Select(e).GetDataValue("customValidationMessage").As<string>();
                }).As<string>());
        }
    }
}