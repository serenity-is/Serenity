
namespace Serenity
{
    using jQueryApi;
    using System;
    using System.Html;
    using System.Runtime.CompilerServices;

    public class PopupToolButton : PopupMenuButton
    {
        public PopupToolButton(jQueryObject div, PopupToolButtonOptions opt)
            : base(div, opt)
        {
            div.AddClass("s-PopupToolButton");
            J("<b/>").AppendTo(div.Children(".button-outer").Children("span"));
        }

        protected override PopupMenuButtonOptions GetDefaults()
        {
            return new PopupToolButtonOptions
            {
            };
        }
    }

    [Imported, Serializable]
    public class PopupToolButtonOptions : PopupMenuButtonOptions
    {
    }
}