import { Authorization, Fluent, PropertyGridTexts, addClass, appendToNode, faIcon, getType, isBS3, isPromiseLike, nsSerenity, tryGetText, type PropertyCondition, type PropertyItem } from "../../base";
import { EditorType } from "../../types/editortype";
import { EditorTypeRegistry } from "../../types/editortyperegistry";
import { EditorUtils } from "../editors/editorutils";
import { isPropertyConditionMet } from "./propertyconditions";
import { ReflectionOptionsSetter } from "./reflectionoptionssetter";
import { Widget } from "./widget";

/**
 * A field element rendered by the property grid, augmented with the editor
 * widget, its loading promise and the associated {@link PropertyItem}.
 */
export type PropertyFieldElement = HTMLElement & {
    /** The editor widget created for this field, once loaded. */
    editorWidget?: Widget<any>;
    /** A promise that resolves when the editor type finishes loading. */
    editorPromise?: PromiseLike<void>;
    /** The property item this field was rendered from. */
    propertyItem?: PropertyItem;
}

/**
 * What the conditions of a field, see {@link PropertyItem.visibleWhen}, {@link PropertyItem.readOnlyWhen} and
 * {@link PropertyItem.requiredWhen}, make of it for the current values of the other fields. They only add to what the
 * property item and the mode make of it, e.g. a field hidden by a permission stays hidden.
 */
export interface PropertyFieldConditionState {
    /** Whether the field is hidden by a condition of its own, or by one on a field that is hidden by a condition. */
    hidden?: boolean;
    /** Whether the field is read only by its conditions. */
    readOnly?: boolean;
    /** Whether the field is required by its conditions, which a hidden or read only field is not. */
    required?: boolean;
}

/**
 * Renders the caption (label) for a property field, including the required
 * marker and localized title/hint text.
 * @param props - Caption rendering props.
 * @returns The label element for the field.
 */
export function PropertyFieldCaption(props: {
    item: Pick<PropertyItem, "name" | "hint" | "labelWidth" | "required" | "title">,
    idPrefix?: string,
    localTextPrefix?: string
}): HTMLLabelElement {
    const labelWidth = props.item.labelWidth;
    const caption = determineText(props.localTextPrefix, props.item.title, p => p + props.item.name) ?? "";
    return (
        <label class="caption" for={(props.idPrefix ?? "") + props.item.name}
            title={determineText(props.localTextPrefix, props.item.hint, p => p + props.item.name + '_Hint') ?? caption}
            hidden={labelWidth == "0"}
            style={{ width: !!labelWidth ? labelWidth : null }}>
            {props.item.required && <sup title={PropertyGridTexts.RequiredHint}>*</sup>}
            {caption}
        </label>
    ) as HTMLLabelElement;
}

/**
 * Creates and initializes the editor widget for a property field, applying
 * editor params, max length, placeholder and editor addons.
 * @param props - Editor rendering props.
 */
