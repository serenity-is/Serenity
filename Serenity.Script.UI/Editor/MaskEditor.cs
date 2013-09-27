using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Maskeli Giriş", typeof(MaskedEditorOptions))]
    [Element("<input type=\"text\"/>")]
    public class MaskedEditor : Widget<MaskedEditorOptions>, IStringValue
    {
        public MaskedEditor(jQueryObject input, MaskedEditorOptions opt)
            : base(input, opt)
        {
            ((dynamic)input).mask(options.Mask, new
            {
                placeholder = options.Placeholder
            });
        }

        protected override MaskedEditorOptions GetDefaults()
        {
            return new MaskedEditorOptions
            {
                Mask = "",
                Placeholder = "_"
            };
        }

        public String Value
        {
            get
            {
                element.TriggerHandler("blur.mask");
                return element.GetValue();
            }
            set
            {
                element.Value(value);
            }
        }
    }

    [Serializable, Reflectable]
    public class MaskedEditorOptions
    {
        [DisplayName("Giriş Maskesi")]
        public string Mask { get; set; }
        [DisplayName("Yer Tutucu Karakter")]
        public string Placeholder { get; set; }
    }
}