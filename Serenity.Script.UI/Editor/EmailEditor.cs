using jQueryApi;
using System;
using System.ComponentModel;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    using jQueryApi.UI;

    [Editor, DisplayName("E-posta")]
    [Element("<input type=\"text\"/>")]
    public class EmailEditor : Widget<EmailEditorOptions>, IStringValue, IReadOnly
    {
        public EmailEditor(jQueryObject input, EmailEditorOptions opt)
            : base(input, opt)
        {
            RegisterValidationMethods();

            input.AddClass("emailuser").RemoveClass("flexify");

            var spanAt = J("<span/>")
                .Text("@")
                .AddClass("emailat")
                .InsertAfter(input);

            var domain = J("<input type=\"text\"/>")
                .AddClass("emaildomain")
                .AddClass("flexify")
                .InsertAfter(spanAt);

            domain.Bind("blur." + this.uniqueName, delegate
            {
                var validator = domain.Closest("form").GetDataValue("validator").As<jQueryValidator>();
                if (validator != null)
                    validator.ValidateElement(input[0]);
            });

            if (!options.Domain.IsEmptyOrNull())
                domain.Value(options.Domain);

            if (options.ReadOnlyDomain)
                domain.Attribute("readonly", "readonly").AddClass("disabled").Attribute("tabindex", "-1");

            input.Bind("keypress." + this.uniqueName, delegate(jQueryEvent e)
            {
                if (e.Which == (int)'@')
                {
                    e.PreventDefault();

                    if (!options.ReadOnlyDomain)
                    {
                        domain.Focus();
                        domain.Select();
                    }
                }
            });

            domain.Bind("keypress." + this.uniqueName, delegate(jQueryEvent e)
            {
                if (e.Which == (int)'@')
                {
                    e.PreventDefault();
                }
            });

            if (!options.ReadOnlyDomain)
            {
                input.Change(e =>
                {
                    this.Value = input.GetValue();
                });
            }
        }

        [InlineCode("this.optional({element})")]
        private static bool ValidatorIsOptional(Element element)
        {
            return false;
        }

        public static void RegisterValidationMethods()
        {
            if (jQueryValidator.Methods["emailuser"] == null)
                jQueryValidator.AddMethod("emailuser", (value, element) =>
                {
                    var domain = J(element).NextAll(".emaildomain");
                    if (domain.Length > 0 && domain.GetAttribute("readonly") == null)
                    {
                        if (ValidatorIsOptional(element) &&
                            ValidatorIsOptional(domain[0]))
                            return true;

                        value = value + "@" + domain.GetValue();

                        if (Q.Config.EmailAllowOnlyAscii)
                            return new Regex(
                                @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
                                .Test(value);

                        return new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$", "i")
                            .Test(value);
                    }
                    else
                    {
                        if (Q.Config.EmailAllowOnlyAscii)
                            return ValidatorIsOptional(element) ||
                                   new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+$").Test(value);

                        return ValidatorIsOptional(element) ||
                            new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))$", "i").Test(value);
                    }
                }, "Lütfen geçerli bir e-posta adresi giriniz!");
        }

        public string Value
        {
            get 
            {
                var domain = this.element.NextAll(".emaildomain");
                var value = this.element.GetValue();
                var domainValue = domain.GetValue();
                if (value.IsEmptyOrNull())
                {
                    if (options.ReadOnlyDomain || domainValue.IsEmptyOrNull())
                        return "";

                    return "@" + domainValue;
                }

                return value + "@" + domainValue;
            }
            set 
            {
                var domain = this.element.NextAll(".emaildomain");
                value = value.TrimToNull();
                if (value == null)
                {
                    if (!options.ReadOnlyDomain)
                        domain.Value("");

                    element.Value("");
                }
                else
                {
                    var parts = value.Split("@");
                    if (parts.Length > 1)
                    {
                        if (!options.ReadOnlyDomain)
                        {
                            domain.Value(parts[1]);
                            element.Value(parts[0]);
                        }
                        else if (!options.Domain.IsEmptyOrNull())
                        {
                            if (parts[1] != options.Domain)
                                element.Value(value); // hatayı görsünler!
                            else
                                element.Value(parts[0]);
                        }
                        else
                            element.Value(parts[0]);
                    }
                    else
                        element.Value(parts[0]);
                }
            }
        }

        public bool ReadOnly
        {
            get
            {
                var domain = element.NextAll(".emaildomain");
                return !(element.GetAttribute("readonly") == null && (!options.ReadOnlyDomain || domain.GetAttribute("readonly") == null));
            }
            set
            {
                var domain = element.NextAll(".emaildomain");
                if (value)
                {
                    element.Attribute("readonly", "readonly").AddClass("readonly");
                    if (!options.ReadOnlyDomain)
                        domain.Attribute("readonly", "readonly").AddClass("readonly");
                }
                else
                {
                    element.RemoveAttr("readonly").RemoveClass("readonly");
                    if (!options.ReadOnlyDomain)
                    {
                        domain.RemoveAttr("readonly").RemoveClass("readonly");
                    }
                }
            }
        }
    }

    [Serializable, Reflectable]
    public class EmailEditorOptions
    {
        [DisplayName("Etki Alanı")]
        public string Domain { get; set; }
        [DisplayName("Etki Alanı Salt Okunur")]
        public bool ReadOnlyDomain { get; set; }
    }
}