using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Element("<input type=\"hidden\"/>"), IncludeGenericArguments(false), ScriptName("LookupEditorBase")]
    public abstract class LookupEditorBase<TOptions, TItem> : Select2Editor<TOptions, TItem>
        where TOptions: LookupEditorOptions, new()
        where TItem: class, new()
    {
        private CascadedWidgetLink<Widget> cascadeLink;

        static LookupEditorBase()
        {
            Q.Prop(typeof(LookupEditorBase<LookupEditorOptions, object>), "cascadeField");
            Q.Prop(typeof(LookupEditorBase<LookupEditorOptions, object>), "cascadeFrom");
            Q.Prop(typeof(LookupEditorBase<LookupEditorOptions, object>), "cascadeValue");
            Q.Prop(typeof(LookupEditorBase<LookupEditorOptions, object>), "filterField");
            Q.Prop(typeof(LookupEditorBase<LookupEditorOptions, object>), "filterValue");
        }

        protected LookupEditorBase(jQueryObject hidden, TOptions opt)
            : base(hidden, opt)
        {
            SetCascadeFrom(options.CascadeFrom);

            var self = this;

            if (!IsAsyncWidget())
            {
                #pragma warning disable 618
                UpdateItems();
                Q.ScriptData.BindToChange("Lookup." + GetLookupKey(), this.uniqueName, () => self.UpdateItems());
                #pragma warning restore 618
            }

            if (options.InplaceAdd)
                AddInplaceCreate(Q.Text("Controls.SelectEditor.InplaceAdd"));
        }

        protected override Promise InitializeAsync()
        {
            return UpdateItemsAsync().Then(() => 
            {
                Q.ScriptData.BindToChange("Lookup." + GetLookupKey(), this.uniqueName, delegate()
                {
                    UpdateItemsAsync();
                });
            });
        }

        public override void Destroy()
        {
            Q.ScriptData.UnbindFromChange(this.uniqueName);
            element.Select2("destroy");

            base.Destroy();
        }

        protected virtual string GetLookupKey()
        {
            if (options.LookupKey != null)
                return options.LookupKey;

            var key = this.GetType().FullName;
            var idx = key.IndexOf(".");
            if (idx >= 0)
                key = key.Substring(idx + 1);

            if (key.EndsWith("Editor"))
                key = key.Substring(0, key.Length - 6);

            return key;
        }

        protected virtual Lookup<TItem> GetLookup()
        {
            #pragma warning disable 618
            return Q.GetLookup<TItem>(GetLookupKey());
            #pragma warning restore 618
        }

        protected virtual Promise<Lookup<TItem>> GetLookupAsync()
        {
            return Promise.Void.ThenAwait(() => {
                var key = GetLookupKey();
                return Q.GetLookupAsync<TItem>(key);
            });
        }

        [IncludeGenericArguments(false)]
        protected virtual IEnumerable<TItem> GetItems(Lookup<TItem> lookup)
        {
            return FilterItems(CascadeItems(lookup.Items)).ToList();
        }

        [IncludeGenericArguments(false)]
        protected virtual string GetItemText(TItem item, Lookup<TItem> lookup)
        {
            object textValue = (lookup.TextFormatter != null ? lookup.TextFormatter(item) : ((dynamic)item)[lookup.TextField]);
            return textValue == null ? "" : textValue.ToString();
        }

        [IncludeGenericArguments(false)]
        protected virtual bool GetItemDisabled(TItem item, Lookup<TItem> lookup)
        {
            return false;
        }

        protected virtual void UpdateItems()
        {
            #pragma warning disable 618
            var lookup = GetLookup();
            #pragma warning restore 618

            ClearItems();

            var items = GetItems(lookup);
            foreach (dynamic item in items)
            {
                var text = GetItemText(((object)item).As<TItem>(), lookup);
                var disabled = GetItemDisabled(((object)item).As<TItem>(), lookup);

                object idValue = item[lookup.IdField];
                string id = idValue == null ? "" : idValue.ToString();

                AddItem(new Select2Item
                {
                    Id = id,
                    Text = text,
                    Source = item,
                    Disabled = disabled
                });
            }
        }

        protected virtual Promise UpdateItemsAsync()
        {
            return GetLookupAsync().Then(lookup =>
            {
                ClearItems();

                var items = GetItems(lookup);
                foreach (dynamic item in items)
                {
                    var text = GetItemText(((object)item).As<TItem>(), lookup);
                    var disabled = GetItemDisabled(((object)item).As<TItem>(), lookup);

                    object idValue = item[lookup.IdField];
                    string id = idValue == null ? "" : idValue.ToString();

                    AddItem(new Select2Item
                    {
                        Id = id,
                        Text = text,
                        Source = item,
                        Disabled = disabled
                    });
                }
            });
        }

        protected virtual string GetDialogTypeKey()
        {
            if (options.DialogType != null)
                return options.DialogType;

            return GetLookupKey();
        }

        protected virtual void CreateEditDialog(Action<IEditDialog> callback)
        {
            var dialogTypeKey = GetDialogTypeKey();
            var dialogType = DialogTypeRegistry.Get(dialogTypeKey);
            Widget.CreateOfType(dialogType, options: new { }, init: dlg => callback((IEditDialog)dlg));
        }

        protected virtual void InitNewEntity(TItem entity)
        {
            if (OnInitNewEntity != null)
                OnInitNewEntity(entity);
        }

        public Action<TItem> OnInitNewEntity { get; set; }

        protected override void InplaceCreateClick(jQueryEvent e)
        {
            var self = this;
            CreateEditDialog(dialog =>
            {
                (dialog.As<Widget>()).BindToDataChange(this, (x, dci) =>
                {
                    Q.ReloadLookup(GetLookupKey());
                    self.UpdateItems();
                    self.Value = null;

                    if ((dci.Type == "create" || dci.Type == "update") && dci.EntityId != null)
                    {
                        self.Value = dci.EntityId.ToString();
                    }
                });

                if (this.Value.IsEmptyOrNull())
                {
                    var entity = new object().As<TItem>();
                    entity.As<JsDictionary<string, object>>()[GetLookup().TextField] = lastCreateTerm.TrimToEmpty();

                    if (OnInitNewEntity != null)
                        OnInitNewEntity(entity);

                    dialog.Load(entity, () => dialog.DialogOpen(), null);
                }
                else
                {
                    dialog.Load(this.Value, () => dialog.DialogOpen(), null);
                }
            });
        }

        protected virtual List<TItem> CascadeItems(List<TItem> items)
        {
            if (CascadeValue == null || (CascadeValue as string) == "")
            {
                if (!CascadeField.IsEmptyOrNull())
                    return new List<TItem>();

                return items;
            }

            var key = CascadeValue.ToString();
            return items.Where(x =>
            {
                var itemKey = ((dynamic)x)[CascadeField] ?? ReflectionUtils.GetPropertyValue(x, CascadeField);
                return itemKey != null && ((object)itemKey).ToString() == key;
            }).ToList();
        }

        protected virtual List<TItem> FilterItems(List<TItem> items)
        {
            if (FilterValue == null || (FilterValue as string) == "")
                return items;

            var key = FilterValue.ToString();
            return items.Where(x =>
            {
                var itemKey = ((dynamic)x)[FilterField] ?? ReflectionUtils.GetPropertyValue(x, FilterField);
                return itemKey != null && ((object)itemKey).ToString() == key;
            }).ToList();
        }

        protected virtual object GetCascadeFromValue(Widget parent)
        {
            return EditorUtils.GetValue(parent);
        }

        private void SetCascadeFrom(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (cascadeLink != null)
                {
                    cascadeLink.ParentID = null;
                    cascadeLink = null;
                }

                options.CascadeFrom = null;
                return;
            }

            cascadeLink = new CascadedWidgetLink<Widget>(this, p => CascadeValue = GetCascadeFromValue(p));
            cascadeLink.ParentID = value;
            options.CascadeFrom = value;
        }

        protected override Select2Options GetSelect2Options()
        {
            var opt = base.GetSelect2Options();

            if (options.MinimumResultsForSearch != null)
                opt.MinimumResultsForSearch = options.MinimumResultsForSearch.Value;

            if (options.InplaceAdd)
                opt.CreateSearchChoice = GetCreateSearchChoice();

            if (options.Multiple)
                opt.Multiple = true;

            return opt;
        }

        [Option]
        public virtual string CascadeFrom
        {
            get { return options.CascadeFrom; }
            set
            {
                if (value != options.CascadeFrom)
                {
                    SetCascadeFrom(value);
                    UpdateItems();
                }
            }
        }

        [Option]
        public virtual string CascadeField
        {
            get { return options.CascadeField ?? options.CascadeFrom; }
            set { options.CascadeField = value; }
        }

        [Option]
        public virtual object CascadeValue
        {
            get
            {
                return options.CascadeValue;
            }
            set
            {
                if (options.CascadeValue != value)
                {
                    options.CascadeValue = value;
                    Value = null;
                    UpdateItems();
                }
            }
        }

        [Option]
        public virtual string FilterField
        {
            get { return options.FilterField; }
            set { options.FilterField = value; }
        }

        [Option]
        public virtual object FilterValue
        {
            get
            {
                return options.FilterValue;
            }
            set
            {
                if (options.FilterValue != value)
                {
                    options.FilterValue = value;
                    Value = null;
                    UpdateItems();
                }
            }
        }
    }

    [Imported, IncludeGenericArguments(false), ScriptName("LookupEditorBase")]
    public abstract class LookupEditorBase<TItem> : LookupEditorBase<LookupEditorOptions, TItem>
        where TItem: class, new()
    {
        public LookupEditorBase(jQueryObject hidden, LookupEditorOptions options = null)
            : base(hidden, options)
        {
        }
    }
}