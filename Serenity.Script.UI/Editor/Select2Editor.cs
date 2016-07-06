using jQueryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class Select2Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public object Source { get; set; }
        public bool Disabled { get; set; }
    }

    [Element("<input type=\"hidden\"/>"), IncludeGenericArguments(false), ScriptName("Select2Editor")]
    public abstract class Select2Editor<TOptions, TItem> : Widget<TOptions>, ISetEditValue, IGetEditValue, IStringValue
        where TOptions : class, new()
        where TItem: class
    {
        private bool multiple;
        protected List<Select2Item> items;
        protected JsDictionary<string, Select2Item> itemById;
        protected int pageSize = 100;
        protected string lastCreateTerm;

        static Select2Editor()
        {
            Q.Prop(typeof(Select2Editor<object, object>), "value");
            Q.Prop(typeof(Select2Editor<object, object>), "values");
        }

        public Select2Editor(jQueryObject hidden, TOptions opt)
            : base(hidden, opt)
        {
            items = new List<Select2Item>();
            itemById = new JsDictionary<string, Select2Item>();

            var emptyItemText = EmptyItemText();
            if (emptyItemText != null)
                hidden.Attribute("placeholder", emptyItemText);

            var select2Options = GetSelect2Options();
            multiple = Q.IsTrue(select2Options.Multiple);
            hidden.Select2(select2Options);
            
            hidden.Attribute("type", "text"); // jquery validate to work
            hidden.Bind2("change." + this.uniqueName, (e, x) =>
            {
                if (e.HasOriginalEvent() || Q.IsFalse(x))
                {
                    if (hidden.GetValidator() != null)
                        hidden.Valid();
                }
            });
        }

        protected virtual string EmptyItemText()
        {
            return element.GetAttribute("placeholder") ?? Q.Text("Controls.SelectEditor.EmptyItemText");
        }

        protected virtual Select2Options GetSelect2Options()
        {
            var emptyItemText = EmptyItemText();

            return new Select2Options
            {
                Data = items,
                PlaceHolder = !emptyItemText.IsEmptyOrNull() ? emptyItemText : null,
                AllowClear = emptyItemText != null,
                CreateSearchChoicePosition = "bottom",
                Query = delegate(Select2QueryOptions query)
                {
                    var term = query.Term.IsEmptyOrNull() ? "" : Q.Externals.StripDiacritics(query.Term ?? "").ToUpperCase();
                    var results = this.items.Filter(item =>
                    {
                        return (term == null ||
                            Q.Externals.StripDiacritics(item.Text ?? "").ToUpperCase().StartsWith(term));
                    });

                    results.AddRange(this.items.Filter(item =>
                    {
                        return term != null &&
                            !Q.Externals.StripDiacritics(item.Text ?? "").ToUpperCase().StartsWith(term) &&
                            Q.Externals.StripDiacritics(item.Text ?? "").ToUpperCase().IndexOf(term) >= 0;
                    }));

                    query.Callback(new Select2Result
                    {
                        Results = results.Slice((query.Page - 1) * pageSize, query.Page * pageSize),
                        More = results.Count >= query.Page * pageSize
                    });
                },
                InitSelection = delegate(jQueryObject element, Action<object> callback)
                {
                    var val = element.GetValue();

                    if (multiple)
                    {
                        var list = new List<object>();
                        foreach (var z in val.Split(","))
                        {
                            var item = itemById[z];
                            if (item != null)
                                list.Add(item);
                        }

                        callback(list);
                        return;
                    }

                    callback(itemById[val]);
                }
            };
        }

        public bool Delimited
        {
            get
            {
                return Q.IsTrue(options.As<dynamic>().delimited);
            }
        }

        protected void ClearItems()
        {
            this.items.Clear();
            this.itemById = new JsDictionary<string, Select2Item>();
        }

        protected void AddItem(Select2Item item)
        {
            this.items.Add(item);
            this.itemById[item.Id] = item;
        }

        protected void AddItem(string key, string text, TItem source = null, bool disabled = false)
        {
            AddItem(new Select2Item
            {
                Id = key,
                Text = text,
                Source = source,
                Disabled = disabled
            });
        }

        protected void AddInplaceCreate(string addTitle = null, string editTitle = null)
        {
            var self = this;

            addTitle = addTitle ?? Q.Text("Controls.SelectEditor.InplaceAdd");
            editTitle = editTitle ?? Q.Text("Controls.SelectEditor.InplaceEdit");

            var inplaceButton = J("<a><b/></a>").AddClass("inplace-button inplace-create")
                .Attribute("title", addTitle)
                .InsertAfter(this.element)
                .Click(e =>
                {
                    self.InplaceCreateClick(e);
                });

            this.Select2Container.Add(this.element).AddClass("has-inplace-button");

            this.Change(e =>
            {
                bool isNew = this.Value.IsEmptyOrNull();
                inplaceButton
                    .Attribute("title", isNew ? addTitle : editTitle)
                    .ToggleClass("edit", !isNew);
            });

            this.ChangeSelect2(e =>
            {
                if (this.Value == Int32.MinValue.ToString())
                {
                    this.Value = null;
                    InplaceCreateClick(null);
                }
            });
        }

        protected virtual void InplaceCreateClick(jQueryEvent e)
        {
        }

        public Func<string, object> GetCreateSearchChoice(
            Func<TItem, string> getName = null)
        {
            return s =>
            {
                s = (Q.Externals.StripDiacritics(s) ?? "").ToLower();
                lastCreateTerm = s;

                if (s.IsTrimmedEmpty())
                    return null;

                if (this.Items.Any(x =>
                {
                    var text = getName != null ? getName(x.Source.As<TItem>()) : x.Text;
                    return Q.Externals.StripDiacritics(text ?? "").ToLower() == s;
                }))
                {
                    return null;
                }

                if (!this.Items.Any(x => (Q.Externals.StripDiacritics(x.Text) ?? "").ToLower().Contains(s)))
                {
                    return new Select2Item
                    {
                        Id = Int32.MinValue.ToString(),
                        Text = Q.Text("Controls.SelectEditor.NoResultsClickToDefine")
                    };
                }

                return new Select2Item
                {
                    Id = Int32.MinValue.ToString(),
                    Text = Q.Text("Controls.SelectEditor.ClickToDefine")
                };
            };
        }

        public void SetEditValue(dynamic source, PropertyItem property)
        {
            object val = source[property.Name];
            if (Q.IsArray(val))
                Values = val.As<string[]>();
            else
                Value = val == null ? null : val.ToString();
        }

        public void GetEditValue(PropertyItem property, dynamic target)
        {
            if ((!multiple || Delimited))
                target[property.Name] = Value;
            else
                target[property.Name] = Values;
        }

        protected jQueryObject Select2Container
        {
            get { return this.element.PrevAll(".select2-container"); }
        }

        public List<Select2Item> Items
        {
            get { return this.items; }
        }

        public JsDictionary<string, Select2Item> ItemByKey
        {
            get { return this.itemById;}
        }

        public string Value
        {
            get
            {
                var val = this.element.Select2Get("val");

                if (val != null && Q.IsArray(val))
                    return ((string[])val).Join(",");

                return val as string;
            }
            set
            {
                if (value != Value)
                {
                    object val = value;

                    if (!string.IsNullOrEmpty(value) && multiple)
                    {
                        val = value.Split(',')
                            .Select(x => x.TrimToNull())
                            .Where(x => x != null)
                            .ToArray();
                    }

                    this.element.Select2("val", val).TriggerHandler("change", new object[] { true });
                }
            }
        }

        public string[] Values
        {
            get
            {
                var val = this.element.Select2Get("val");
                if (val == null)
                    return new string[0];

                if (Q.IsArray(val))
                    return ((string[])val);

                var str = val as string;
                if (string.IsNullOrEmpty(str))
                    return new string[0];

                return new string[] { str };
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    Value = null;
                    return;
                }

                Value = string.Join(",", value);
            }
        }

        public string Text
        {
            get
            {
                return ((dynamic)element.Select2Get("data") ?? new object()).text;
            }
        }
    }
}