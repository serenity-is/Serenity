import { addDisposingListener, bindThis, removeDisposingListener } from "@serenity-is/domwise";
import { ClassTypeInfo, Config, EditorTypeInfo, Fluent, StringLiteral, addClass, addValidationRule, appendToNode, classTypeInfo, editorTypeInfo, getCustomAttribute, getInstanceType, getTypeFullName, getTypeShortName, htmlEncode, isArrayLike, nsSerenity, registerType, toggleClass, type AttributeSpecifier, type CustomAttribute, type InterfaceType } from "../../base";
import { ensureParentOrFragment, handleElementProp, isFragmentWorkaround, setElementProps } from "./widgetinternal";
import { IdPrefixType, associateWidget, deassociateWidget, getWidgetName, useIdPrefix, type WidgetProps } from "./widgetutils";
export { getWidgetFrom, tryGetWidget, useIdPrefix, type IdPrefixType, type WidgetProps } from "./widgetutils";

const afterRenderSymbol = Symbol();

/**
 * The base class for all Serenity widgets. A widget wraps a DOM node, manages
 * its lifecycle (create/destroy), associates itself with its element for later
 * lookup, and provides helpers for id prefixes, validation and rendering.
 * @typeParam P - The widget's options/props type.
 */
export class Widget<P = {}> {
    private static nextWidgetNumber = 0;
    /** The widget's options/props. */
    declare protected readonly options: WidgetProps<P>;
    /** A unique name for this widget instance, used for event namespacing. */
    declare public readonly uniqueName: string;
    /** The id prefix used for this widget's child element ids. */
    declare public readonly idPrefix: string;
    /** The DOM node this widget is bound to. */
    declare public readonly domNode: HTMLElement;

    /**
     * Creates a widget bound to the given props, resolving the DOM node,
     * associating the widget with it and rendering its contents.
     * @param props - The widget props, including the target element.
     */
    constructor(props: WidgetProps<P>) {
        if (isArrayLike(props)) {
            this.domNode = ensureParentOrFragment(props[0]);
            this.options = {} as any;
        }
        else {
            this.options = props ?? {} as any;
            this.domNode = handleElementProp(getInstanceType(this), this.options);
        }

        delete this.options.element;
        setElementProps(this, this.props as any);

        this.uniqueName = getWidgetName(getInstanceType(this)) + (Widget.nextWidgetNumber++).toString();

        associateWidget(this);

        addDisposingListener(this.domNode, bindThis(this).destroy, this.uniqueName);

        this.idPrefix = (this.options as any)?.idPrefix ?? (this.uniqueName + '_');

        (this as any)[afterRenderSymbol] = [];
        this.addCssClass();
        !this.deferRender() && this.internalRenderContents();
    }

    /**
     * Destroys the widget, removing its association with the DOM node, its CSS
     * classes and its event handlers.
     */
    public destroy(): void {
        if (this.domNode) {
            removeDisposingListener(this.domNode, bindThis(this).destroy);
            deassociateWidget(this);
            toggleClass(this.domNode, this.getCssClass(), false);
            Fluent.off(this.domNode, '.' + this.uniqueName);
            delete (this as any).domNode;
        }
    }

    /**
     * Creates the default DOM element for a widget.
     * @returns A new `div` element.
     */
    static createDefaultElement(): HTMLElement {
        return document.createElement("div");
    }

    /**
     * Returns a Fluent(this.domNode) object
     */
    public get element(): Fluent {
        return Fluent(this.domNode);
    }

    /**
     * Adds the widget's CSS class to its DOM node.
     */
    protected addCssClass(): void {
        addClass(this.domNode, this.getCssClass());
    }

    /**
     * Determines whether rendering should be deferred until {@link init} is
     * called.
     * @returns True to defer rendering.
     */
    protected deferRender() {
        return false;
    }

    /**
     * Returns the CSS class(es) applied to the widget's DOM node.
     * @returns The space-separated CSS class string.
     */
    protected getCssClass(): string {
        var type = getInstanceType(this);
        var classList: string[] = [];
        var fullClass = getTypeFullName(type).replace(/\./g, '-');
        classList.push(fullClass);

        for (let k of Config.rootNamespaces) {
            if (fullClass.startsWith(k + '-')) {
                classList.push(fullClass.substring(k.length + 1));
                break;
            }
        }

        classList.push(getTypeShortName(type));
        return classList
            .filter((v, i, a) => a.indexOf(v) === i)
            .map(s => 's-' + s)
            .join(" ");
    }

