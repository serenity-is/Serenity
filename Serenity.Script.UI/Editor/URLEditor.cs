using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("URL")]
    public class URLEditor : StringEditor
    {
        public URLEditor(jQueryObject input)
            : base(input)
        {
            input.AddClass("url").Attribute("title",
                "URL 'http://www.site.com/sayfa' formatında girilmelidir.");

            input.Bind("blur." + this.uniqueName, delegate
            {
                var validator = input.Closest("form").GetDataValue("validator").As<jQueryValidator>();
                if (validator == null)
                    return;

                if (!input.HasClass("error"))
                    return;

                var value = input.GetValue().TrimToNull();
                if (value == null)
                    return;

                value = "http://" + value;

                if (((dynamic)(jQueryValidator.Methods["url"])).apply(validator, new object[] { value, input[0] }) == true)
                {
                    input.Value(value);
                    validator.ValidateElement(input[0]);
                }
            });
        }
    }
}