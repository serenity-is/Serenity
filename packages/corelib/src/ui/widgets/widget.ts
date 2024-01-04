import sQuery from "@optionaldeps/squery";
import { Config, getInstanceType, getTypeFullName, getTypeShortName, isArrayLike, stringFormat, toggleClass } from "@serenity-is/base";
import { Decorators, ElementAttribute } from "../../decorators";
import { jQueryPatch } from "../../patch/jquerypatch";
import { reactPatch } from "../../patch/reactpatch";
import { Exception, addValidationRule as addValRule, appendChild, getAttributes, replaceAll } from "../../q";
import { EditorUtils } from "../editors/editorutils";

export type NoInfer<T> = [T][T extends any ? 0 : never];

export type WidgetProps<P> = {
    id?: string;
    class?: string;
    element?: ((el: HTMLElement) => void) | HTMLElement | ArrayLike<HTMLElement> | string;
} & NoInfer<P>

export type EditorProps<T> = WidgetProps<T> & {
    initialValue?: any;
    maxLength?: number;
    name?: string;
    placeholder?: string;
    required?: boolean;
    readOnly?: boolean;
}

export interface CreateWidgetParams<TWidget extends Widget<P>, P> {
    type?: { new(options?: P): TWidget, prototype: TWidget };
    options?: P & WidgetProps<{}>;
    container?: HTMLElement | ArrayLike<HTMLElement>;
    element?: (e: JQuery) => void;
    init?: (w: TWidget) => void;
}

let initialized = Symbol();
let isFragmentWorkaround = Symbol();
let renderContentsCalled = Symbol();

@Decorators.registerClass('Serenity.Widget')
export class Widget<P = {}> {
    private static nextWidgetNumber = 0;
    declare protected readonly options: WidgetProps<P>;
    declare protected readonly widgetName: string;
    declare protected readonly uniqueName: string;
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
        this.setElementProps();

        this.widgetName = Widget.getWidgetName(getInstanceType(this));
        this.uniqueName = this.widgetName + (Widget.nextWidgetNumber++).toString();

        if (!sQuery.isMock) {
            if (sQuery(this.domNode).data(this.widgetName))
                throw new Exception(stringFormat("The element already has widget '{0}'!", this.widgetName));

            sQuery(this.domNode).on('remove.' + this.widgetName, e => {
                if (e.bubbles || e.cancelable) {
                    return;
                }
                this.destroy();
            }).data(this.widgetName, this);
        }