    /**
     * Returns the widget name for a type, used for association and unique names.
     * @param type - The widget type.
     * @returns The widget name.
     */
    public static getWidgetName(type: Function): string {
        return getWidgetName(type);
    }

    /**
     * Adds a validation rule to the widget's DOM node.
     * @param rule - The validation rule function, or a unique name when the
     *   two-argument overload is used.
     * @param uniqueName - A unique name for the rule, or the rule function when
     *   the two-argument overload is used.
     */
    public addValidationRule(rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string, uniqueName?: string): void;
    public addValidationRule(uniqueName: string, rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string): void;
    public addValidationRule(rule: any, uniqueName: any): void {
        addValidationRule(this.domNode, typeof rule === "function" ? rule : uniqueName,
            typeof rule === "function" ? uniqueName ?? this.uniqueName : rule);
    }

    /**
     * Finds a child element by its prefix-relative id.
     * @param id - The id relative to the widget's id prefix.
     * @returns A {@link Fluent} wrapper for the matching element.
     */
    protected byId<TElement extends HTMLElement = HTMLElement>(id: string): Fluent<TElement> {
        return this.element.findFirst<TElement>('#' + this.idPrefix + id);
    }

    /**
     * Finds a child element by its prefix-relative id.
     * @param id - The id relative to the widget's id prefix.
     * @returns The matching element, or null if not found.
     */
    protected findById<TElement extends HTMLElement = HTMLElement>(id: string): TElement {
        return this.domNode?.querySelector<TElement>('#' + this.idPrefix + id);
    }

    /**
     * Returns the closest `.field` element containing the widget's DOM node.
     * @returns A {@link Fluent} wrapper for the grid field.
     */
    public getGridField(): Fluent {
        return Fluent(this.domNode.closest('.field'));
    }

    /**
     * Registers a `change` handler on the widget's DOM node.
     * @param handler - The change event handler.
     */
    public change(handler: (e: Event) => void) {
        Fluent.on(this.domNode, "change." + this.uniqueName, handler);
    };

    /**
     * Registers a `change` handler that ignores changes originating from
     * combobox setting values.
     * @param handler - The change event handler.
     */
    public changeSelect2(handler: (e: Event) => void) {
        Fluent.on(this.domNode, "change." + this.uniqueName, e => {
            if ((e.target as HTMLElement)?.dataset?.comboboxsettingvalue)
                return;
            handler(e);
        });
    }

    /**
     * Creates a widget instance from the given params, appending its element to
     * the container and invoking the init/init callbacks.
     * @param params - The widget creation params.
     * @returns The created widget instance.
     */
    public static create<TWidget extends Widget<P>, P>(params: CreateWidgetParams<TWidget, P>) {
        let props: WidgetProps<P> = params.options ?? ({} as any);
        let node = handleElementProp(params.type as any, props);
        params.container && (isArrayLike(params.container) ? params.container[0] : params.container)?.appendChild(node);
        params.element?.(Fluent(node));
        props.element = node;
        let widget = new params.type(props as any);
        widget.init();
        params.init?.(widget);
        return widget;
    }

    /**
     * Returns a custom attribute applied to the widget's type.
     * @param attrType - The attribute type to look up.
     * @param inherit - Whether to search inherited types; defaults to true.
     * @returns The matching attribute, or null.
     */
    protected getCustomAttribute<TAttr extends CustomAttribute>(attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): TAttr {
        return getCustomAttribute(getInstanceType(this), attrType, inherit);
    }

    /**
     * Queues a callback to run after the widget's contents are rendered.
     * @param callback - The callback to run after rendering.
     */
    protected afterRender(callback: () => void) {
        if (!callback)
            return;

        const queue = (this as any)[afterRenderSymbol];
        if (!queue)
            callback();
        else
            queue.push(callback);
    }

    /**
     * Initializes the widget, rendering its contents if rendering was deferred.
     * @returns This widget instance.
     */
    public init(): this {
        this.deferRender() && this.internalRenderContents();
        return this;
    }

    /**
     * Returns the main element for this widget or the document fragment.
     * As widgets may get their elements from props unlike regular JSX widgets, 
     * this method should not be overridden. Override renderContents() instead.
     */
    public render(): any {
        let el = this.init().domNode;
        let parent = el?.parentNode;
        if (parent instanceof DocumentFragment &&
            parent.childNodes.length > 1 &&
            (parent as any)[isFragmentWorkaround])
            return parent;
        return el;
    }

