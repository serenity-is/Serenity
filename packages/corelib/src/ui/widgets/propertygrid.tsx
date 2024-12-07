import { Authorization, Fluent, addClass, appendToNode, faIcon, getCustomAttribute, getType, isBS3, isPromiseLike, localText, tryGetText, type PropertyItem } from "../../base";
import { extend } from "../../compat";
import { OptionsTypeAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { EditorType } from "../../types/editortype";
import { EditorTypeRegistry } from "../../types/editortyperegistry";
import { EditorUtils } from "../editors/editorutils";
import { ReflectionOptionsSetter } from "./reflectionoptionssetter";
import { Widget } from "./widget";

export type PropertyFieldElement = HTMLElement & {
    editorWidget?: Widget<any>;
    editorPromise?: PromiseLike<void>;
    propertyItem?: PropertyItem;
}

export function PropertyFieldCaption(props: {
    item: Pick<PropertyItem, "name" | "hint" | "labelWidth" | "required" | "title">,
    idPrefix?: string,
    localTextPrefix?: string
}): HTMLLabelElement {
    const labelWidth = props.item.labelWidth;
    const caption = determineText(props.localTextPrefix, props.item.title, p => p + props.item.name) ?? "";
    return (
        <label className="caption" htmlFor={(props.idPrefix ?? "") + props.item.name}
            title={determineText(props.localTextPrefix, props.item.hint, p => p + props.item.name + '_Hint') ?? caption}
            style={!!labelWidth && (labelWidth == "0" ? "display: none" : ("width: " + labelWidth))}>
            {props.item.required && <sup title={localText('Controls.PropertyGrid.RequiredHint')}>*</sup>}
            {caption}
        </label>
    ) as HTMLLabelElement;
}

export function PropertyFieldEditor(props: {
    fieldElement: PropertyFieldElement,
    item: Pick<PropertyItem, "editorCssClass" | "editorType" | "editorParams" | "maxLength" | "name" | "editorAddons" | "placeholder">,
    idPrefix?: string,
    localTextPrefix?: string
}): void {
    const { fieldElement, item, idPrefix, localTextPrefix } = props;

    const placeHolder = determineText(localTextPrefix, item.placeholder, p => p + item.name + '_Placeholder');
    let editorParams = item.editorParams;

    const editorType = (isPromiseLike(item.editorType) || typeof item.editorType === "function")
        ? item.editorType : (EditorTypeRegistry.getOrLoad(item.editorType ?? 'String'));
    let loadingPoint: Comment;

    const then = (editorType: EditorType) => {
        let optionsType = null;
        const optionsAttr = getCustomAttribute(editorType, OptionsTypeAttribute);
        if (optionsAttr) {
            optionsType = optionsAttr.value as any;
        }
        if (optionsType != null) {
            editorParams = extend(new optionsType(), item.editorParams);
        }
        else {
            editorParams = extend(new Object(), item.editorParams);
        }

        const editor = new editorType({
            ...editorParams,
            id: idPrefix + item.name,
            element: (el: HTMLElement) => {
                !el.id && (el.id = idPrefix + item.name);
                el.classList.add("editor");

                if (item.editorCssClass)
                    addClass(el, item.editorCssClass);

                if (Fluent.isInputLike(el))
                    el.setAttribute("name", item.name ?? "");

                if (placeHolder)
                    el.setAttribute("placeholder", placeHolder);

                let wrappedNode: Node = el;
                if (item.editorAddons?.length) {
                    wrappedNode = document.createDocumentFragment();
                    wrappedNode.appendChild(el);
                    for (var wrapper of item.editorAddons) {
                        const wrapperComponent = (typeof wrapper.type === "function" ? wrapper.type : getType(wrapper.type)) as (props: any) => void;
                        if (typeof wrapperComponent !== "function")
                            throw `Invalid editor addon type: ${wrapper.type} for property: "${item.name}"`;
                        wrapperComponent({ ...wrapper.params, propertyItem: item, editorElement: el, documentFragment: wrappedNode });
                    }
                }

                if (loadingPoint) {
                    loadingPoint.parentElement?.replaceChild(wrappedNode, loadingPoint);
                    loadingPoint = null;
                    delete fieldElement.editorPromise;
                }
                else {
                    fieldElement.append(wrappedNode);
                }
            }
        }).init();

        if (item.maxLength != null) {
            setMaxLength(editor, item.maxLength);
        }

        if (item.editorParams != null) {
            ReflectionOptionsSetter.set(editor, item.editorParams);
        }

        fieldElement.editorWidget = editor;
    };

    if (isPromiseLike(editorType)) {
        loadingPoint = document.createComment("Loading editor type...");
        fieldElement.append(loadingPoint);
        fieldElement.editorPromise = editorType.then(then);
    }
    else {
        then(editorType);
    }
}

export function PropertyFieldLineBreak(props: {
    item: Pick<PropertyItem, "formCssClass">
}): HTMLElement {
    const klass = props?.item?.formCssClass;
    if (!klass || klass.indexOf('line-break') < 0)
        return null;

    var splitted = klass.split(' ');
    if (splitted.indexOf('line-break-xs') >= 0) {
        return createLineBreak("line-break");
    }
    if (splitted.indexOf('line-break-sm') >= 0) {
        return createLineBreak("line-break hidden-xs");
    }
    else if (splitted.indexOf('line-break-md') >= 0) {
        return createLineBreak("line-break hidden-sm");
    }
    else if (splitted.indexOf('line-break-lg') >= 0) {
        return createLineBreak("line-break hidden-md");
    }
}

export function PropertyField(props: {
    item: PropertyItem,
    container?: ParentNode,
    idPrefix?: string,
    localTextPrefix?: string
}): PropertyFieldElement {

    const { item, container, localTextPrefix } = props;
    const idPrefix = props?.idPrefix ?? "";

    const fieldElement = (
        <div class={["field", item.name, item.cssClass, item.formCssClass]} data-itemname={item.name}>
            <PropertyFieldCaption item={item} idPrefix={idPrefix} localTextPrefix={localTextPrefix} />
        </div>
    ) as PropertyFieldElement;
    fieldElement.propertyItem = item;

    if (item.formCssClass && container) {
        const lineBreak = PropertyFieldLineBreak({ item });
        lineBreak && container.appendChild(lineBreak);
    }

    container?.appendChild(fieldElement); // editor might expect to be in the DOM for cascade links etc.

    PropertyFieldEditor({
        fieldElement,
        item,
        idPrefix,
        localTextPrefix
    });

    fieldElement.appendChild(<div class="vx" />);
    fieldElement.appendChild(<div class="clear" />);

    return fieldElement;
}

export function PropertyCategoryTitle(props: { category: string, localTextPrefix: string }): HTMLElement {
    return (
        <div className="category-title">
            {determineText(props.localTextPrefix, props.category, prefix => prefix + 'Categories.' + props.category)}
        </div>
    ) as HTMLElement;
}

export function PropertyCategory(props: { category?: string, children?: any, collapsed?: boolean, localTextPrefix?: string }): HTMLElement {

    const categoryDiv = <div class="category" /> as HTMLElement;

    const { category, children, collapsed, localTextPrefix } = props;
    if (category) {
        let key = category;
        let idx = category.lastIndexOf('.Categories.');
        if (idx >= 0) {
            key = category.substring(idx + 12);
        }
        categoryDiv.dataset.category = key;

        const title = categoryDiv.appendChild(<PropertyCategoryTitle category={category} localTextPrefix={localTextPrefix} />) as HTMLElement;

        if (collapsed != null) {
            categoryDiv.classList.add("collapsible");
            collapsed && categoryDiv.classList.add("collapsed");

            const icon = title.appendChild(<i class={faIcon(collapsed ? "plus" : "minus")} />) as HTMLElement;

            title.addEventListener("click", function () {
                categoryDiv.classList.toggle('collapsed');
                icon.classList.toggle('fa-plus');
                icon.classList.toggle('fa-minus');
            });
        }
    }

    appendToNode(categoryDiv, children);

    return categoryDiv;
}

export function PropertyTabItem(props: { title: string, active?: boolean, paneId?: string, localTextPrefix?: string }): HTMLLIElement {
    const bs3 = isBS3();
    return (
        <li role="tab" className={bs3 ? (props.active ? "active" : "") : "nav-item"}>
            <a className={bs3 ? "" : `nav-link ${props.active ? "active" : ""}`}
                data-bs-toggle="tab"
                href={!!props.paneId && `#${props.paneId}`}
                data-tabkey={extractTabKey(props.title)}>
                {determineText(props.localTextPrefix, props.title, prefix => prefix + 'Tabs.' + props.title)}
            </a>
        </li>
    ) as HTMLLIElement;
}

export function PropertyTabPane(props: { active?: boolean, id?: string, children?: any }): HTMLElement {
    return (
        <div id={props.id} className={`tab-pane fade${props.active ? (isBS3() ? " in active" : " show active") : ""}`} role="tabpanel">
            {props.children}
        </div>
    ) as HTMLElement;
}

export function PropertyCategories(props: {
    items: PropertyItem[],
    container?: ParentNode,
    fieldElements?: PropertyFieldElement[],
    idPrefix?: string,
    localTextPrefix?: string
}): HTMLElement {
    const categoriesDiv = <div class="categories" /> as HTMLElement;
    props.container && props.container.appendChild(categoriesDiv);

    const { items, fieldElements, idPrefix, localTextPrefix } = props;
    var categoryEl: HTMLElement = null;
    var priorCategory = null;
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        var category = item.category ?? '';

        if (!categoryEl || priorCategory !== category) {
            categoryEl = categoriesDiv.appendChild(<PropertyCategory category={category} localTextPrefix={localTextPrefix}
                collapsed={(item.collapsible !== true) ? null : item.collapsed ?? false} />) as HTMLElement;
        }

        const fieldElement = <PropertyField item={item} container={categoryEl} idPrefix={idPrefix} localTextPrefix={localTextPrefix} /> as PropertyFieldElement;
        fieldElements?.push(fieldElement);   
        priorCategory = category;
    }


    return categoriesDiv;
}

