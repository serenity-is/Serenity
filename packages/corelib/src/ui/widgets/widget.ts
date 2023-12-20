import sQuery from "@optionaldeps/squery";
import { Config, getGlobalObject, getInstanceType, getTypeFullName, getTypeShortName, isArrayLike, isAssignableFrom, notifyError, stringFormat, toggleClass } from "@serenity-is/base";
import { Decorators, ElementAttribute } from "../../decorators";
import { jQueryPatch } from "../../patch/jquerypatch";
import { ArgumentNullException, Exception, addValidationRule as addValRule, getAttributes, replaceAll } from "../../q";
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
    type?: { new (options?: P): TWidget, prototype: TWidget };
    options?: P & WidgetProps<{}>;
    container?: HTMLElement | ArrayLike<HTMLElement>;
    element?: (e: JQuery) => void;
    init?: (w: TWidget) => void;
}

@Decorators.registerClass('Serenity.Widget')
export class Widget<P = {}> {
    private static nextWidgetNumber = 0;
    protected options: WidgetProps<P>;
    protected widgetName: string;
    protected uniqueName: string;
    declare public readonly idPrefix: string;
    public readonly node: HTMLElement;

    public get element(): JQuery {
        return jQuery(this.node);
    }

    constructor(props?: WidgetProps<P>) {
        if (isArrayLike(props)) {
            this.node = props[0];
            this.options = {} as any;
        }
        else {
            this.options = props ?? {} as any;
            this.node = handleElementProp(getInstanceType(this), this.options.element);
        }

        delete this.options.element;
        this.setElementProps();

        this.widgetName = Widget.getWidgetName(getInstanceType(this));
        this.uniqueName = this.widgetName + (Widget.nextWidgetNumber++).toString();

        if (!jQuery.isMock) {
            if (jQuery(this.node).data(this.widgetName))
                throw new Exception(stringFormat("The element already has widget '{0}'!", this.widgetName));

            jQuery(this.node).on('remove.' + this.widgetName, e => {
                if (e.bubbles || e.cancelable) {
                    return;
                }
                this.destroy();
            }).data(this.widgetName, this);
        }

        this.addCssClass();
        this.idPrefix = this.uniqueName + '_';
        this.renderContents();
    }

    public destroy(): void {
        if (this.node) {
            toggleClass(this.node, this.getCssClass(), false);
            !jQuery.isMock && jQuery(this.node).off('.' + this.widgetName).off('.' + this.uniqueName).removeData(this.widgetName);
            delete (this as any).node;
        }
    }

    static createNode(): HTMLElement {
        var elementAttr = getAttributes(this, ElementAttribute, true);
        if (elementAttr.length) {
            if (!jQuery.isMock)
                return jQuery(elementAttr[0].value).get(0);
            var el = document.createElement("div");
            el.innerHTML = elementAttr[0].value;
            return el.children[0] as HTMLElement ?? document.createElement("input");
        }
        else {
            return document.createElement("input");
        }
    }

