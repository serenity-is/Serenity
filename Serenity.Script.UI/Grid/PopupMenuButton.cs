
namespace Serenity
{
    using jQueryApi;
    using System;
    using System.Html;
    using System.Runtime.CompilerServices;

    public class PopupMenuButton : Widget<PopupMenuButtonOptions>
    {
        public override void Destroy()
        {
            if (this.options.Menu != null)
                this.options.Menu.Remove();

            base.Destroy();
        }

        public PopupMenuButton(jQueryObject div, PopupMenuButtonOptions opt)
            : base(div, opt)
        {
            var self = this;

            div.AddClass("s-PopupMenuButton");

            div.Click((jQueryEvent e) =>
            {
                e.PreventDefault();
                e.StopPropagation();

                if (options.OnPopup != null)
                    options.OnPopup();

                var menu = self.options.Menu;

                menu.Show().As<dynamic>().position(new
                {
                    my = options.PositionMy ?? "left top",
                    at = options.PositionAt ?? "left bottom",
                    of = self.element
                });

                var uq = self.uniqueName;

                jQuery.Document.One("click." + uq, x =>
                {
                    menu.Hide();
                });
            });

            self.options.Menu.Hide().AppendTo(Document.Body).AddClass("s-PopupMenu")
                .As<dynamic>().menu();
        }
    }

    [Imported, Serializable]
    public class PopupMenuButtonOptions
    {
        public jQueryObject Menu { get; set; }
        public Action OnPopup { get; set; }
        public string PositionMy { get; set; }
        public string PositionAt { get; set; }
    }
}