    /**
     * Renders the widget's contents and runs any queued after-render callbacks.
     */
    internalRenderContents() {
        const queue = (this as any)[afterRenderSymbol];
        if (queue) {
            let contents = this.renderContents();
            if (this.domNode && contents)
                appendToNode(this.domNode, contents);
            delete (this as any)[afterRenderSymbol];
            for (var callback of queue) callback();
        }
    }

    /**
     * Renders the widget's contents. Override this to provide custom content.
     * @returns The rendered contents.
     */
    protected renderContents(): any {
        if (this.legacyTemplateRender())
            return void 0;
        return (this.options as any).children;
    }

    /**
     * Renders the widget from a legacy `getTemplate` string, if defined.
     * @returns True if a legacy template was rendered.
     */
    protected legacyTemplateRender(): boolean {
        if (typeof (this as any).getTemplate !== "function")
            return;

        var template = (this as any).getTemplate();
        if (typeof template !== "string")
            return;

        template = template.replace(new RegExp('~_', 'g'), htmlEncode(this.idPrefix));
        this.domNode.innerHTML = template;
        return true;
    }

    /**
     * Returns the widget's props/options.
     */
    public get props(): WidgetProps<P> {
        return this.options;
    }

    /**
     * Runs a method synchronously or asynchronously depending on the widget's
     * `useAsync` flag, then invokes a continuation.
     * @param syncMethod - The synchronous method to run.
     * @param asyncMethod - The asynchronous method to run.
     * @param then - The continuation invoked with the result.
     */
    protected syncOrAsyncThen<T>(syncMethod: (() => T), asyncMethod: (() => PromiseLike<T>), then: (v: T) => void) {
        if (!(this as any).useAsync?.())
            then.call(this, syncMethod.call(this));
        else
            asyncMethod.call(this).then(then.bind(this));
    }

    /**
     * Returns an id prefix helper for resolving child element ids.
     * @returns An {@link IdPrefixType} proxy for this widget's id prefix.
     */
    protected useIdPrefix(): IdPrefixType {
        return useIdPrefix(this.idPrefix);
    }

    // jsx-dom >= 8.1.5 requires isComponent as a static property
    static readonly isComponent = true;

    /**
     * Registers this type as a class with the given type name.
     * @param typeName - The type name to register.
     * @param intfAndAttr - Optional interfaces and attributes.
     * @returns The class type info.
     */
    protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): ClassTypeInfo<TypeName> {
        if (Object.prototype.hasOwnProperty.call(this, Symbol.typeInfo) && this[Symbol.typeInfo])
            throw new Error(`Type ${this.name} already has a typeInfo property!`);

        const typeInfo = this[Symbol.typeInfo] = classTypeInfo(typeName, intfAndAttr);
        registerType(this);
        return typeInfo;
    }

    /**
     * Registers this type as an editor with the given type name.
     * @param typeName - The type name to register.
     * @param intfAndAttr - Optional interfaces and attributes.
     * @returns The editor type info.
     */
    protected static registerEditor<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): EditorTypeInfo<TypeName> {
        if (Object.prototype.hasOwnProperty.call(this, Symbol.typeInfo) && this[Symbol.typeInfo])
            throw new Error(`Type ${this.name} already has a typeInfo property!`);

        const typeInfo = this[Symbol.typeInfo] = editorTypeInfo(typeName, intfAndAttr);
        registerType(this);
        return typeInfo;
    }

    static [Symbol.typeInfo] = this.registerClass(nsSerenity);
}

/** @deprecated Use {@link Widget} */
export const TemplatedWidget = Widget;

// jsx-dom < 8.1.5 requires isReactComponent on prototype
Object.defineProperties(Widget.prototype, { isReactComponent: { value: true } });

/**
 * Parameters for {@link Widget.create}.
 * @typeParam TWidget - The widget type to create.
 * @typeParam P - The widget's options type.
 */
export interface CreateWidgetParams<TWidget extends Widget<P>, P> {
    /** The widget type to instantiate. */
    type?: { new(options?: P): TWidget, prototype: TWidget };
    /** The options to pass to the widget. */
    options?: P & WidgetProps<{}>;
    /** The container to append the widget's element to. */
    container?: HTMLElement | ArrayLike<HTMLElement>;
    /** Callback invoked with the created element. */
    element?: (e: Fluent) => void;
    /** Callback invoked after the widget is initialized. */
    init?: (w: TWidget) => void;
}