export function PropertyFieldEditor(props: {
    fieldElement: PropertyFieldElement,
    item: Pick<PropertyItem, "editorCssClass" | "editorType" | "editorParams" | "maxLength" | "name" | "editorAddons" | "placeholder">,
    idPrefix?: string,
    localTextPrefix?: string
}): void {
    const { fieldElement, item, idPrefix, localTextPrefix } = props;

    const placeHolder = determineText(localTextPrefix, item.placeholder, p => p + item.name + '_Placeholder');

    const editorType = (isPromiseLike(item.editorType) || typeof item.editorType === "function")
        ? item.editorType : (EditorTypeRegistry.getOrLoad(item.editorType ?? 'String'));
    let loadingPoint: Comment;

    const then = (editorType: EditorType) => {

        const editor = new editorType({
            ...item.editorParams,
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
                    for (const wrapper of item.editorAddons) {
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

/**
 * Renders a line-break element when the item's form CSS class requests one at
 * the current breakpoint, or null otherwise.
 * @param props - Line-break rendering props.
 * @returns A line-break element, or null if none is needed.
 */
export function PropertyFieldLineBreak(props: {
    item: Pick<PropertyItem, "formCssClass">
}): HTMLElement {
    const klass = props?.item?.formCssClass;
    if (!klass || klass.indexOf('line-break') < 0)
        return null;

    const splitted = klass.split(' ');
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

/**
 * Renders a full property field (caption plus editor) for a property item and
 * appends it to the given container.
 * @param props - Field rendering props.
 * @returns The created field element.
 */
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

/**
 * Renders a category title with localized text.
 * @param props - Category title rendering props.
 * @returns The category title element.
 */
export function PropertyCategoryTitle(props: { category: string, localTextPrefix: string }): HTMLElement {
    return (
        <div class="category-title">
            {determineText(props.localTextPrefix, props.category, prefix => prefix + 'Categories.' + props.category)}
        </div>
    ) as HTMLElement;
}

/**
 * Renders a collapsible category container holding its child fields.
 * @param props - Category rendering props.
 * @returns The category element.
 */
export function PropertyCategory(props: { category?: string, children?: any, collapsed?: boolean, localTextPrefix?: string }): HTMLElement {

    const categoryDiv = <div class="category" /> as HTMLElement;

    const { category, children, collapsed, localTextPrefix } = props;
    if (category) {
        let key = category;
        const idx = category.lastIndexOf('.Categories.');
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

/**
 * Renders a single tab item in the property tab list.
 * @param props - Tab item rendering props.
 * @returns The tab list item element.
 */
export function PropertyTabItem(props: { title: string, active?: boolean, paneId?: string, localTextPrefix?: string }): HTMLLIElement {
    const bs3 = isBS3();
    return (
        <li role="tab" class={bs3 ? (props.active ? "active" : "") : "nav-item"}>
            <a class={bs3 ? "" : `nav-link ${props.active ? "active" : ""}`}
                data-bs-toggle="tab"
                href={!!props.paneId && `#${props.paneId}`}
                data-tabkey={extractTabKey(props.title)}>
                {determineText(props.localTextPrefix, props.title, prefix => prefix + 'Tabs.' + props.title)}
            </a>
        </li>
    ) as HTMLLIElement;
}

/**
 * Renders a single tab pane that hosts the fields of a tab.
 * @param props - Tab pane rendering props.
 * @returns The tab pane element.
 */
export function PropertyTabPane(props: { active?: boolean, id?: string, children?: any }): HTMLElement {
    return (
        <div id={props.id} class={`tab-pane fade${props.active ? (isBS3() ? " in active" : " show active") : ""}`} role="tabpanel">
            {props.children}
        </div>
    ) as HTMLElement;
}

/**
 * Renders the categories container and populates it with fields for the given
 * property items, grouping them by category.
 * @param props - Categories rendering props.
 * @returns The categories element.
 */
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
    let categoryEl: HTMLElement = null;
    let priorCategory = null;
    for (let i = 0; i < items.length; i++) {
        const item = items[i];
        const category = item.category ?? '';

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

/**
 * Renders the tab list (nav) element for the property tabs.
 * @param props - Optional children to place inside the tab list.
 * @returns The tab list element.
 */
export function PropertyTabList(props?: { children?: any }): HTMLElement {
    return (
        <ul class="nav nav-underline property-tabs" role="tablist">
            {props?.children}
        </ul>
    ) as HTMLElement;
}

/**
 * Renders the container that holds the tab panes.
 * @returns The tab panes element.
 */
export function PropertyTabPanes(_?: {}): HTMLElement {
    return <div class="tab-content property-panes" /> as HTMLElement;
}

/**
 * Renders the full tabbed layout for property items that declare a `tab`,
 * grouping items without a tab into a leading untabbed section.
 * @param props - Tabs rendering props.
 * @returns A document fragment containing the tabs, or null when a container
 *   was provided and the content was appended directly to it.
 */
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

    let tabIndex = 0;
    let i = 0;
    while (i < itemsWithTab.length) {
        const title = itemsWithTab[i].tab?.trim() ?? '';
        const withSameTab = [];

        let j = i;
        do {
            withSameTab.push(itemsWithTab[j]);
        } while (++j < itemsWithTab.length &&
            (itemsWithTab[j].tab?.trim() ?? '') === title);
        i = j;

        const paneId = (paneIdPrefix ?? idPrefix ?? "") + 'Tab' + tabIndex;

        tabList.appendChild(PropertyTabItem({ title, active: tabIndex === 0, paneId, localTextPrefix }));

        const pane = tabPanes.appendChild(PropertyTabPane({ active: tabIndex === 0, id: paneId }));
        createItems(pane, withSameTab);

        tabIndex++;
    }

    return container ? null : parentNode as DocumentFragment;
}

/**
 * A widget that renders a set of {@link PropertyItem}s as a form, organizing
 * them into categories and/or tabs, and manages loading/saving values to and
 * from the underlying editors.
 * @typeParam P - Widget props type, constrained to {@link PropertyGridOptions}.
 */
export class PropertyGrid<P extends PropertyGridOptions = PropertyGridOptions> extends Widget<P> {

    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    declare private fieldElements: PropertyFieldElement[];
    /** What the conditions made of the fields that have some, when they were last applied. */
    declare private conditionStates: Map<PropertyFieldElement, PropertyFieldConditionState>;
    declare private updatingConditions: boolean;

    /**
     * Renders the property grid contents, building categories/tabs and loading
     * the initial value.
     * @returns The rendered contents.
     */
    protected override renderContents(): any {

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

        if (this.hasConditions()) {
            // the fields are updated as the values they depend on change, which the editors tell by their events
            const update = () => this.updateConditions();
            Fluent.on(this.domNode, "change." + this.uniqueName, update);
            Fluent.on(this.domNode, "input." + this.uniqueName, update);
            // and once the editors that are still loading have their values
            const pending = this.fieldElements.filter(x => x.editorPromise).map(x => x.editorPromise);
            pending.length && Promise.all(pending).then(update);
        }

        if (this.options.value !== false) {
            this.value = this.options.value ?? {};
        }
        this.updateInterface();
    }

    /**
     * Gets whether any field has conditions on the values of the others.
     */
    protected hasConditions(): boolean {
        return !!this.fieldElements?.some(x => hasItemConditions(x.propertyItem));
    }

    /**
     * Gets the current value of a field, for the conditions of the others, which is undefined when there is no such
     * field, or its editor is not loaded yet.
     * @param name - The name of the field.
     */
    protected getConditionValue(name: string): any {
        const editor = this.fieldElements?.find(x => x.propertyItem?.name === name)?.editorWidget;
        if (!editor)
            return undefined;

        try {
            return EditorUtils.getValue(editor);
        }
        catch {
            return undefined;
        }
    }

    /**
     * Gets what the conditions make of the fields that have some, for the current values of the fields. A field hidden by
     * its own condition also hides the fields whose visibility conditions are on it, as they depend on a field that can't
     * be seen.
     * @returns The states by the field elements.
     */
    protected getConditionStates(): Map<PropertyFieldElement, PropertyFieldConditionState> {
        const states = new Map<PropertyFieldElement, PropertyFieldConditionState>();
        const elements = (this.fieldElements ?? []).filter(x => hasItemConditions(x.propertyItem));
        if (!elements.length)
            return states;

        const values = new Map<string, any>();
        const valueOf = (name: string) => {
            if (!values.has(name))
                values.set(name, this.getConditionValue(name));
            return values.get(name);
        };
        const allMet = (conditions: PropertyCondition[]) => conditions.every(x => isPropertyConditionMet(x, valueOf(x.field)));

        const conditional = elements.filter(x => x.propertyItem.visibleWhen?.length);
        const shown = new Map(conditional.map(x => [x, allMet(x.propertyItem.visibleWhen)]));
        const hidden = new Set<string>();
        // until no more fields are hidden by the ones they depend on, which a cycle of them can't keep going beyond
        for (let pass = 0; pass <= conditional.length; pass++) {
            let changed = false;
            for (const element of conditional) {
                const item = element.propertyItem;
                const isHidden = !shown.get(element) || item.visibleWhen.some(x => x.field !== item.name && hidden.has(x.field));
                if (isHidden !== hidden.has(item.name)) {
                    if (isHidden)
                        hidden.add(item.name);
                    else
                        hidden.delete(item.name);
                    changed = true;
                }
            }

            if (!changed)
                break;
        }

        for (const element of elements) {
            const item = element.propertyItem;
            states.set(element, {
                hidden: hidden.has(item.name),
                readOnly: !!item.readOnlyWhen?.length && allMet(item.readOnlyWhen),
                required: !!item.requiredWhen?.length && allMet(item.requiredWhen)
            });
        }

        return states;
    }

    /**
     * Updates the fields whose conditions changed by the current values of the fields, and the categories and tabs
     * they are in. A field whose conditions didn't change is left as it is, e.g. one a dialog made read only itself.
     * It is called as the values change, and can be called after setting the value of an editor in code, which
     * doesn't tell it.
     */
    updateConditions(): void {
        if (!this.fieldElements || this.updatingConditions)
            return;

        this.updatingConditions = true;
        try {
            const states = this.getConditionStates();
            const mode = this.get_mode();
            for (const [element, state] of states) {
                const old = this.conditionStates?.get(element);
                if (old && old.hidden === state.hidden && old.readOnly === state.readOnly && old.required === state.required)
                    continue;

                PropertyGrid.updateFieldElement(element, mode, !!this.canModifyItem(element.propertyItem), state);
            }

            this.conditionStates = states;
            this.updateConditionContainers();
        }
        finally {
            this.updatingConditions = false;
        }
    }

    /**
     * Hides the categories and the tabs whose fields are all hidden, when they have a field with a visibility condition,
     * so the others are left as they were.
     */
    protected updateConditionContainers(): void {
        const elements = this.fieldElements ?? [];
        const conditional = elements.filter(x => x.propertyItem?.visibleWhen?.length);
        if (!conditional.length)
            return;

        const isShown = (field: HTMLElement) => !field.hidden && field.style.display !== "none";
        const hasShownField = (container: Element) => elements.some(x => container.contains(x) && isShown(x));

        const categories = new Set(conditional.map(x => x.closest(".category")).filter(x => !!x && this.domNode.contains(x)));
        for (const category of categories)
            Fluent.toggle(category, hasShownField(category));

        const panes = new Set(conditional.map(x => x.closest(".tab-pane")).filter(x => !!x && this.domNode.contains(x)));
        for (const pane of panes) {
            const tab = pane.id ? this.domNode.querySelector(`a[href="#${pane.id}"]`)?.closest("li") : null;
            if (!tab)
                continue;

            const shown = hasShownField(pane);
            Fluent.toggle(tab, shown);
            // the tab that was open is hidden, so another one is opened
            if (!shown && pane.classList.contains("active")) {
                const other = Array.from(this.domNode.querySelectorAll<HTMLElement>(".property-tabs > li"))
                    .find(x => !x.hidden)?.querySelector<HTMLElement>("a");
                other?.click();
            }
        }
    }

    /**
     * Destroys all field editors and clears the grid contents.
     */
    override destroy() {

        Fluent.off(this.domNode, "." + this.uniqueName);
        this.conditionStates = null;

        if (this.fieldElements) {
            for (const fieldElement of this.fieldElements) {
                if (fieldElement) {
                    fieldElement.editorWidget?.destroy();
                    delete fieldElement.editorWidget;
                    delete fieldElement.editorPromise;
                    delete fieldElement.propertyItem;
                }
            }
            this.fieldElements = null;
        }

        Fluent.empty(this.domNode);

        super.destroy();
    }

    /**
     * Returns the editor widgets for all rendered fields.
     * @returns Array of editor widgets.
     */
    get_editors(): Widget<any>[] {
        return this.fieldElements?.map(x => x.editorWidget) ?? [];
    }

    /**
     * Returns the property items for all rendered fields.
     * @returns Array of property items.
     */
    get_items(): PropertyItem[] {
        return this.fieldElements?.map(x => x.propertyItem) ?? [];
    }

    /**
     * Returns the id prefix used by this grid.
     * @returns The id prefix.
     */
    get_idPrefix(): string {
        return this.idPrefix;
    }

    /**
     * Invokes a callback for each rendered field with its property item and
     * editor widget.
     * @param callback - Callback receiving the property item and editor widget.
     */
    enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void {
        for (const fieldElement of this.fieldElements) {
            const item = fieldElement.propertyItem;
            const editor = fieldElement.editorWidget;
            if (!editor && fieldElement.editorPromise)
                throw new Error(`Editor for "${item.name}" is not loaded yet.`);
            callback(item, editor);
        }
    }

    /**
     * Returns the current grid mode (insert or update).
     * @returns The current {@link PropertyGridMode}.
     */
    get_mode(): PropertyGridMode {
        return this.options.mode;
    }

    /**
     * Sets the grid mode and refreshes the interface.
     * @param value - The new {@link PropertyGridMode}.
     */
    set_mode(value: PropertyGridMode) {
        if (this.options.mode !== value) {
            this.options.mode = value;
            this.updateInterface();
        }
    }

    /**
     * Loads a field's value from a source object into its editor, applying
     * defaults in insert mode.
     * @param source - The source object to read values from.
     * @param fieldElement - The field element whose editor receives the value.
     * @param mode - The grid mode, used to apply insert defaults.
     */
    static loadFieldValue(source: any, fieldElement: PropertyFieldElement, mode?: PropertyGridMode) {
        const item = fieldElement.propertyItem;
        if (!!(mode === PropertyGridMode.insert && item.defaultValue != null) &&
            typeof (source[item.name]) === 'undefined') {
            source[item.name] = item.defaultValue;
        }

        if (item.unbound ?? item.skipOnLoad)
            return;

        const editor = fieldElement.editorWidget;
        if (!editor && fieldElement.editorPromise) {
            fieldElement.editorPromise.then(() => {
                fieldElement.editorWidget && EditorUtils.loadValue(fieldElement.editorWidget, item, source);
            });
        }
        else {
            EditorUtils.loadValue(editor, item, source);
        }
    }

    /**
     * Loads values from a source object into all field editors.
     * @param source - The source object to read values from.
     */
    load(source: any): void {
        const mode = this.get_mode();
        for (const fieldElement of this.fieldElements) {
            PropertyGrid.loadFieldValue(source, fieldElement, mode);
        }

        // the values the conditions depend on may have changed, without the events of the editors
        if (this.hasConditions())
            this.updateConditions();
    }

    /**
     * Saves a field's editor value into a target object when the item is
     * modifiable.
     * @param target - The target object to write values into.
     * @param fieldElement - The field element whose editor value is saved.
     * @param canModify - Whether the item may be modified; defaults to the
     *   result of {@link PropertyGrid.canModifyItem}.
     */
    static saveFieldValue(target: any, fieldElement: PropertyFieldElement, canModify?: boolean): void {
        const item = fieldElement.propertyItem;
        if ((item.unbound ?? item.skipOnSave ?? (item as any).oneWay) !== true && (canModify ?? PropertyGrid.canModifyItem(item))) {
            const editor = fieldElement.editorWidget;
            if (!editor && fieldElement.editorPromise)
                throw new Error(`Editor for "${item.name}" is not loaded yet.`);

            EditorUtils.saveValue(editor, item, target);
        }
    }

    /**
     * Saves all field editor values into a target object.
     * @param target - Optional target object; a new object is created if omitted.
     * @returns The object containing the saved values.
     */
    save(target?: any): any {
        if (target == null)
            target = Object.create(null);
        // the values of the fields that are cleared while they are hidden, which are checked again, as the values they
        // depend on may have been set in code
        const states = this.fieldElements.some(x => x.propertyItem?.clearWhenHidden) ? this.getConditionStates() : null;
        for (const fieldElement of this.fieldElements) {
            const item = fieldElement.propertyItem;
            const canModify = !!this.canModifyItem(item);
            if (item.clearWhenHidden && states?.get(fieldElement)?.hidden) {
                if (canModify && (item.unbound ?? item.skipOnSave ?? (item as any).oneWay) !== true)
                    target[item.name] = null;
                continue;
            }

            PropertyGrid.saveFieldValue(target, fieldElement, canModify);
        }
        return target;
    }

    /**
     * Commits pending edits on all editors that support it.
     * @returns True if all commits succeeded, false if any editor rejected.
     */
    async commitEdits(): Promise<boolean> {
        for (const fieldElement of this.fieldElements) {
            if (fieldElement.editorWidget &&
                typeof (fieldElement.editorWidget as any).commitEdits === "function") {
                const result = await (fieldElement.editorWidget as any).commitEdits();
                if (result === false)
                    return false;
            }
        }
        return true;
    }

    /**
     * Gets the current values of all editors as an object.
     */
    public get value(): any {
        return this.save();
    }

    /**
     * Loads values from an object into all editors.
     * @param val - The object containing values to load.
     */
    public set value(val: any) {
        if (val == null)
            val = Object.create(null);
        this.load(val);
    }

    /**
     * Determines whether a property item may be modified in the given mode,
     * taking insert/update permissions into account.
     * @param item - The property item to check.
     * @param mode - The grid mode; defaults to update semantics when omitted.
     * @returns True if the item can be modified.
     */
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

    /**
     * Determines whether a property item may be modified in the current mode.
     * @param item - The property item to check.
     * @returns True if the item can be modified.
     */
    protected canModifyItem(item: PropertyItem) {
        return PropertyGrid.canModifyItem(item, this.get_mode());
    }

    /**
     * Updates a field element's editor read-only/required state and visibility
     * based on the item and mode.
     * @param fieldElement - The field element to update.
     * @param mode - The grid mode.
     * @param canModify - Whether the item may be modified.
     * @param conditions - What the conditions of the field make of it, which only add to the rest, e.g. a field hidden
     *   by a permission stays hidden.
     */
    static updateFieldElement(fieldElement: PropertyFieldElement, mode?: PropertyGridMode, canModify?: boolean,
        conditions?: PropertyFieldConditionState) {
        const item = fieldElement.propertyItem;
        canModify ??= PropertyGrid.canModifyItem(item, mode);
        const readOnly = item.readOnly === true || !canModify || !!conditions?.readOnly;
        const editor = fieldElement.editorWidget;
        const then = (editor: Widget<any>) => {
            if (!editor)
                return;
            EditorUtils.setReadOnly(editor, readOnly);
            EditorUtils.setRequired(editor, !readOnly && !conditions?.hidden &&
                (!!item.required || !!conditions?.required) && item.editorType !== 'Boolean');
            if (item.visible === false || item.readPermission != null ||
                item.insertPermission != null || item.updatePermission != null ||
                item.hideOnInsert === true || item.hideOnUpdate === true || conditions?.hidden != null) {
                const hidden = (item.readPermission != null &&
                    !Authorization.hasPermission(item.readPermission)) ||
                    item.visible === false ||
                    (mode === PropertyGridMode.insert && item.hideOnInsert === true) ||
                    (mode === PropertyGridMode.update && item.hideOnUpdate === true) ||
                    !!conditions?.hidden;

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

    /**
     * Updates a single field element in the current mode.
     * @param fieldElement - The field element to update.
     */
    protected updateFieldElement(fieldElement: PropertyFieldElement) {
        PropertyGrid.updateFieldElement(fieldElement, this.get_mode(), !!this.canModifyItem(fieldElement.propertyItem),
            this.conditionStates?.get(fieldElement));
    }

    /**
     * Refreshes the read-only/required state and visibility of all fields, including what their conditions make of them.
     */
    updateInterface() {
        this.conditionStates = this.getConditionStates();
        for (const fieldElement of this.fieldElements) {
            this.updateFieldElement(fieldElement);
        }
        this.updateConditionContainers();
    }
}

/** Gets whether a property item has conditions on the values of other fields. */
function hasItemConditions(item: PropertyItem): boolean {
    return !!(item?.visibleWhen?.length || item?.readOnlyWhen?.length || item?.requiredWhen?.length);
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

    const idx = title.lastIndexOf('.Tabs.');
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
    return <div class={klass} style={{ width: "100%" }} /> as HTMLElement;
}

/**
 * Determines the editing mode of a {@link PropertyGrid}, which affects how
 * defaults, permissions and visibility are applied to fields.
 */
export enum PropertyGridMode {
    /** The grid is used for inserting a new record. */
    insert = 1,
    /** The grid is used for updating an existing record. */
    update = 2
}

/**
 * Options for configuring a {@link PropertyGrid}.
 */
export interface PropertyGridOptions {
    /** Optional id prefix used for field element ids. */
    idPrefix?: string;
    /** The property items to render as fields. */
    items: PropertyItem[];
    /** Optional local text prefix used to localize captions and hints. */
    localTextPrefix?: string;
    /** Optional initial value to load into the editors. */
    value?: any;
    /** The grid mode; defaults to {@link PropertyGridMode.insert}. */
    mode?: PropertyGridMode;
}