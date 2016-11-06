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

            items = options.Items ?? new List<PropertyItem>();

            div.AddClass("s-PropertyGrid");

            editors = new List<Widget>();

            var categoryIndexes = new JsDictionary<string, int>();

            var categoriesDiv = div;

            if (options.UseCategories)
            {
                var linkContainer = J("<div/>")
                    .AddClass("category-links");

                categoryIndexes = CreateCategoryLinks(linkContainer, items);

                if (categoryIndexes.Count > 1)
                    linkContainer.AppendTo(div);
                else
                    linkContainer.Find("a.category-link").Unbind("click", CategoryLinkClick).Remove();

                categoriesDiv = J("<div/>")
                    .AddClass("categories")
                    .AppendTo(div);
            }

            var fieldContainer = categoriesDiv;

            string priorCategory = null;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (options.UseCategories &&
                    priorCategory != item.Category)
                {
                    var categoryDiv = CreateCategoryDiv(categoriesDiv, categoryIndexes, item.Category, item.Collapsible ?? false, item.Collapsed ?? true);

                    if (priorCategory == null)
                        categoryDiv.AddClass("first-category");

                    priorCategory = item.Category;
                    fieldContainer = categoryDiv;
                }

                var editor = CreateField(fieldContainer, item);

                editors[i] = editor;
            }

            UpdateInterface();
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

        private jQueryObject CreateCategoryDiv(jQueryObject categoriesDiv, JsDictionary<string, int> categoryIndexes, string category, bool collapsible, bool collapsed)
        {
            var categoryId = options.IdPrefix + "Category" + categoryIndexes[category].ToString();

            var categoryTitleDiv = J("<div/>")
                .AddClass("category-title")
                .Append(J("<a/>")
                    .AddClass("category-anchor")
                    .Text(DetermineText(category, prefix => prefix + "Categories." + category))
                    .Attribute("name", categoryId));

            var categoryDiv = J("<div/>")
                .AddClass("category")
                // add category-scroll class for CategoryLinkClick
                .AddClass("category-scroll")
                .Attribute("id", categoryId);

            if (collapsible)
            {
                var button = J("<button class='btn btn-box-tool' data-target='#" + categoryId + "'></button>");
                var i = J("<i class='fa'></i>");
                categoryTitleDiv.Prepend(button.Append(i));
                categoryTitleDiv.Attribute("href", "#" + categoryId);
                categoryTitleDiv.Attribute("data-toggle", "collapse");
                categoryDiv.AddClass("collapse");
                if (collapsed)
                {
                    // remove category class in order to remove flex behaviour
                    // that doesn't work properly with bootstrap collapse
                    categoryDiv.RemoveClass("category");
                    i.AddClass("fa fa-plus");
                }
                else
                {
                    categoryDiv.AddClass("in");
                    i.AddClass("fa fa-minus");
                }

                // bind to on collapse / expand events
                categoryDiv.On("hide.bs.collapse", (e) =>
                {
                    i.RemoveClass("fa fa-minus");
                    i.AddClass("fa fa-plus");
                });
                categoryDiv.On("hidden.bs.collapse", (e) =>
                {
                    categoryDiv.RemoveClass("category");
                });
                categoryDiv.On("show.bs.collapse", (e) =>
                {
                    categoryDiv.AddClass("category");
                    i.RemoveClass("fa fa-plus");
                    i.AddClass("fa fa-minus");
                });
            }
            categoryTitleDiv.AppendTo(categoriesDiv);
            categoryDiv.AppendTo(categoriesDiv);

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
                if (result[x.Category] == null)
                    result[x.Category] = order++;
            }

            return result;
        }

        private JsDictionary<string, int> CreateCategoryLinks(jQueryObject container, List<PropertyItem> items)
        {
            int idx = 0;
            var itemIndex = new JsDictionary<string, int>();
            foreach (var x in items)
            {
                x.Category = x.Category ?? options.DefaultCategory ?? "";
                itemIndex[x.Name] = idx++;
            }

            var self = this;
            var categoryOrder = GetCategoryOrder(items);

            items.Sort((x, y) =>
            {
                var c = 0;

                if (x.Category != y.Category)
                {
                    var c1 = categoryOrder[x.Category];
                    var c2 = categoryOrder[y.Category];
                    if (c1 != null && c2 != null)
                        c = c1.Value - c2.Value;
                    else if (c1 != null)
                        c = -1;
                    else if (c2 != null)
                        c = 1;
                }

                if (c == 0)
                    c = String.Compare(x.Category, y.Category);

                if (c == 0)
                    c = itemIndex[x.Name].CompareTo(itemIndex[y.Name]);

                return c;
            });

            var categoryIndexes = new JsDictionary<string, int>();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (!categoryIndexes.ContainsKey(item.Category))
                {
                    int index = categoryIndexes.Count + 1;
                    categoryIndexes[item.Category] = index;

                    if (index > 1)
                        J("<span/>")
                            .AddClass("separator")
                            .Text("|")
                            .PrependTo(container);

                    J("<a/>")
                        .AddClass("category-link")
                        .Text(DetermineText(item.Category, prefix => prefix + "Categories." + item.Category))
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

            Action animate = delegate
            {
                title.FadeTo(100, 0.5, () => title.FadeTo(100, 1, () => { }));
            };

            var intoView = title.Parent().Next(".category-scroll");
            if (!intoView.HasClass("in"))
                ((dynamic)intoView).collapse("show");
            if (intoView.Closest(":scrollable(both)").Length == 0)
                animate();
            else
                ((dynamic)intoView).scrollintoview(new
                {
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
                    !(Mode == PropertyGridMode.Insert && item.Insertable == false) &&
                    !(Mode == PropertyGridMode.Update && item.Updatable == false))
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

        private void UpdateInterface()
        {
            for (var i = 0; i < editors.Count; i++)
            {
                var item = items[i];
                var editor = editors[i];

                bool readOnly = item.ReadOnly == true ||
                    (Mode == PropertyGridMode.Insert && item.Insertable == false) ||
                    (Mode == PropertyGridMode.Update && item.Updatable == false);

                EditorUtils.SetReadOnly(editor, readOnly);
                EditorUtils.SetRequired(editor, !readOnly && Q.IsTrue(item.Required) &&
                    (item.EditorType != "Boolean"));

                if (item.Visible == false ||
                    item.HideOnInsert == true ||
                    item.HideOnUpdate == true)
                {
                    bool hidden =
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