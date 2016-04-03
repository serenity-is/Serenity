using jQueryApi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Maskeli Giriş"), OptionsType(typeof(MaskedEditorOptions))]
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
        public MaskedEditorOptions()
        {
            Mask = "";
            Placeholder = "_";
        }

        [DisplayName("Giriş Maskesi")]
        public string Mask { get; set; }
        [DisplayName("Yer Tutucu Karakter")]
        public string Placeholder { get; set; }
    }
}