        this.idPrefix = this.uniqueName + '_';
        this.addCssClass();
        !getInstanceType(this).deferRenderContents && this.internalRenderContents();
    }

    public destroy(): void {
        if (this.domNode) {
            toggleClass(this.domNode, this.getCssClass(), false);
            !sQuery.isMock && sQuery(this.domNode).off('.' + this.widgetName).off('.' + this.uniqueName).removeData(this.widgetName);
            delete (this as any).domNode;
        }
    }

    static createDefaultElement(): HTMLElement {
        var elementAttr = getAttributes(this, ElementAttribute, true);
        if (elementAttr.length) {
            let node: HTMLElement;
            if (!sQuery.isMock) {
                node = sQuery(elementAttr[0].value).get(0);
                node.parentNode?.removeChild(node);
                return node;
            }
            let wrap = document.createElement("div");
            wrap.innerHTML = elementAttr[0].value;
            node = wrap.children[0] as HTMLElement;
            if (!node)
                return document.createElement(this.defaultTagName);
            node.parentNode?.removeChild(node);
            return node;
        }
        else {
            return document.createElement(this.defaultTagName);
        }
    }

    /**
     * @deprecated
     * Prefer domNode as this one depends on jQuery or a mock one if jQuery is not loaded
     */
    public get element(): JQuery {
        return sQuery(this.domNode);
    }

    protected addCssClass(): void {
        toggleClass(this.domNode, this.getCssClass(), true);
    }

    protected getCssClass(): string {
        var type = getInstanceType(this);
        var classList: string[] = [];
        var fullClass = replaceAll(getTypeFullName(type), '.', '-');
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
        return replaceAll(getTypeFullName(type), '.', '_');
    }

    /**
     * @deprecated Prefer WidgetType.createDefaultElement
     */
    public static elementFor(editorType: typeof Widget): JQuery {
        return sQuery(editorType.createDefaultElement());
    }

    public addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery {
        return addValRule(sQuery(this.domNode), eventClass, rule);
    }

    public getFieldElement(): HTMLElement {
        return this.domNode.closest('.field');
    }

    public getGridField(): JQuery {
        return sQuery(this.domNode).closest('.field');
    }

    public change(handler: (e: Event) => void) {
        sQuery(this.domNode).on('change.' + this.uniqueName, handler);
    };

    public changeSelect2(handler: (e: Event) => void) {
        sQuery(this.domNode).on('change.' + this.uniqueName, function (e, valueSet) {
            if (valueSet !== true)
                handler(e as any);
        });
    };

    protected static defaultTagName: string = "div";

    public static create<TWidget extends Widget<P>, P>(params: CreateWidgetParams<TWidget, P>) {
        let props: WidgetProps<P> = params.options ?? ({} as any);
        let node = handleElementProp(params.type as any, props);
        params.container && (isArrayLike(params.container) ? sQuery(params.container).append(node) : params.container.appendChild(node));
        params.element?.(sQuery(node));
        props.element = node;
        let widget = new params.type(props as any);
        widget.init();
        params.init?.(widget);
        return widget;
    }

    private setElementProps(): void {
        let el = this.domNode;
        let props = this.props as EditorProps<{}>;
        if (!el || !props)
            return;

        if (typeof props.id === "string")
            el.id = props.id;

        if (typeof props.name === "string")
            el.setAttribute("name", props.name);

        if (typeof props.placeholder === "string")
            el.setAttribute("placeholder", props.placeholder);

        if (typeof props.class === "string")
            toggleClass(el, props.class, true);

        if (typeof (props as any).className === "string")
            toggleClass(el, (props as any).className, true);

        if (typeof props.maxLength === "number")
            el.setAttribute("maxLength", (props.maxLength || 0).toString());
        else if (typeof (props as any).maxlength === "number")
            el.setAttribute("maxLength", ((props as any).maxlength || 0).toString());

        // using try catch here as some editors might not be able
        // to set them in the constructor if forwarding these
        // properties to any elements created in the constructor
        if (props.required != null)
        {
            try {
                EditorUtils.setRequired(this, props.required);
            }
            catch {
            }
        }

        if (props.readOnly != null) {
            try {
                EditorUtils.setReadOnly(this, props.readOnly);
            }
            catch {
            }
        }

        if (props.initialValue != null) {
            try {
                EditorUtils.setValue(this, props.initialValue);            
            }
            catch {
            }
        }
    }

    protected internalInit() {
        getInstanceType(this).deferRenderContents && this.internalRenderContents();

        if (typeof (this.options as any)?.ref === "function")
            (this.options as any)?.ref(this);
    }

    public init(): this {
        if (!(this as any)[initialized]) {
            (this as any)[initialized] = true;
            this.internalInit();
        }
        return this;
    }

    /**
     * Returns the main element for this widget or the document fragment.
     * As widgets may get their elements from props unlike regular JSX widgets, 
     * this method should not be overridden. Override renderContents() instead.
     */
    public render(): HTMLElement | DocumentFragment {
        let el = this.init().domNode;
        let parent = el?.parentNode;
        if (parent instanceof DocumentFragment &&
            parent.childNodes.length > 1 && 
            (parent as any)[isFragmentWorkaround])
            return parent;
        return el;
    }

    protected internalRenderContents() {
        if ((this as any)[renderContentsCalled])
            return;
        (this as any)[renderContentsCalled] = true;
        let contents = this.renderContents();
        if (typeof contents !== "undefined" && contents !== false && this.domNode)
            appendChild(contents, this.domNode);
    }

    protected renderContents(): any | void {
        return (this.options as any).children;
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
}

function ensureParentOrFragment(node: HTMLElement): HTMLElement {
    if (!node || node.parentNode)
        return node;
    let fragment = document.createDocumentFragment();
    fragment.appendChild(node);
    (fragment as any)[isFragmentWorkaround] = true;
    return node;
}

function handleElementProp(type: typeof Widget, props: WidgetProps<{}>): HTMLElement {
    let elementProp = props?.element;
    let domNode: HTMLElement;
    if (typeof elementProp == "string") {
        domNode = document.querySelector(elementProp);
        if (domNode == null)
            throw `The element ${elementProp} specified for the ${getTypeFullName(type)} is not found in the DOM!`;
    }
    else if (isArrayLike(elementProp)) {
        domNode = elementProp[0];
    }
    else if (elementProp instanceof HTMLElement) {
        domNode = elementProp;
    }
    else {
        domNode = type.createDefaultElement();
        if (typeof elementProp === "function")
            elementProp(domNode);
    }

    return ensureParentOrFragment(domNode);
}

Object.defineProperties(Widget.prototype, { isReactComponent: { value: true } });

export class EditorWidget<P> extends Widget<EditorProps<P>> {
    constructor(props: EditorProps<P>) {
        super(props);
    }
}


type IdPrefixType = { [key: string]: string, Form: string, Tabs: string, Toolbar: string, PropertyGrid: string };

export function useIdPrefix(prefix: string): IdPrefixType {
    return new Proxy({ _: prefix ?? '' }, idPrefixHandler);
}

const idPrefixHandler = {
    get(target: any, p: string) {
        if (p.startsWith('#'))
            return '#' + target._ + p.substring(1);

        return target._ + p;
    }
};

jQueryPatch(sQuery, Widget);
reactPatch();