    protected addCssClass(): void {
        toggleClass(this.node, this.getCssClass(), true);
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

    public static elementFor<TWidget>(editorType: { new(...args: any[]): TWidget }): JQuery {
        return $((editorType as any).createNode());
    }

    public addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery {
        return addValRule(sQuery(this.node), eventClass, rule);
    }

    public getGridField(): JQuery {
        return sQuery(this.node).closest('.field');
    }

    public change(handler: (e: Event) => void) {
        sQuery(this.node).on('change.' + this.uniqueName, handler);
    };

    public changeSelect2(handler: (e: Event) => void) {
        sQuery(this.node).on('change.' + this.uniqueName, function (e, valueSet) {
            if (valueSet !== true)
                handler(e as any);
        });
    };

    public static create<TWidget extends Widget<P>, P>(params: CreateWidgetParams<TWidget, P>) {
        let props: WidgetProps<P> = params.options ?? ({} as any);
        let node = handleElementProp(params.type as any, props.element);
        params.container && (isArrayLike(params.container) ? sQuery(params.container).append(node) : params.container.appendChild(node));
        params.element?.(sQuery(node));
        props.element = node;
        let widget = new params.type(props as any);
        widget.init();
        params.init?.(widget);
        return widget;
    }

    private setElementProps(): void {
        let el = this.node;
        let props = this.props as EditorProps<{}>;
        if (!el || !props)
            return;

        if (props.id != null) {
            el.id = props.id;
            delete props.id;
        }

        if (props.name != null)
            el.setAttribute('name', props.name);

        if (props.placeholder != null)
            el.setAttribute("placeholder", props.placeholder);

        if (props.class != null) {
            el.className = props.class;
            delete props.class
        }
        else if (typeof (props as any).className === "string") {
            el.className = (props as any).className;
            delete (props as any).className;
        }

        if (props.maxLength != null)
            el.setAttribute("maxLength", (props.maxLength || 0).toString());
        else if ((props as any).maxlength != null)
            el.setAttribute("maxLength", ((props as any).maxlength || 0).toString());
    }

    protected initialized: boolean;

    protected initialize(): void {
        if (this.initialized)
            return;

        let props = this.props as EditorProps<any>;
        if (props.required != null)
            EditorUtils.setRequired(this, props.required);

        if (props.readOnly !== null)
            EditorUtils.setReadOnly(this, props.readOnly);

        if (props.initialValue != undefined)
            EditorUtils.setValue(this, props.initialValue);

        if (typeof (props as any).ref === "function") {
            (props as any).ref(this);
        }
    }

    public init(): this {
        if (!this.initialized) {
            try {
                this.initialize();
            }
            finally {
                this.initialized = true;
            }
        }
        return this;
    }

    public render(): HTMLElement {
        return this.init().node;
    }

    protected renderContents(): void {
    }

    public get props(): WidgetProps<P> {
        return this.options;
    }
}

function handleElementProp(type: typeof Widget, element: (((el: HTMLElement) => void) | HTMLElement | ArrayLike<HTMLElement> | string)): HTMLElement {
    let node: HTMLElement;
    if (typeof element == "string") {
        node = document.querySelector(element);
        if (node == null)
            throw `The element ${element} specified for the ${getTypeFullName(type)} is not found in the DOM!`;
    }
    else if (isArrayLike(element)) {
        node = element[0];
    }
    else if (element instanceof HTMLElement) {
        node = element;
    }
    else {
        node = type.createNode();
        if (typeof element === "function")
            element(node);
    }
    return node;
}

Object.defineProperties(Widget.prototype, { isReactComponent: { value: true } });

export declare interface Widget<P> {
    change(handler: (e: Event) => void): void;
    changeSelect2(handler: (e: Event) => void): void;
}

export class WidgetComponent<P> extends Widget<P> {
    constructor(props?: WidgetProps<P>) {
        super(props);
    }
}

export class EditorComponent<P> extends Widget<EditorProps<P>> {
    constructor(props?: EditorProps<P>) {
        super(props);
    }
}

export declare interface Widget<P> {
    change(handler: (e: Event) => void): void;
    changeSelect2(handler: (e: Event) => void): void;
}

sQuery.fn.tryGetWidget = function (this: JQuery, type?: any) {
    var element = this;
    var w;
    type ??= Widget;
    if (isAssignableFrom(Widget, type)) {
        var widgetName = Widget.getWidgetName(type);
        w = element.data(widgetName);
        if (w != null && !isAssignableFrom(type, getInstanceType(w))) {
            w = null;
        }
        if (w != null) {
            return w;
        }
    }

    var data = element.data();
    if (data == null) {
        return null;
    }

    for (var key of Object.keys(data)) {
        w = data[key];
        if (w != null && isAssignableFrom(type, getInstanceType(w))) {
            return w;
        }
    }

    return null;
};

sQuery.fn.getWidget = function <TWidget>(this: JQuery, type: { new(...args: any[]): TWidget }) {
    if (this == null) {
        throw new ArgumentNullException('element');
    }
    if (this.length === 0) {
        throw new Exception(stringFormat("Searching for widget of type '{0}' on a non-existent element! ({1})",
            getTypeFullName(type), (this as any).selector));
    }

    var w = (this as any).tryGetWidget(type);
    if (w == null) {
        var message = stringFormat("Element has no widget of type '{0}'! If you have recently changed " +
            "editor type of a property in a form class, or changed data type in row (which also changes " +
            "editor type) your script side Form definition might be out of date. Make sure your project " +
            "builds successfully and transform T4 templates", getTypeFullName(type));
        notifyError(message, '', null);
        throw new Exception(message);
    }
    return w;
};

!sQuery.isMock && jQueryPatch(sQuery);

export function reactPatch() {
    let global = getGlobalObject();
    if (!global.React) {
        if (global.preact) {
            global.React = global.ReactDOM = global.preact;
            global.React.Fragment = global.Fragment ?? "x-fragment";
        }
        else {
            global.React = {
                Component: function () { },
                Fragment: "x-fragment",
                createElement: function () { return { _reactNotLoaded: true }; }
            }
            global.ReactDOM = {
                render: function () { throw Error("To use React, it should be included before Serenity.CoreLib.js"); }
            }
        }
    }
}

reactPatch();
