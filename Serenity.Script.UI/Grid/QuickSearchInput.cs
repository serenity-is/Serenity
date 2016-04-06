
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
            input.Attribute("title", Q.Text("Controls.QuickSearch.Hint"))
                .Attribute("placeholder", Q.Text("Controls.QuickSearch.Placeholder"));

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

            J("<span><i></i></span>").AddClass("quick-search-icon")
                .InsertBefore(input);

            if (options.Fields != null && options.Fields.Count > 0)
            {
                var a = J("<a/>").AddClass("quick-search-field")
                    .Attribute("title", Q.Text("Controls.QuickSearch.FieldSelection"))
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

            this.element.Bind("execute-search." + this.uniqueName, e =>
            {
                if (Q.IsTrue(this.timer))
                    Window.ClearTimeout(this.timer);

                SearchNow(Q.Trim(this.element.GetValue() ?? ""));
            });
        }

        private void UpdateInputPlaceHolder()
        {
            var qsf = this.element.PrevAll(".quick-search-field");

            if (field != null)
            {
                qsf.Text(field.Title);
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

            if (Q.IsTrue(this.timer))
                Window.ClearTimeout(this.timer);
            
            var self = this;
            this.timer = Window.SetTimeout(delegate
            {
                self.SearchNow(value);
            }, this.options.TypeDelay);

            this.lastValue = value;
        }

        private void SearchNow(string value)
        {
            this.element.Parent().ToggleClass(this.options.FilteredParentClass ?? "", Q.IsTrue(value.Length > 0));
            this.element.Parent().AddClass(this.options.LoadingParentClass ?? "")
                .AddClass(this.options.LoadingParentClass ?? "");

            Action<bool> done = delegate(bool results)
            {
                this.element.RemoveClass(this.options.LoadingParentClass ?? "")
                    .Parent().RemoveClass(this.options.LoadingParentClass ?? "");

                if (!results)
                {
                    this.element.Closest(".s-QuickSearchBar").Find(".quick-search-icon i").As<dynamic>()
                        .effect("shake", new { distance = 2 });
                }
            };

            if (this.options.OnSearch != null)
                this.options.OnSearch(field != null && !field.Name.IsEmptyOrNull() ? field.Name : null, value, done);
            else
                done(true);
        }

        public QuickSearchField Field
        {
            get { return field; }
            set
            {
                if (this.field != value)
                {
                    fieldChanged = true;
                    this.field = value;
                    UpdateInputPlaceHolder();
                    CheckIfValueChanged();
                }
            }
        }
    }

    [Serializable, Reflectable]
    public class QuickSearchInputOptions
    {
        public QuickSearchInputOptions()
        {
            TypeDelay = 500;
            LoadingParentClass = "s-QuickSearchLoading";
            FilteredParentClass = "s-QuickSearchFiltered";
        }

        public int TypeDelay { get; set; }
        public string LoadingParentClass { get; set; }
        public string FilteredParentClass { get; set; }
        public Action<string, string, Action<bool>> OnSearch { get; set; }
        public List<QuickSearchField> Fields { get; set; }
    }

    [Imported, Serializable]
    public class QuickSearchField
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}