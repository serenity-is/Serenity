import { Fluent, addClass, faIcon, getCustomAttribute, isBS3, isBS5Plus, isPromiseLike, localText, tryGetText, type PropertyItem } from "../../base";
import { Authorization, extend } from "../../q";
import { OptionsTypeAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { EditorType } from "../../types/editortype";
import { EditorTypeRegistry } from "../../types/editortyperegistry";
import { EditorUtils } from "../editors/editorutils";
import { ReflectionOptionsSetter } from "./reflectionoptionssetter";
import { Widget } from "./widget";

@Decorators.registerClass('Serenity.PropertyGrid')
export class PropertyGrid<P extends PropertyGridOptions = PropertyGridOptions> extends Widget<P> {

    private editorPromises: PromiseLike<void>[];
    private editors: Widget<any>[];
    private items: PropertyItem[];

    protected renderContents(): any {

        this.domNode.classList.add('s-PropertyGrid');

        if (this.options.mode == null)
            this.options.mode = 1;

        this.editors = [];
        this.editorPromises = [];
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
            this.editorPromises = null;
        }

        super.destroy();
    }

    private createItems(container: HTMLElement, items: PropertyItem[]) {
        var categoriesDiv = container;

        categoriesDiv = Fluent("div").class("categories").appendTo(container).getNode();
        var fieldContainer: HTMLElement = null;
        var priorCategory = null;
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var category = item.category ?? '';

            if (!fieldContainer || priorCategory !== category) {
                var categoryDiv = this.createCategoryDiv(categoriesDiv, category,
                    ((item.collapsible !== true) ? null : item.collapsed ?? false));

                priorCategory = category;
                fieldContainer = categoryDiv;
            }
            this.createField(fieldContainer, item);
        }
    }

    private createCategoryDiv(categoriesDiv: HTMLElement, category: string, collapsed: boolean): HTMLElement {

        var categoryDiv = Fluent("div")
            .class("category")
            .appendTo(categoriesDiv);

        if (category) {
            let key = category;
            let idx = category.lastIndexOf('.Categories.');
            if (idx >= 0) {
                key = category.substring(idx + 12);
            }
            categoryDiv.data("category", key)

            var title = Fluent("div")
                .class("category-title")
                .appendTo(categoryDiv)
                .append(this.determineText(category, prefix => prefix + 'Categories.' + category));

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
        }

        return categoryDiv.getNode();
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

    private createField(container: HTMLElement, item: PropertyItem): void {

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

        var editorParams = item.editorParams;

        const editorType = (isPromiseLike(item.editorType) || typeof item.editorType === "function")
            ? item.editorType : (EditorTypeRegistry.getOrLoad(item.editorType ?? 'String'));
        let editorSpan: HTMLSpanElement;
        const index = this.editors.length;

        const then = (editorType: EditorType) => {
            if (!this.editors) {
                // destroyed?
                return;
            }

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
                element: (el: HTMLElement) => {
                    Fluent(el).addClass("editor");

                    if (Fluent.isInputLike(el))
                        el.setAttribute("name", item.name ?? "");

                    if (placeHolder)
                        el.setAttribute("placeholder", placeHolder);

                    if (editorSpan) {
                        editorSpan.replaceWith(el);
                        editorSpan = null;
                        this.editorPromises[index] = null;
                    }
                    else {
                        fieldDiv.append(el);
                    }
                }
            }).init();

            if (item.maxLength != null) {
                PropertyGrid.setMaxLength(editor, item.maxLength);
            }

            if (item.editorParams != null) {
                ReflectionOptionsSetter.set(editor, item.editorParams);
            }

            this.editors[index] = editor;
        };

        this.editors.push(null);
        this.items.push(item);
        this.editorPromises.push(null);

        if (isPromiseLike(editorType)) {
            editorSpan = document.createElement("span");
            editorSpan.className = "editor-loading-placeholder";
            fieldDiv.append(editorSpan);
            this.editorPromises.push(editorType.then(then));
        }
        else {
            then(editorType);
        }

        Fluent("div").class('vx').appendTo(fieldDiv);
        Fluent("div").class('clear').appendTo(fieldDiv);
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
            if (!!(this.get_mode() === 1 && item.defaultValue != null) &&
                typeof (source[item.name]) === 'undefined') {
                source[item.name] = item.defaultValue;
            }

            var editor = this.editors[i];
            if (!editor && this.editorPromises[i]) {
                this.editorPromises[i].then(() => {
                    this.editors && EditorUtils.loadValue(this.editors[i], item, source);
                });
            }
            else {
                EditorUtils.loadValue(editor, item, source);
            }
        }
    }

    save(target?: any): any {
        if (target == null)
            target = Object.create(null);
        for (var i = 0; i < this.editors.length; i++) {
            var item = this.items[i];
            if (item.oneWay !== true && this.canModifyItem(item)) {
                var editor = this.editors[i];
                if (!editor && this.editorPromises)
                    throw `Editor for "${this.items[i]?.name}" at index ${i} is not loaded yet.`;

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
            var readOnly = item.readOnly === true || !this.canModifyItem(item);
            var editor = this.editors[i];
            const then = (editor: Widget<any>) => {
                if (!editor)
                    return;
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
            if (!editor && this.editorPromises[i]) {
                this.editorPromises[i].then(() => {
                    if (!this.editors)
                        return;
                    then(this.editors[i]);
                });
            }
            else {
                then(editor);
            }
        }
    }

    enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void {
        for (var i = 0; i < this.editors.length; i++) {
            var item = this.items[i];
            var editor = this.editors[i];
            if (!editor && this.editorPromises[i])
                throw `Editor for "${this.items[i]?.name}" at index ${i} is not loaded yet.`;
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
    localTextPrefix?: string;
    mode?: PropertyGridMode;
}