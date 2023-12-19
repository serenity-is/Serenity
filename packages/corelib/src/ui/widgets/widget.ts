import sQuery from "@optionaldeps/squery";
import { Config, JQueryLike, getGlobalObject, getInstanceType, getTypeFullName, getTypeShortName, isAssignableFrom, isJQueryLike, notifyError, stringFormat, toggleClass } from "@serenity-is/base";
import { Decorators, ElementAttribute } from "../../decorators";
import { IDialog } from "../../interfaces";
import { jQueryPatch } from "../../patch/jquerypatch";
import { ArgumentNullException, Exception, addValidationRule as addValRule, getAttributes, replaceAll } from "../../q";
import { EditorUtils } from "../editors/editorutils";

export type NoInfer<T> = [T][T extends any ? 0 : never];

export type WidgetNode = JQueryLike | HTMLElement;

export type WidgetProps<P> = {
    id?: string;
    class?: string;
    nodeRef?: (el: HTMLElement) => void;
    replaceNode?: HTMLElement;
} & NoInfer<P>

export type WidgetNodeOrProps<P> = WidgetNode | WidgetProps<P>;

export type EditorProps<T> = WidgetProps<T> & {
    initialValue?: any;
    maxLength?: number;
    name?: string;
    placeholder?: string;
    required?: boolean;
    readOnly?: boolean;
}

export interface CreateWidgetParams<TWidget extends Widget<P>, P> {
    type?: (new (node: WidgetNode, options?: P) => TWidget) | (new (props?: P) => TWidget);
    options?: WidgetProps<P>;
    container?: WidgetNode;
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

    constructor(node: WidgetNode, opt?: WidgetProps<P>);
    constructor(props?: WidgetProps<P>);
    constructor(props?: any, opt?: any) {
        if (isJQueryLike(props))
            this.node = props.get(0);
        else if (props instanceof HTMLElement)
            this.node = props;
        else {
            if (isJQueryLike(opt))
                this.node = opt.get(0);
            else if (opt instanceof HTMLElement)
                this.node = opt;
            else if (props && (props as WidgetProps<P>).replaceNode) {
                this.node = props.replaceNode
                delete props.replaceNode;
            }
            else
                this.node = getInstanceType(this).createNode();
            opt ??= props;
        }

        Widget.setElementProps(this.node, opt);
        this.options = opt || {};

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
        this.initialize();
        Widget.setInstanceProps(this, props);
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
        let widget: TWidget;

        if (isAssignableFrom(IDialog, params.type) ||
            (params.type as any).isWidgetComponent) {
            if (params.container || params.element) {
                var oldRef = params.options?.nodeRef;
                params.options ??= {} as any;
                params.options.nodeRef = (node => {
                    typeof oldRef === "function" && oldRef(node);
                    if (params.container) {
                        if (isJQueryLike(params.container))
                            (params.container as JQuery).append(node);
                        else
                            params.container.appendChild(node);
                    }
                    params.element && params.element(sQuery(node));
                });
            }
            widget = new (params.type as any)(params.options);
        }
        else {
            var e = Widget.elementFor(params.type);
            if (params.container)
                e.appendTo(params.container as JQuery);
            params.element && params.element(e);
            widget = new params.type(e as any, params.options as any);
        }

        if (params.options && widget.options !== params.options) {
            // widget has a constructor that does not accept options
            Widget.setElementProps(widget.node, params.options);
            Widget.setInstanceProps(widget, params.options);
        }

        widget.init(null);
        params.init && params.init(widget);

        return widget;
    }

    static setElementProps(el: HTMLElement, props: any): void {
        if (!el || !props)
            return;

        if (typeof props.nodeRef === "function") {
            props.nodeRef(el);
            delete props.nodeRef;
        }

        if (props.id != null)
            el.id = props.id;

        if (props.name != null)
            el.setAttribute('name', props.name);

        if (props.placeholder != null)
            el.setAttribute("placeholder", props.placeholder);

        if (props.class != null)
            el.className = props.class;

        if (props.maxLength != null)
            el.setAttribute("maxLength", (props.maxLength || 0).toString());
        else if ((props as any).maxlength != null)
            el.setAttribute("maxLength", ((props as any).maxlength || 0).toString());
    }

    private static setInstanceProps(widget: Widget, props: any) {
        if (!props || !widget?.node)
            return;

        if (props.required != null)
            EditorUtils.setRequired(widget, props.required);

        if (props.readOnly !== null)
            EditorUtils.setReadOnly(widget, props.readOnly);

        if (props.initialValue != undefined)
            EditorUtils.setValue(widget, props.initialValue);

        if ((props as any).ref)
            (props as any).ref(widget);
    }

    public initialize(): void {
        this.renderContents();
    }

    public init(action?: (widget: any) => void): this {
        action && action(this);
        return this;
    }

    public render(): HTMLElement {
        return this.node;
    }

    protected renderContents(): void {
    }

    public get props(): WidgetProps<P> {
        return this.options;
    }

    static isWidgetComponent: boolean;
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

    static override isWidgetComponent: true = true;
}

export class EditorComponent<P> extends Widget<EditorProps<P>> {
    constructor(props?: EditorProps<P>) {
        super(props);
    }

    static override isWidgetComponent: true = true;
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