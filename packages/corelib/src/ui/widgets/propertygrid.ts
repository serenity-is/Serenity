import { Culture, Fluent, addClass, faIcon, getCustomAttribute, getTypeShortName, getjQuery, isBS3, isBS5Plus, localText, tryGetText, type PropertyItem } from "../../base";
import { Authorization, extend } from "../../q";
import { OptionsTypeAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { EditorTypeRegistry } from "../../types/editortyperegistry";
import { EditorUtils } from "../editors/editorutils";
import { ReflectionOptionsSetter } from "./reflectionoptionssetter";
import { Widget, WidgetProps } from "./widget";

@Decorators.registerClass('Serenity.PropertyGrid')
export class PropertyGrid<P extends PropertyGridOptions = PropertyGridOptions> extends Widget<P> {

    private editors: Widget<any>[];
    private items: PropertyItem[];

    protected renderContents() {

        this.domNode.classList.add('s-PropertyGrid');

        if (this.options.mode == null)
            this.options.mode = 1;

        this.editors = [];
        this.items = [];

        const items = this.options.items || [];
        const useTabs = items.some(x => !!x.tab);
        const bs3 = isBS3();

        if (useTabs) {
            var itemsWithoutTab = items.filter(f => !f.tab);
            if (itemsWithoutTab.length > 0) {
                this.createItems(this.domNode, itemsWithoutTab);
                Fluent("div").class("pad").appendTo(this.domNode);
            }

            var itemsWithTab = items.filter(f => f.tab);

            var tabs = Fluent("ul").class("nav nav-underline property-tabs").attr("role", "tablist").appendTo(this.domNode);
            var panes = Fluent("div").class("tab-content property-panes").appendTo(this.domNode);

            var tabIndex = 0;
            var i = 0;
            while (i < itemsWithTab.length) {
                var tabName = itemsWithTab[i].tab?.trim() ?? '';
                var withSameTab = [];

                var j = i;
                do {
                    withSameTab.push(itemsWithTab[j]);
                } while (++j < itemsWithTab.length &&
                    (itemsWithTab[j].tab?.trim() ?? '') === tabName);
                i = j;

                var tabId = this.uniqueName + '_Tab' + tabIndex;

                Fluent("li").class([!bs3 && "nav-item", bs3 && tabIndex === 0 && "active"])
                    .append(Fluent("a")
                        .class([!bs3 && "nav-link", !bs3 && tabIndex === 0 && "active"])
                        .attr("role", "tab").data((isBS5Plus() ? "bs-" : "") + "toggle", "tab")
                        .attr("href", "#" + tabId)
                        .text(this.determineText(tabName, prefix => prefix + 'Tabs.' + tabName)))
                    .appendTo(tabs);

                var pane = Fluent("div")
                    .class(["tab-pane fade", tabIndex === 0 && (isBS3() ? "in active" : "show active")])
                    .attr("id", tabId)
                    .attr("role", "tabpanel")
                    .appendTo(panes);

                this.createItems(pane.getNode(), withSameTab);
                tabIndex++;
            }
        }
        else {
            this.createItems(this.domNode, items);
        }

        this.updateInterface();
    }

    destroy() {

        if (this.editors) {
            for (var i = 0; i < this.editors.length; i++) {
                this.editors[i]?.destroy?.();
            }
            this.editors = null;
        }

        this.domNode.querySelectorAll<HTMLAnchorElement>('a.category-link').forEach(el => {
            Fluent.off(el, 'click', this.categoryLinkClick);
            el.remove();
        });

        super.destroy();
    }

    private createItems(container: HTMLElement, items: PropertyItem[]) {
        var categoryIndexes = {};
        var categoriesDiv = container;

        var useCategories = this.options.useCategories !== false && items.some(x => !!x.category);

        if (useCategories) {
            var linkContainer = Fluent("div").class("category-links").getNode();
            categoryIndexes = this.createCategoryLinks(linkContainer, items);
            if (Object.keys(categoryIndexes).length > 1) {
                container.appendChild(linkContainer);
            }
            else {
                linkContainer.querySelectorAll<HTMLAnchorElement>('a.category-link').forEach(el => {
                    Fluent.off(el, "click", this.categoryLinkClick);
                    el.remove();
                });
            }
        }

        categoriesDiv = Fluent("div").class("categories").appendTo(container).getNode();
        var fieldContainer: HTMLElement;
        if (useCategories) {
            fieldContainer = categoriesDiv;
        }
        else {
            fieldContainer = Fluent("div").class("category").appendTo(categoriesDiv).getNode();
        }
        var priorCategory = null;
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var category = item.category;
            if (category == null) {
                category = this.options.defaultCategory ?? '';
            }

            if (useCategories && priorCategory !== category) {
                var categoryDiv = this.createCategoryDiv(categoriesDiv,
                    categoryIndexes, category,
                    ((item.collapsible !== true) ? null :
                        item.collapsed ?? false));

                if (priorCategory == null) {
                    categoryDiv.classList.add("first-category");
                }
                priorCategory = category;
                fieldContainer = categoryDiv;
            }
            var editor = this.createField(fieldContainer, item);
            this.items.push(item);
            this.editors.push(editor);
        }
    }

    private createCategoryDiv(categoriesDiv: HTMLElement, categoryIndexes: { [key: string]: number },
        category: string, collapsed: boolean): HTMLElement {

        var categoryDiv = Fluent("div").class("category").appendTo(categoriesDiv);

        var title = Fluent("div")
            .class("category-title")
            .appendTo(categoryDiv)
            .append(Fluent("a")
                .class("category-anchor")
                .attr('name', this.idPrefix + 'Category' + categoryIndexes[category].toString()))
            .text(this.determineText(category, prefix => prefix + 'Categories.' + category));

        if (collapsed != null) {
            categoryDiv.addClass(["collapsible", collapsed && "collapsed"]);

            var img = Fluent("i").appendTo(title).class(faIcon(collapsed ? "plus" : "minus")).getNode();
            let categoryEl = categoryDiv.getNode();

            title.on("click", function () {
                categoryEl.classList.toggle('collapsed');
                img.classList.toggle('fa-plus');
                img.classList.toggle('fa-minus');
            });
        }

        return categoryDiv.getNode();
    }

    private categoryLinkClick = (e: Event) => {
        e.preventDefault();

        var title = document.querySelector('a[name=' + (e.target as HTMLElement).getAttribute('href')
            .toString().substring(1) + ']');

        if (title.closest('.category')?.classList.contains('collapsed'))
            title.closest('.category').querySelector<HTMLElement>(':scope > .category-title')?.click();

        var animate = function () {
            if (getjQuery()?.fn?.fadeTo) {
                getjQuery()(title).fadeTo(100, 0.5, function () {
                    getjQuery()(title).fadeTo(100, 1, function () { });
                });
            }
        };

        let intoView = title.closest('.category');
        if (intoView?.scrollIntoView) {
            intoView.scrollIntoView();
            animate();
        }
    }

    private determineText(text: string, getKey: (s: string) => string) {
        if (text != null && !text.startsWith('`')) {
            var local = tryGetText(text);
            if (local != null) {
                return local;
            }
        }

        if (text != null && text.startsWith('`')) {
            text = text.substring(1);
        }

        if (this.options.localTextPrefix) {
            var local1 = tryGetText(getKey(this.options.localTextPrefix));
            if (local1 != null) {
                return local1;
            }
        }

        return text;
    }

    private createField(container: HTMLElement, item: PropertyItem) {

        var fieldDiv = container.appendChild(document.createElement("div"));
        fieldDiv.classList.add("field");
        addClass(fieldDiv, item.name);
        fieldDiv.dataset.itemname = item.name;

        if (item.cssClass) {
            addClass(fieldDiv, item.cssClass);
        }

        if (item.formCssClass) {
            addClass(fieldDiv, item.formCssClass);
            if (item.formCssClass.indexOf('line-break-') >= 0) {
                var splitted = item.formCssClass.split(String.fromCharCode(32));
                const addLineBreak = (klass: string) => Fluent("div").class(klass).attr("style", "width: 100%").insertBefore(fieldDiv);
                if (splitted.indexOf('line-break-xs') >= 0) {
                    addLineBreak("line-break");
                }
                else if (splitted.indexOf('line-break-sm') >= 0) {
                    addLineBreak("line-break hidden-xs");
                }
                else if (splitted.indexOf('line-break-md') >= 0) {
                    addLineBreak("line-break hidden-sm");
                }
                else if (splitted.indexOf('line-break-lg') >= 0) {
                    addLineBreak("line-break hidden-md");
                }
            }
        }

        var editorId = this.idPrefix + item.name;
        var title = this.determineText(item.title, function (prefix) {
            return prefix + item.name;
        });

        var hint = this.determineText(item.hint, function (prefix1) {
            return prefix1 + item.name + '_Hint';
        });

        var placeHolder = this.determineText(item.placeholder, function (prefix2) {
            return prefix2 + item.name + '_Placeholder';
        });

        var label = Fluent("label")
            .class('caption')
            .attr('for', editorId)
            .attr('title', hint ?? title ?? "")
            .text(title ?? '')
            .appendTo(fieldDiv);

        if (item.labelWidth) {
            if (item.labelWidth === '0') {
                label.getNode().style.display = "none";
            }
            else {
                label.getNode().style.width = item.labelWidth;
            }
        }

        if (item.required) {
            Fluent("sup")
                .text("*")
                .attr('title', localText('Controls.PropertyGrid.RequiredHint'))
                .prependTo(label);
        }

        var editorType = EditorTypeRegistry.get(item.editorType ?? 'String') as typeof Widget<WidgetProps<P>>;

        var editorParams = item.editorParams;
        var optionsType = null;
        var optionsAttr = getCustomAttribute(editorType, OptionsTypeAttribute);

        if (optionsAttr) {
            optionsType = optionsAttr.value as any;
        }
        if (optionsType != null) {
            editorParams = extend(new optionsType(), item.editorParams);
        }
        else {
            editorParams = extend(new Object(), item.editorParams);
        }

        let editor = new editorType({
            ...editorParams,
            id: editorId,
            element: el => {
                Fluent(el).addClass("editor");

                if (Fluent.isInputLike(el))
                    el.setAttribute("name", item.name ?? "");

                if (placeHolder)
                    el.setAttribute("placeholder", placeHolder);

                fieldDiv.append(el);
            }
        }).init();

        if (getTypeShortName(editor) == "BooleanEditor" &&
            (item.editorParams == null || !!!item.editorParams['labelFor'])) {
            label.removeAttr('for');
        }

        if (getTypeShortName(editor) == "RadioButtonEditor" &&
            (item.editorParams == null || !!!item.editorParams['labelFor'])) {
            label.removeAttr('for');
        }

        if (item.maxLength != null) {
            PropertyGrid.setMaxLength(editor, item.maxLength);
        }

        if (item.editorParams != null) {
            ReflectionOptionsSetter.set(editor, item.editorParams);
        }

        Fluent("div").class('vx').appendTo(fieldDiv);
        Fluent("div").class('clear').appendTo(fieldDiv);

        return editor;
    }

    private getCategoryOrder(items: PropertyItem[]) {
        var order = 0;
        var result = {} as any;
        var categoryOrder = this.options.categoryOrder?.trim() || null;
        if (categoryOrder != null) {
            var split = categoryOrder.split(';');
            for (var s of split) {
                var x = s?.trim() || null;
                if (x == null) {
                    continue;
                }
                if (result[x] as any != null) {
                    continue;
                }
                result[x] = order++;
            }
        }

        for (var x1 of items) {
            var category = x1.category;
            if (category == null) {
                category = this.options.defaultCategory ?? '';
            }
            if (result[category] == null) {
                result[category] = order++;
            }
        }

        return result;
    }

    private createCategoryLinks(container: HTMLElement, items: PropertyItem[]) {
        var idx = 0;
        var itemIndex: Record<string, number> = {};
        var itemCategory: Record<string, string> = {};
        for (var x of items) {
            var name1 = x.name;
            var cat1 = x.category;
            if (cat1 == null) {
                cat1 = this.options.defaultCategory ?? '';
            }
            itemCategory[name1] = cat1;
            itemIndex[x.name] = idx++;
        }

        var self = this;
        var categoryOrder = this.getCategoryOrder(items);

        items.sort(function (x1, y) {
            var c = 0;
            var xcategory = itemCategory[x1.name];
            var ycategory = itemCategory[y.name];
            if (xcategory != ycategory) {
                var c1 = categoryOrder[xcategory];
                var c2 = categoryOrder[ycategory];
                if (c1 != null && c2 != null) {
                    c = c1 - c2;
                }
                else if (c1 != null) {
                    c = -1;
                }
                else if (c2 != null) {
                    c = 1;
                }
            }
            if (c === 0) {
                c = Culture.stringCompare(xcategory, ycategory);
            }
            if (c === 0) {
                c = itemIndex[x1.name] < itemIndex[y.name] ? -1 : (itemIndex[x1.name] > itemIndex[y.name] ? 1 : 0)
            }
            return c;
        });

        var categoryIndexes: Record<string, number> = {};
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var category = itemCategory[item.name];
            if (categoryIndexes[category] == null) {
                var index = Object.keys(categoryIndexes).length + 1;
                categoryIndexes[category] = index;
                if (index > 1) {
                    Fluent("span").class('separator').text('|').prependTo(container);
                }
                Fluent("a").class("category-link")
                    .text(this.determineText(category, prefix => prefix + 'Categories.' + category))
                    .attr('tabindex', '-1')
                    .attr('href', `#${this.idPrefix}Category${index}`)
                    .on("click", this.categoryLinkClick)
                    .prependTo(container);
            }
        }

        Fluent("div").class('clear').appendTo(container);
        return categoryIndexes;
    }

    get_editors(): Widget<any>[] {
        return this.editors;
    }

    get_items(): PropertyItem[] {
        return this.items;
    }

    get_idPrefix(): string {
        return this.idPrefix;
    }

    get_mode(): PropertyGridMode {
        return this.options.mode;
    }

    set_mode(value: PropertyGridMode) {
        if (this.options.mode !== value) {
            this.options.mode = value;
            this.updateInterface();
        }
    }

    private static setMaxLength(widget: Widget<any>, maxLength: number) {
        if (Fluent.isInputLike(widget.domNode)) {
            if (maxLength > 0) {
                widget.domNode.setAttribute('maxlength', (maxLength ?? 0).toString());
            }
            else {
                widget.domNode.removeAttribute('maxlength');
            }
        }
    }

    load(source: any): void {
        for (var i = 0; i < this.editors.length; i++) {
            var item = this.items[i];
            var editor = this.editors[i];
            if (!!(this.get_mode() === 1 && item.defaultValue != null) &&
                typeof (source[item.name]) === 'undefined') {
                source[item.name] = item.defaultValue;
            }

            EditorUtils.loadValue(editor, item, source);
        }
    }

    save(target?: any): any {
        if (target == null)
            target = Object.create(null);
        for (var i = 0; i < this.editors.length; i++) {
            var item = this.items[i];
            if (item.oneWay !== true && this.canModifyItem(item)) {
                var editor = this.editors[i];
                EditorUtils.saveValue(editor, item, target);
            }
        }
        return target;
    }

    public get value(): any {
        return this.save();
    }

    public set value(val: any) {
        if (val == null)
            val = Object.create(null);
        this.load(val);
    }

    private canModifyItem(item: PropertyItem) {
        if (this.get_mode() === PropertyGridMode.insert) {
            if (item.insertable === false) {
                return false;
            }

            if (item.insertPermission == null) {
                return true;
            }

            return Authorization.hasPermission(item.insertPermission);
        }
        else if (this.get_mode() === PropertyGridMode.update) {
            if (item.updatable === false) {
                return false;
            }

            if (item.updatePermission == null) {
                return true;
            }

            return Authorization.hasPermission(item.updatePermission);
        }
        return true;
    }

    updateInterface() {
        for (var i = 0; i < this.editors.length; i++) {
            var item = this.items[i];
            var editor = this.editors[i];
            var readOnly = item.readOnly === true || !this.canModifyItem(item);
            EditorUtils.setReadOnly(editor, readOnly);
            EditorUtils.setRequired(editor, !readOnly &&
                !!item.required && item.editorType !== 'Boolean');
            if (item.visible === false || item.readPermission != null ||
                item.insertPermission != null || item.updatePermission != null ||
                item.hideOnInsert === true || item.hideOnUpdate === true) {
                var hidden = (item.readPermission != null &&
                    !Authorization.hasPermission(item.readPermission)) ||
                    item.visible === false ||
                    (this.get_mode() === PropertyGridMode.insert && item.hideOnInsert === true) ||
                    (this.get_mode() === 2 && item.hideOnUpdate === true);

                editor.getGridField().toggle(!hidden);
            }
        }
    }

    enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void {
        for (var i = 0; i < this.editors.length; i++) {
            var item = this.items[i];
            var editor = this.editors[i];
            callback(item, editor);
        }
    }
}

export enum PropertyGridMode {
    insert = 1,
    update = 2
}

export interface PropertyGridOptions {
    idPrefix?: string;
    items: PropertyItem[];
    useCategories?: boolean;
    categoryOrder?: string;
    defaultCategory?: string;
    localTextPrefix?: string;
    mode?: PropertyGridMode;
}