export function PropertyTabList(props?: { children?: any }): HTMLElement {
    return (
        <ul className="nav nav-underline property-tabs" role="tablist">
            {props?.children}
        </ul>
    ) as HTMLElement;
}

export function PropertyTabPanes(_?: {}): HTMLElement {
    return <div class="tab-content property-panes" /> as HTMLElement;
}

export function PropertyTabs(props: {
    items: PropertyItem[],
    container?: ParentNode,
    fieldElements?: PropertyFieldElement[],
    idPrefix?: string,
    localTextPrefix?: string,
    paneIdPrefix?: string
}): DocumentFragment | null {

    const { items, container, fieldElements, idPrefix, localTextPrefix, paneIdPrefix } = props;

    const parentNode = container ?? document.createDocumentFragment();

    const createItems = (container: ParentNode, items: PropertyItem[]) => PropertyCategories({
        items,
        container,
        fieldElements,
        idPrefix,
        localTextPrefix
    });

    const itemsWithoutTab = items.filter(f => !f.tab);
    if (itemsWithoutTab.length > 0) {
        createItems(parentNode, itemsWithoutTab);
        parentNode.appendChild(<div class="pad" />);
    }

    const itemsWithTab = items.filter(f => f.tab);
    const tabList = parentNode.appendChild(PropertyTabList());
    const tabPanes = parentNode.appendChild(PropertyTabPanes());

    var tabIndex = 0;
    var i = 0;
    while (i < itemsWithTab.length) {
        var title = itemsWithTab[i].tab?.trim() ?? '';
        var withSameTab = [];

        var j = i;
        do {
            withSameTab.push(itemsWithTab[j]);
        } while (++j < itemsWithTab.length &&
            (itemsWithTab[j].tab?.trim() ?? '') === title);
        i = j;

        var paneId = (paneIdPrefix ?? idPrefix ?? "") + 'Tab' + tabIndex;

        tabList.appendChild(PropertyTabItem({ title, active: tabIndex === 0, paneId, localTextPrefix }));

        const pane = tabPanes.appendChild(PropertyTabPane({ active: tabIndex === 0, id: paneId }));
        createItems(pane, withSameTab);

        tabIndex++;
    }

    return container ? null : parentNode as DocumentFragment;
}

