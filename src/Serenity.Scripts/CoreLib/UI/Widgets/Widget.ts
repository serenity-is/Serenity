/// <reference types="react" />

import { Decorators, ElementAttribute } from "../../Decorators";
import { Config } from "../../Q/Config";
import { Exception, ArgumentNullException } from "../../Q/System";
import { format } from "../../Q/Formatting";
import { replaceAll, startsWith } from "../../Q/Strings";
import { getAttributes, getInstanceType, getTypeName, getTypeFullName, isAssignableFrom  } from "../../Q/System";
import { IDialog } from "../../Interfaces";
import { addValidationRule as addValRule } from "../../Q/Validation";
import { notifyError } from "../../Q/Notify";

export interface WidgetClass<TOptions = object> {
    new(element: JQuery, options?: TOptions): Widget<TOptions>;
    element: JQuery;
}

export interface WidgetDialogClass<TOptions = object> {
    new(options?: TOptions): Widget<TOptions> & IDialog;
    element: JQuery;
}

export type AnyWidgetClass<TOptions = object> = WidgetClass<TOptions> | WidgetDialogClass<TOptions>;

export function reactPatch() {
    // @ts-ignore
    if (typeof React === "undefined" && typeof window !== "undefined") {
        if (window['preact'] != null) {
            window['React'] = window['ReactDOM'] = window['preact'];
            // @ts-ignore
            (React as any).Fragment = (React as any).Fragment ?? "x-fragment";
        }
        else if (window['Nerv'] != null) {
            window['React'] = window['ReactDOM'] = window['Nerv'];
            // @ts-ignore
            (React as any).Fragment = (React as any).Fragment ?? "x-fragment";
        }
        else {
            window['React'] = {
                Component: function () { } as any,
                Fragment: "x-fragment" as any,
                createElement: function () { return { _reactNotLoaded: true }; } as any
            } as any
            window['ReactDOM'] = {
                render: function () { throw Error("To use React, it should be included before Serenity.CoreLib.js"); }
            }
        }
    }
}

reactPatch();

export interface CreateWidgetParams<TWidget extends Widget<TOptions>, TOptions> {
    type?: new (element: JQuery, options?: TOptions) => TWidget;
    options?: TOptions;
    container?: JQuery;
    element?: (e: JQuery) => void;
    init?: (w: TWidget) => void;
}

@Decorators.registerClass()
// @ts-ignore
export class Widget<TOptions> extends React.Component<TOptions> {
    private static nextWidgetNumber = 0;
    public element: JQuery;
    protected options: TOptions;
    protected widgetName: string;
    protected uniqueName: string;

    constructor(element: JQuery, options?: TOptions) {
        super(options);

        this.element = element;
        this.options = options || ({} as TOptions);

        this.widgetName = Widget.getWidgetName(getInstanceType(this));
        this.uniqueName = this.widgetName + (Widget.nextWidgetNumber++).toString();

        if (element.data(this.widgetName)) {
            throw new Exception(format("The element already has widget '{0}'!", this.widgetName));
        }

        element.on('remove.' + this.widgetName, e => {
            if (e.bubbles || e.cancelable) {
                return;
            }
            this.destroy();
        }).data(this.widgetName, this);

        this.addCssClass();
    }

    public destroy(): void {
        if (this.element) {
            this.element.removeClass('s-' + getTypeName(getInstanceType(this)));
            this.element.off('.' + this.widgetName).off('.' + this.uniqueName).removeData(this.widgetName);
            this.element = null;
        }
    }

    protected addCssClass(): void {
        this.element.addClass(this.getCssClass());
    }

    protected getCssClass(): string {
        var type = getInstanceType(this);
        var klass = 's-' + getTypeName(type);
        var fullClass = replaceAll(getTypeFullName(type), '.', '-');

        for (let k of Config.rootNamespaces) {
            if (startsWith(fullClass, k + '-')) {
                fullClass = fullClass.substr(k.length + 1);
                break;
            }
        }

        fullClass = 's-' + fullClass;
        if (klass === fullClass) {
            return klass;
        }

        return klass + ' ' + fullClass;
    }

    public static getWidgetName(type: Function): string {
        return replaceAll(getTypeFullName(type), '.', '_');
    }

    public static elementFor<TWidget>(editorType: { new (...args: any[]): TWidget }): JQuery {
        var elementAttr = getAttributes(editorType, ElementAttribute, true);
        var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].value : '<input/>');
        return $(elementHtml);
    };

    public addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery {
        return addValRule(this.element, eventClass, rule);
    }

    public getGridField(): JQuery {
        return this.element.closest('.field');
    } 

    public change(handler: (e: JQueryEventObject) => void) {
        this.element.on('change.' + this.uniqueName, handler);
    };

    public changeSelect2(handler: (e: JQueryEventObject) => void) {
        this.element.on('change.' + this.uniqueName, function (e, valueSet) {
            if (valueSet !== true)
                handler(e);
        });
    };

    public static create<TWidget extends Widget<TOpt>, TOpt>(params: CreateWidgetParams<TWidget, TOpt>) {
        let widget: TWidget;

        if (isAssignableFrom(IDialog, params.type)) {
            widget = new (params.type as any)(params.options);
            if (params.container)
                widget.element.appendTo(params.container);
            params.element && params.element(widget.element);
        }
        else {
            var e = Widget.elementFor(params.type);
            if (params.container)
                e.appendTo(params.container);
            params.element && params.element(e);
            widget = new params.type(e, params.options);
        }

        widget.init(null);
        params.init && params.init(widget);

        return widget;
    }

    public initialize(): void {
    }

    public init(action?: (widget: any) => void): this {
        action && action(this);
        return this;
    }

    private static __isWidgetType = true;
    props: Readonly<{ children?: React.ReactNode }> & Readonly<TOptions> & WidgetComponentProps<this>;
}

if (typeof $ !== "undefined" && $.fn) {
    $.fn.tryGetWidget = function (this: JQuery, type: any) {
        var element = this;
        var w;
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

    $.fn.getWidget = function<TWidget>(this: JQuery, type: { new (...args: any[]): TWidget }) {
        if (this == null) {
            throw new ArgumentNullException('element');
        }
        if (this.length === 0) {
            throw new Exception(format("Searching for widget of type '{0}' on a non-existent element! ({1})",
                getTypeFullName(type), this.selector));
        }

        var w = this.tryGetWidget(type);
        if (w == null) {
            var message = format("Element has no widget of type '{0}'! If you have recently changed " +
                "editor type of a property in a form class, or changed data type in row (which also changes " +
                "editor type) your script side Form definition might be out of date. Make sure your project " +
                "builds successfully and transform T4 templates", getTypeFullName(type));
            notifyError(message, '', null);
            throw new Exception(message);
        }
        return w;
    };
}

export interface WidgetComponentProps<W extends Widget<any>> {
    id?: string;
    name?: string;
    className?: string;
    maxLength?: number;
    placeholder?: string;
    setOptions?: any;
    required?: boolean;
    readOnly?: boolean;
    oneWay?: boolean;
    onChange?: (e: JQueryEventObject) => void;
    onChangeSelect2?: (e: JQueryEventObject) => void;
    value?: any;
    defaultValue?: any;
}

export declare interface Widget<TOptions> {
    change(handler: (e: JQueryEventObject) => void): void;
    changeSelect2(handler: (e: JQueryEventObject) => void): void;
}