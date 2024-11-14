import { Config, Fluent, addClass, addValidationRule, appendToNode, getCustomAttribute, getInstanceType, getTypeFullName, getTypeShortName, isArrayLike, toggleClass } from "../../base";
import { Decorators } from "../../types/decorators";
import { ensureParentOrFragment, handleElementProp, isFragmentWorkaround, setElementProps } from "./widgetinternal";
import { IdPrefixType, associateWidget, deassociateWidget, getWidgetName, useIdPrefix, type WidgetProps } from "./widgetutils";
export { getWidgetFrom, tryGetWidget, useIdPrefix, type IdPrefixType, type WidgetProps } from "./widgetutils";

const afterRenderSymbol = Symbol();

@Decorators.registerType()
export class Widget<P = {}> {
    static typeInfo = Decorators.classType("Serenity.Widget");

    private static nextWidgetNumber = 0;
    declare protected readonly options: WidgetProps<P>;
    declare public readonly uniqueName: string;
    declare public readonly idPrefix: string;
    declare public readonly domNode: HTMLElement;

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

        Fluent.one(this.domNode, 'disposing.' + this.uniqueName, (e) => {
            this.destroy();
        });

        this.idPrefix = (this.options as any)?.idPrefix ?? (this.uniqueName + '_');

        (this as any)[afterRenderSymbol] = [];        
        this.addCssClass();
        !this.deferRender() && this.internalRenderContents();
    }

    public destroy(): void {
        if (this.domNode) {
            deassociateWidget(this);
            toggleClass(this.domNode, this.getCssClass(), false);
            Fluent.off(this.domNode, '.' + this.uniqueName);
            delete (this as any).domNode;
        }
    }

    static createDefaultElement(): HTMLElement {
        return document.createElement("div");
    }

    /**
     * Returns a Fluent(this.domNode) object
     */
    public get element(): Fluent {
        return Fluent(this.domNode);
    }

    protected addCssClass(): void {
        addClass(this.domNode, this.getCssClass());
    }

    protected deferRender() {
        return false;
    }

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

    public static getWidgetName(type: Function): string {
        return getWidgetName(type);
    }

    public addValidationRule(rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string, uniqueName?: string): void;
    public addValidationRule(uniqueName: string, rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string): void;
    public addValidationRule(rule: any, uniqueName: any): void {
        addValidationRule(this.domNode, typeof rule === "function" ? rule : uniqueName,
            typeof rule === "function" ? uniqueName ?? this.uniqueName : rule);
    }

    protected byId<TElement extends HTMLElement = HTMLElement>(id: string): Fluent<TElement> {
        return this.element.findFirst<TElement>('#' + this.idPrefix + id);
    }

    protected findById<TElement extends HTMLElement = HTMLElement>(id: string): TElement {
        return this.domNode?.querySelector<TElement>('#' + this.idPrefix + id);
    }

    public getGridField(): Fluent {
        return Fluent(this.domNode.closest('.field'));
    }

    public change(handler: (e: Event) => void) {
        Fluent.on(this.domNode, "change." + this.uniqueName, handler);
    };

    public changeSelect2(handler: (e: Event) => void) {
        Fluent.on(this.domNode, "change." + this.uniqueName, e => {
            if ((e.target as HTMLElement)?.dataset?.comboboxsettingvalue)
                return;
            handler(e);
        });
    }

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

    protected getCustomAttribute<TAttr>(attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): TAttr {
        return getCustomAttribute(getInstanceType(this), attrType, inherit);
    }

    protected afterRender(callback: () => void) {
        if (!callback)
            return;

        const queue = (this as any)[afterRenderSymbol];
        if (!queue)
            callback();
        else 
            queue.push(callback);
    }

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

    protected renderContents(): any {
        if (this.legacyTemplateRender())
            return void 0;
        return (this.options as any).children;
    }

    protected legacyTemplateRender(): boolean {
        if (typeof (this as any).getTemplate !== "function")
            return;

        var template = (this as any).getTemplate();
        if (typeof template !== "string")
            return;

        template = template.replace(new RegExp('~_', 'g'), this.idPrefix);
        this.domNode.innerHTML = template;
        return true;
    }

    public get props(): WidgetProps<P> {
        return this.options;
    }

    protected syncOrAsyncThen<T>(syncMethod: (() => T), asyncMethod: (() => PromiseLike<T>), then: (v: T) => void) {
        if (!(this as any).useAsync?.())
            then.call(this, syncMethod.call(this));
        else
            asyncMethod.call(this).then(then.bind(this));
    }

    protected useIdPrefix(): IdPrefixType {
        return useIdPrefix(this.idPrefix);
    }

    // jsx-dom >= 8.1.5 requires isComponent as a static property
    static readonly isComponent = true;
}

/** @deprecated Use Widget */
export const TemplatedWidget = Widget;

// jsx-dom < 8.1.5 requires isReactComponent on prototype
Object.defineProperties(Widget.prototype, { isReactComponent: { value: true } });

export interface CreateWidgetParams<TWidget extends Widget<P>, P> {
    type?: { new(options?: P): TWidget, prototype: TWidget };
    options?: P & WidgetProps<{}>;
    container?: HTMLElement | ArrayLike<HTMLElement>;
    element?: (e: Fluent) => void;
    init?: (w: TWidget) => void;
}