@Decorators.registerClass('Serenity.PropertyGrid')
export class PropertyGrid<P extends PropertyGridOptions = PropertyGridOptions> extends Widget<P> {

    declare private fieldElements: PropertyFieldElement[];

    protected renderContents(): any {

        this.domNode.classList.add('s-PropertyGrid');
        this.options.mode ??= PropertyGridMode.insert;
        this.fieldElements = [];

        const items = this.options.items || [];

        const commonProps = {
            items,
            container: this.domNode,
            fieldElements: this.fieldElements,
            idPrefix: this.idPrefix,
            localTextPrefix: this.options.localTextPrefix
        }

        if (items.some(x => !!x.tab)) {
            PropertyTabs({ ...commonProps, paneIdPrefix: this.uniqueName + '_' });
        }
        else {
            PropertyCategories(commonProps);
        }

        if (this.options.value !== false) {
            this.value = this.options.value ?? {};
        }
        this.updateInterface();
    }

    destroy() {

        if (this.fieldElements) {
            for (var fieldElement of this.fieldElements) {
                if (fieldElement) {
                    fieldElement.editorWidget?.destroy();
                    delete fieldElement.editorWidget;
                    delete fieldElement.editorPromise;
                    delete fieldElement.propertyItem;
                }
            }
            this.fieldElements = null;
        }

        super.destroy();
    }

    get_editors(): Widget<any>[] {
        return this.fieldElements?.map(x => x.editorWidget) ?? [];
    }

    get_items(): PropertyItem[] {
        return this.fieldElements?.map(x => x.propertyItem) ?? [];
    }

    get_idPrefix(): string {
        return this.idPrefix;
    }

    enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void {
        for (let fieldElement of this.fieldElements) {
            var item = fieldElement.propertyItem;
            var editor = fieldElement.editorWidget;
            if (!editor && fieldElement.editorPromise)
                throw `Editor for "${item.name}" is not loaded yet.`;
            callback(item, editor);
        }
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

    static loadFieldValue(source: any, fieldElement: PropertyFieldElement, mode?: PropertyGridMode) {
        var item = fieldElement.propertyItem;
        if (!!(mode === PropertyGridMode.insert && item.defaultValue != null) &&
            typeof (source[item.name]) === 'undefined') {
            source[item.name] = item.defaultValue;
        }

        var editor = fieldElement.editorWidget;
        if (!editor && fieldElement.editorPromise) {
            fieldElement.editorPromise.then(() => {
                fieldElement.editorWidget && EditorUtils.loadValue(fieldElement.editorWidget, item, source);
            });
        }
        else {
            EditorUtils.loadValue(editor, item, source);
        }
    }

    load(source: any): void {
        const mode = this.get_mode();
        for (let fieldElement of this.fieldElements) {
            PropertyGrid.loadFieldValue(source, fieldElement, mode);
        }
    }

    static saveFieldValue(target: any, fieldElement: PropertyFieldElement, canModify?: boolean): void {
        var item = fieldElement.propertyItem;
        if (item.oneWay !== true && (canModify ?? PropertyGrid.canModifyItem(item))) {
            var editor = fieldElement.editorWidget;
            if (!editor && fieldElement.editorPromise)
                throw `Editor for "${item.name}" is not loaded yet.`;

            EditorUtils.saveValue(editor, item, target);
        }
    }

    save(target?: any): any {
        if (target == null)
            target = Object.create(null);
        for (let fieldElement of this.fieldElements) {
            PropertyGrid.saveFieldValue(target, fieldElement, !!this.canModifyItem(fieldElement.propertyItem));
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

    static canModifyItem(item: PropertyItem, mode?: PropertyGridMode) {
        if (mode === PropertyGridMode.insert) {
            if (item.insertable === false) {
                return false;
            }

            if (item.insertPermission == null) {
                return true;
            }

            return Authorization.hasPermission(item.insertPermission);
        }
        else if (mode === PropertyGridMode.update) {
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

    protected canModifyItem(item: PropertyItem) {
        return PropertyGrid.canModifyItem(item, this.get_mode());
    }

    static updateFieldElement(fieldElement: PropertyFieldElement, mode?: PropertyGridMode, canModify?: boolean) {
        var item = fieldElement.propertyItem;
        canModify ??= PropertyGrid.canModifyItem(item, mode);
        var readOnly = item.readOnly === true || !canModify;
        var editor = fieldElement.editorWidget;
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
                    (mode === PropertyGridMode.insert && item.hideOnInsert === true) ||
                    (mode === PropertyGridMode.update && item.hideOnUpdate === true);

                editor.getGridField().toggle(!hidden);
            }
        }
        if (!editor && fieldElement.editorPromise) {
            fieldElement.editorPromise.then(() => {
                fieldElement.editorWidget && then(fieldElement.editorWidget);
            });
        }
        else {
            then(editor);
        }
    }

    protected updateFieldElement(fieldElement: PropertyFieldElement) {
        PropertyGrid.updateFieldElement(fieldElement, this.get_mode(), !!this.canModifyItem(fieldElement.propertyItem));
    }

    updateInterface() {
        for (let fieldElement of this.fieldElements) {
            this.updateFieldElement(fieldElement);
        }
    }
}

function determineText(localTextPrefix: string, text: string, getKey: (s: string) => string) {
    let local: string;
    if (text != null && !text.startsWith('`')) {
        local = tryGetText(text);
        if (local != null) {
            return local;
        }
    }

    if (text != null && text.startsWith('`')) {
        text = text.substring(1);
    }

    if (localTextPrefix) {
        local = tryGetText(getKey(localTextPrefix));
        if (local != null) {
            return local;
        }
    }

    return text;
}

function extractTabKey(title: string) {
    if (!title)
        return null;

    let idx = title.lastIndexOf('.Tabs.');
    if (idx >= 0) {
        return title.substring(idx + 6);
    }

    return title;
}

function setMaxLength(widget: Widget<any>, maxLength: number) {
    if (Fluent.isInputLike(widget.domNode)) {
        if (maxLength > 0) {
            widget.domNode.setAttribute('maxlength', (maxLength ?? 0).toString());
        }
        else {
            widget.domNode.removeAttribute('maxlength');
        }
    }
}

function createLineBreak(klass: string): HTMLElement {
    return <div className={klass} style="width: 100%" /> as HTMLElement;
}

export enum PropertyGridMode {
    insert = 1,
    update = 2
}

export interface PropertyGridOptions {
    idPrefix?: string;
    items: PropertyItem[];
    localTextPrefix?: string;
    value?: any;
    mode?: PropertyGridMode;
}