
namespace Serenity
{
    using jQueryApi;
    using System;
    using System.Collections.Generic;
    using System.Html;
    using System.Runtime.CompilerServices;

    public class QuickSearchInput : Widget<QuickSearchInputOptions>
    {
        private string lastValue;
        private int timer;
        private QuickSearchField field;
        private bool fieldChanged;

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

            if (options.Fields != null && options.Fields.Count > 0)
            {
                var a = J("<a/>").AddClass("quick-search-field")
                    .Attribute("title", "arama yapılacak alanı seç")
                    .InsertBefore(input);

                var menu = J("<ul></ul>").CSS("width", "120px");
                foreach (var item in options.Fields)
                {
                    var field = item;
                    J("<li><a/></li>").AppendTo(menu)
                        .Children().Attribute("href", "#")
                        .Text(item.Title ?? "")
                        .Click(delegate(jQueryEvent e) {
                            e.PreventDefault();
                            fieldChanged = self.field != field;
                            self.field = field;
                            UpdateInputPlaceHolder();
                            CheckIfValueChanged();
                        });
                }

                new PopupMenuButton(a, new PopupMenuButtonOptions
                {
                    PositionMy = "right top",
                    PositionAt = "right bottom",
                    Menu = menu
                });

                this.field = options.Fields[0];
                UpdateInputPlaceHolder();
            }
        }

        private void UpdateInputPlaceHolder()
        {
            var qsf = this.element.PrevAll(".quick-search-field");

            if (field != null)
            {
                qsf.Text(field.Title + ":");
            }
            else
            {
                qsf.Text("");
            }
        }

        protected void CheckIfValueChanged()
        {
            var value = Q.Trim(this.element.GetValue() ?? "");
            if (value == this.lastValue && (!fieldChanged || value.IsEmptyOrNull()))
            {
                fieldChanged = false;
                return;
            }

            fieldChanged = false;

            this.element.Parent().ToggleClass(this.options.FilteredParentClass ?? "", Q.IsTrue(value.Length > 0));
            this.element.AddClass(this.options.LoadingParentClass ?? "");

            if (Q.IsTrue(this.timer))
                Window.ClearTimeout(this.timer);
            
            var self = this;
            this.timer = Window.SetTimeout(delegate
            {
                if (self.options.OnSearch != null)
                    self.options.OnSearch(field != null && !field.Name.IsEmptyOrNull() ? field.Name : null, value);

                self.element.RemoveClass(self.options.LoadingParentClass ?? "");

            }, this.options.TypeDelay);

            this.lastValue = value;
        }
    }

    [Imported, Serializable]
    public class QuickSearchInputOptions
    {
        public QuickSearchInputOptions()
        {
            TypeDelay = 1000;
            LoadingParentClass = "s-QuickSearchLoading";
            FilteredParentClass = "s-QuickSearchFiltered";
        }

        public int TypeDelay { get; set; }
        public string LoadingParentClass { get; set; }
        public string FilteredParentClass { get; set; }
        public Action<string, string> OnSearch { get; set; }
        public List<QuickSearchField> Fields { get; set; }
    }

    [Imported, Serializable]
    public class QuickSearchField
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}