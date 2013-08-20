
namespace Serenity
{
    using jQueryApi;
    using System;
    using System.Html;
    using System.Runtime.CompilerServices;

    public class QuickSearchInput : Widget<QuickSearchInputOptions>
    {
        private string lastValue;
        private int timer;

        public QuickSearchInput(jQueryObject input, QuickSearchInputOptions opt)
            : base(input, opt)
        {
            input.Attribute("title", "aranacak kelimeyi giriniz")
                .Attribute("placeholder", "hızlı arama");


            lastValue = Q.Trim(input.GetValue() ?? "");

            var self = this;
            this.element.Bind("keyup." + this.uniqueName, delegate
            {
                self.CheckIfValueChanged();
            });

            this.element.Bind("change." + this.uniqueName, delegate
            {
                self.CheckIfValueChanged();
            });
        }

        protected void CheckIfValueChanged()
        {
            var value = Q.Trim(this.element.GetValue() ?? "");
            if (value == this.lastValue)
                return;

            this.element.Parent().ToggleClass(this.options.FilteredParentClass ?? "", Q.IsTrue(value.Length > 0));
            this.element.AddClass(this.options.LoadingParentClass ?? "");

            if (Q.IsTrue(this.timer))
                Window.ClearTimeout(this.timer);
            
            var self = this;
            this.timer = Window.SetTimeout(delegate
            {
                if (self.options.OnSearch != null)
                    self.options.OnSearch(value);

                self.element.RemoveClass(self.options.LoadingParentClass ?? "");

            }, this.options.TypeDelay);

            this.lastValue = value;
        }

        protected override QuickSearchInputOptions GetDefaults()
        {
            return new QuickSearchInputOptions
            {
                TypeDelay = 1000,
                LoadingParentClass = "s-QuickSearchLoading",
                FilteredParentClass = "s-QuickSearchFiltered"
            };
        }
    }

    [Imported, Serializable]
    public class QuickSearchInputOptions
    {
        public int TypeDelay { get; set; }
        public string LoadingParentClass { get; set; }
        public string FilteredParentClass { get; set; }
        public Action<string> OnSearch { get; set; }
    }
}