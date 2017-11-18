
using jQueryApi;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public class PropertyGrid : Widget<PropertyGridOptions>
    {
        private static JsDictionary<string, Type> KnownEditorTypes;

        static PropertyGrid()
        {
            KnownEditorTypes = new JsDictionary<string, Type>();
        }

        private List<Widget> editors;
        private List<PropertyItem> items;

        public PropertyGrid(jQueryObject div, PropertyGridOptions opt)
            : base(div, opt)
        {
            if (!Script.IsValue(opt.Mode))
                opt.Mode = PropertyGridMode.Insert;

            div.AddClass("s-PropertyGrid");

            editors = new List<Widget>();
            items = options.Items ?? new List<PropertyItem>();

            var useTabs = Q.Any(items, x => !string.IsNullOrEmpty(x.Tab));

            if (useTabs)
            {
                var ul = J("<ul class='nav nav-tabs property-tabs' role='tablist'></ul>")
                    .AppendTo(this.element);

                var tc = J("<div class='tab-content property-panes'></div>")
                    .AppendTo(this.element);

                int tabIndex = 0;
                int i = 0;
                while (i < items.Count)
                {
                    var tab = items[i].Tab.TrimToEmpty();
                    var tabItems = new List<PropertyItem>();
                    var j = i;
                    do
                    {
                        tabItems.Add(items[j]);
                    }
                    while (++j < items.Count && items[j].Tab.TrimToEmpty() == tab);
                    i = j;

                    var li = J("<li><a data-toggle='tab' role='tab'></a></li>").AppendTo(ul);
                    if (tabIndex == 0)
                        li.AddClass("active");
                    var tabID = this.UniqueName + "_Tab" + tabIndex;
                    li.Children("a")
                        .Attribute("href", "#" + tabID)
                        .Text(DetermineText(tab, prefix => prefix + "Tabs." + tab));

                    var pane = J("<div class='tab-pane fade' role='tabpanel'>").AppendTo(tc);
                    if (tabIndex == 0)
                        pane.AddClass("in active");
                    pane.Attribute("id", tabID);
                    CreateItems(pane, tabItems);
                    tabIndex++;
                }
            }
            else
                CreateItems(this.element, items);

            UpdateInterface();
        }

        private void CreateItems(jQueryObject container, List<PropertyItem> items)
        { 
            var categoryIndexes = new JsDictionary<string, int>();
            var categoriesDiv = container;
            
            var useCategories = options.UseCategories &&
                Q.Any(items, x => !string.IsNullOrEmpty(x.Category));

            if (options.UseCategories)
            {
                var linkContainer = J("<div/>")
                    .AddClass("category-links");

                categoryIndexes = CreateCategoryLinks(linkContainer, items);

                if (categoryIndexes.Count > 1)
                    linkContainer.AppendTo(container);
                else
                    linkContainer.Find("a.category-link").Unbind("click", CategoryLinkClick).Remove();
            }

            categoriesDiv = J("<div/>")
                .AddClass("categories")
                .AppendTo(container);

            jQueryObject fieldContainer;
            if (useCategories)
            {
                fieldContainer = categoriesDiv;
            }
            else
            { 
                fieldContainer = J("<div/>")
                    .AddClass("category")
                    .AppendTo(categoriesDiv);
            }

            string priorCategory = null;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var category = (item.Category ?? options.DefaultCategory ?? "");
                if (useCategories && priorCategory != category)
                {
                    var categoryDiv = CreateCategoryDiv(categoriesDiv, categoryIndexes, category, 
                        item.Collapsible != true ? (bool?)null : item.Collapsed ?? false);

                    if (priorCategory == null)
                        categoryDiv.AddClass("first-category");

                    priorCategory = category;
                    fieldContainer = categoryDiv;
                }

                var editor = CreateField(fieldContainer, item);

                editors.Add(editor);
            }
        }

        public override void Destroy()
        {
            if (editors != null)
            {
                for (var i = 0; i < editors.Count; i++)
                    editors[i].Destroy();
                editors = null;
            }

            this.element.Find("a.category-link").Unbind("click", CategoryLinkClick).Remove();

            base.Destroy();
        }

        private jQueryObject CreateCategoryDiv(jQueryObject categoriesDiv, JsDictionary<string, int> categoryIndexes, string category,
            bool? collapsed)
        {
            var categoryDiv = J("<div/>")
                .AddClass("category")
                .AppendTo(categoriesDiv);

            var title = J("<div/>")
                .AddClass("category-title")
                .Append(J("<a/>")
                    .AddClass("category-anchor")
                    .Text(DetermineText(category, prefix => prefix + "Categories." + category))
                    .Attribute("name", options.IdPrefix + "Category" + categoryIndexes[category].ToString()))
                .AppendTo(categoryDiv);

            if (collapsed != null)
            {
                categoryDiv.AddClass(collapsed == true ? "collapsible collapsed" : "collapsible");
                var img = J("<i/>").AddClass(collapsed == true ? "fa fa-plus" : "fa fa-minus").AppendTo(title);

                title.Click(e =>
                {
                    categoryDiv.ToggleClass("collapsed");
                    img.ToggleClass("fa-plus").ToggleClass("fa-minus");
                });
            }
              
            return categoryDiv;
        }

        private string DetermineText(string text, Func<string, string> getKey)
        {
            if (text != null &&
                !text.StartsWith("`"))
            {
                var local = Q.TryGetText(text);
                if (local != null)
                    return local;
            }

            if (text != null && text.StartsWith("`"))
                text = text.Substr(1);

            if (!options.LocalTextPrefix.IsEmptyOrNull())
            {
                var local = Q.TryGetText(getKey(options.LocalTextPrefix));
                if (local != null)
                    return local;
            }

            return text;
        }

        private Widget CreateField(jQueryObject container, PropertyItem item)
        {
            var fieldDiv = J("<div/>")
                .AddClass("field")
                .AddClass(item.Name)
                .Data("PropertyItem", item)
                .AppendTo(container);

            if (!String.IsNullOrEmpty(item.CssClass))
                fieldDiv.AddClass(item.CssClass);

            if (!String.IsNullOrEmpty(item.FormCssClass))
            {
                fieldDiv.AddClass(item.FormCssClass);
                if (item.FormCssClass.IndexOf("-break") >= 0)
                {
                    var splitted = item.FormCssClass.Split(' ');
                    if (splitted.IndexOf("col-break") >= 0)
                        J("<div class='break-before' style='width: 100%' />").InsertAfter(fieldDiv);
                    else if (splitted.IndexOf("col-xs-break") >= 0)
                        J("<div class='break-before visible-xs' style='width: 100%' />").InsertBefore(fieldDiv);
                    else if (splitted.IndexOf("col-sm-break") >= 0)
                        J("<div class='break-before visible-sm' style='width: 100%' />").InsertBefore(fieldDiv);
                    else if (splitted.IndexOf("col-md-break") >= 0)
                        J("<div class='break-before visible-md' style='width: 100%' />").InsertBefore(fieldDiv);
                    else if (splitted.IndexOf("col-lg-break") >= 0)
                        J("<div class='break-before visible-lg' style='width: 100%' />").InsertBefore(fieldDiv);
                }
            }

            string editorId = options.IdPrefix + item.Name;

            string title = DetermineText(item.Title, prefix => prefix + item.Name);
            string hint = DetermineText(item.Hint, prefix => prefix + item.Name + "_Hint");
            string placeHolder = DetermineText(item.Placeholder, prefix => prefix + item.Name + "_Placeholder");

            var label = J("<label/>")
                .AddClass("caption")
                .Attribute("for", editorId)
                .Attribute("title", hint ?? title ?? "")
                .Html(title ?? "")
                .AppendTo(fieldDiv);

            if (!string.IsNullOrEmpty(item.LabelWidth))
            {
                if (item.LabelWidth == "0")
                    label.Hide();
                else
                    label.CSS("width", item.LabelWidth);
            }

            if (item.Required == true)
                J("<sup>*</sup>")
                    .Attribute("title", Q.Text("Controls.PropertyGrid.RequiredHint"))
                    .PrependTo(label);

            var editorType = EditorTypeRegistry.Get(item.EditorType ?? "String");
            var elementAttr = editorType.GetCustomAttributes(typeof(ElementAttribute), true);
            string elementHtml = (elementAttr.Length > 0) ? elementAttr[0].As<ElementAttribute>().Value : "<input/>";

            var element = Widget.ElementFor(editorType)
                .AddClass("editor")
                .AddClass("flexify")
                .Attribute("id", editorId)
                .AppendTo(fieldDiv);

            if (element.Is(":input"))
                element.Attribute("name", item.Name ?? "");

            if (!placeHolder.IsEmptyOrNull())
                element.Attribute("placeholder", placeHolder);

            object editorParams = item.EditorParams;
            Type optionsType = null;
            var optionsAttr = editorType.GetCustomAttributes(typeof(OptionsTypeAttribute), true);
            if (optionsAttr != null && optionsAttr.Length > 0)
            {
                optionsType = optionsAttr[0].As<OptionsTypeAttribute>().OptionsType;
            }

            Widget editor;

            if (optionsType != null)
            {
                editorParams = jQuery.ExtendObject(Activator.CreateInstance(optionsType), item.EditorParams);

                editor = (Widget)(Activator.CreateInstance(editorType, element, editorParams));
            }
            else
            {
                editorParams = jQuery.ExtendObject(new object(), item.EditorParams);
                editor = (Widget)(Activator.CreateInstance(editorType, element, editorParams));
            }

            editor.Initialize();

            if (editor is BooleanEditor && (item.EditorParams == null || !Q.IsTrue(item.EditorParams["labelFor"])))
                label.RemoveAttr("for");

            if (editor is RadioButtonEditor && (item.EditorParams == null || !Q.IsTrue(item.EditorParams["labelFor"])))
                label.RemoveAttr("for");

            if (Script.IsValue(item.MaxLength))
                SetMaxLength(editor, item.MaxLength.Value);

            if (item.EditorParams != null)
                ReflectionOptionsSetter.Set(editor, item.EditorParams);

            J("<div/>")
                .AddClass("vx")
                .AppendTo(fieldDiv);

            J("<div/>")
                .AddClass("clear")
                .AppendTo(fieldDiv);

            return editor;
        }

        private JsDictionary<string, int?> GetCategoryOrder(List<PropertyItem> items)
        {
            int order = 0;
            var result = new JsDictionary<string, int?>();

            var categoryOrder = options.CategoryOrder.TrimToNull();
            if (categoryOrder != null)
            {
                var split = categoryOrder.Split(";");
                foreach (var s in split)
                {
                    var x = s.TrimToNull();
                    if (x == null)
                        continue;

                    if (result[x] != null)
                        continue;

                    result[x] = order++;
                }
            }

            foreach (var x in items)
            {
                var category = x.Category ?? options.DefaultCategory ?? "";
                if (result[category] == null)
                    result[category] = order++;
            }

            return result;
        }

        private JsDictionary<string, int> CreateCategoryLinks(jQueryObject container, List<PropertyItem> items)
        {
            int idx = 0;
            var itemIndex = new JsDictionary<string, int>();
            var itemCategory = new JsDictionary<string, string>();
            foreach (var x in items)
            {
                itemCategory[x.Name] = x.Category ?? options.DefaultCategory ?? "";
                itemIndex[x.Name] = idx++;
            }

            var self = this;
            var categoryOrder = GetCategoryOrder(items);
            
            items.Sort((x, y) => 
            {
                var c = 0;

                var xcategory = itemCategory[x.Name];
                var ycategory = itemCategory[y.Name];
                if (xcategory != ycategory)
                {
                    var c1 = categoryOrder[xcategory];
                    var c2 = categoryOrder[ycategory];
                    if (c1 != null && c2 != null)
                        c = c1.Value - c2.Value;
                    else if (c1 != null)
                        c = -1;
                    else if (c2 != null)
                        c = 1;
                }

                if (c == 0)
                    c = String.Compare(xcategory, ycategory);

                if (c == 0)
                    c = itemIndex[x.Name].CompareTo(itemIndex[y.Name]);

                return c;
            });

            var categoryIndexes = new JsDictionary<string, int>();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var category = itemCategory[item.Name];

                if (!categoryIndexes.ContainsKey(category))
                {
                    int index = categoryIndexes.Count + 1;
                    categoryIndexes[category] = index;

                    if (index > 1)
                        J("<span/>")
                            .AddClass("separator")
                            .Text("|")
                            .PrependTo(container);
                    
                    J("<a/>")
                        .AddClass("category-link")
                        .Text(DetermineText(category, prefix => prefix + "Categories." + category))
                        .Attribute("tabindex", "-1")
                        .Attribute("href", "#" + options.IdPrefix + "Category" + index.ToString())
                        .Click(CategoryLinkClick)
                        .PrependTo(container);
                }
            }

            J("<div/>")
                .AddClass("clear")
                .AppendTo(container);

            return categoryIndexes;
        }

        private static void CategoryLinkClick(jQueryEvent e)
        {
            e.PreventDefault();

            var title = J("a[name=" + e.Target.GetAttribute("href").ToString().Substr(1) + "]");

            if (title.Closest(".category").HasClass("collapsed"))
                title.Closest(".category").Children(".category-title").Click();

            Action animate = delegate {
                title.FadeTo(100, 0.5, () => title.FadeTo(100, 1, () => { }));
            };


            var intoView = title.Closest(".category");
            if (intoView.Closest(":scrollable(both)").Length == 0)
                animate();
            else
                ((dynamic)intoView).scrollintoview(new {
                    duration = "fast",
                    direction = "y",
                    complete = animate
                });
        }

        public Widget[] Editors
        {
            get { return this.editors.As<Widget[]>(); }
        }

        public PropertyItem[] Items 
        { 
            get { return this.items.As<PropertyItem[]>(); }
        }

        public string IdPrefix
        {
            get { return options.IdPrefix; }
        }

        public PropertyGridMode Mode
        {
            get { return options.Mode; }
            set
            {
                if (options.Mode != value)
                {
                    options.Mode = value;
                    UpdateInterface();
                }
            }
        }

        public void Load(dynamic source)
        {
            for (var i = 0; i < editors.Count; i++)
            {
                var item = items[i];
                var editor = editors[i];

                if (Mode == PropertyGridMode.Insert &&
                    !Script.IsNullOrUndefined(item.DefaultValue) &&
                    Script.IsUndefined(source[item.Name]))
                    source[item.Name] = item.DefaultValue;

                EditorUtils.LoadValue(editor, item, source);
            }
        }

        [Obsolete("Use EditorUtils.LoadValue")]
        public static void LoadEditorValue(Widget editor, PropertyItem item, dynamic source)
        {
        }

        public void Save(dynamic target)
        {
            for (var i = 0; i < editors.Count; i++)
            {
                var item = items[i];
                if (item.OneWay != true &&
                    CanModifyItem(item))
                {
                    var editor = editors[i];
                    EditorUtils.SaveValue(editor, item, target);
                }
            }
        }

        [Obsolete("Use EditorUtils.SaveValue")]
        public static void SaveEditorValue(Widget editor, PropertyItem item, dynamic target)
        {
            EditorUtils.SaveValue(editor, item, target);
        }

        private bool CanModifyItem(PropertyItem item)
        {
            if (Mode == PropertyGridMode.Insert)
            {
                if (item.Insertable == false)
                    return false;

                if (item.InsertPermission == null)
                    return true;

                return Q.Authorization.HasPermission(item.InsertPermission);
            }
            else if (Mode == PropertyGridMode.Update)
            {
                if (item.Updatable == false)
                    return false;

                if (item.UpdatePermission == null)
                    return true;

                return Q.Authorization.HasPermission(item.UpdatePermission);
            }

            return true;
        }

        private void UpdateInterface()
        {
            for (var i = 0; i < editors.Count; i++)
            {
                var item = items[i];
                var editor = editors[i];

                bool readOnly = item.ReadOnly == true || !CanModifyItem(item);
                EditorUtils.SetReadOnly(editor, readOnly);
                EditorUtils.SetRequired(editor, !readOnly && Q.IsTrue(item.Required) && 
                    (item.EditorType != "Boolean"));

                if (item.Visible == false ||
                    item.ReadPermission != null ||
                    item.InsertPermission != null ||
                    item.UpdatePermission != null ||
                    item.HideOnInsert == true ||
                    item.HideOnUpdate == true)
                {
                    bool hidden = 
                        (item.ReadPermission != null && !Q.Authorization.HasPermission(item.ReadPermission)) ||
                        item.Visible == false ||
                        (Mode == PropertyGridMode.Insert && item.HideOnInsert == true) ||
                        (Mode == PropertyGridMode.Update && item.HideOnUpdate == true);

                    editor.GetGridField().Toggle(!hidden);
                }
            }
        }

        public void EnumerateItems(Action<PropertyItem, Widget> callback)
        {
            for (var i = 0; i < editors.Count; i++)
            {
                var item = items[i];
                var editor = editors[i];

                callback(item, editor);
            }
        }

        private static void SetMaxLength(Widget widget, int maxLength)
        {
            if (widget.Element.Is(":input"))
            {
                if (maxLength > 0)
                    widget.Element.Attribute("maxlength", maxLength.ToString());
                else
                    widget.Element.RemoveAttr("maxlength");
            }
        }

        [Obsolete("Use EditorUtils.SetRequired")]
        public static void SetRequired(Widget widget, bool isRequired)
        {
            EditorUtils.SetRequired(widget, isRequired);
        }

        [Obsolete("Use EditorUtils.SetReadOnly")]
        public static void SetReadOnly(Widget widget, bool isReadOnly)
        {
            EditorUtils.SetReadOnly(widget, isReadOnly);
        }

        [Obsolete("Use EditorUtils.SetReadOnly")]
        public static jQueryObject SetReadOnly(jQueryObject elements, bool isReadOnly)
        {
            return EditorUtils.SetReadOnly(elements, isReadOnly);
        }
    }
}