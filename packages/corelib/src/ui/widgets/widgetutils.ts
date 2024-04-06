import { Fluent, NoInfer, getInstanceType, getTypeFullName, isArrayLike, isAssignableFrom, notifyError } from "../../base";

let elementMap: WeakMap<Element, { [key: string]: { domNode: HTMLElement } }> = new WeakMap();

export function getWidgetName(type: Function): string {
    return getTypeFullName(type)?.replace(/\./g, '_');
}

export function associateWidget(widget: { domNode: HTMLElement }) {
    if (!widget || !widget.domNode)
        return;
    let type = getInstanceType(widget);
    let name = getWidgetName(type);
    var widgets = elementMap.get(widget.domNode);
    if (widgets) {
        if (widgets[name])
            throw new Error(`The element already has widget '${name}!`);

        widgets[name] = widget;
    }
    else {
        elementMap.set(widget.domNode, {
            [name]: widget
        });
    }
}

export function deassociateWidget(widget: { domNode: HTMLElement }) {
    if (!widget || !widget.domNode)
        return;
    let type = getInstanceType(widget);
    let name = getWidgetName(type);
    var widgets = elementMap.get(widget.domNode);
    if (widgets) {
        delete widgets[name];
        if (!Object.keys(widgets).length)
            elementMap.delete(widget.domNode);
    }
}

export function tryGetWidget<TWidget>(element: Element | ArrayLike<HTMLElement> | string, type?: { new (...args: any[]): TWidget }): TWidget {

    if (typeof element === "string") {
        element = document.querySelector(element);
    }
    else if (isArrayLike(element))
        element = element[0];

    if (!element)
        return null;

    let widgets = elementMap.get(element);
    if (!widgets)
        return null;

    var keys = Object.keys(widgets);
    if (!keys.length)
        return null;

    if (!type)
        return (widgets[keys[0]] ?? null) as TWidget;

    var name = getWidgetName(type);
    var widget = widgets[name];
    if (widget)
        return widgets[name] as TWidget;

    for (var key of Object.keys(widgets)) {
        widget = widgets[key];
        if (widget && isAssignableFrom(type, getInstanceType(widget)))
            return widget as TWidget;
    }

    return null;
}

export function getWidgetFrom<TWidget>(element: ArrayLike<HTMLElement> | Element | string, type?: { new (...args: any[]): TWidget }): TWidget {
    let selector: string;
    if (typeof element === "string") {
        selector = element;
        element = document.querySelector(selector);
    }

    if (!element)
        throw new Error(`Searching for widget of type '${getTypeFullName(type) ?? "Widget"}' on a non-existent element! (${selector ?? 'unknown'})`);

    var widget = tryGetWidget(element, type);
    if (!widget) {
        var message = `Element (${selector ?? 'unknown'}) has no widget of type '${getTypeFullName(type)}'! If you have recently changed ` +
            "editor type of a property in a form class, or changed data type in row (which also changes " +
            "editor type) your script side Form definition might be out of date. Make sure your project " +
            "builds successfully and transformations are executed.";
        notifyError(message, '', null);
        throw new Error(message);
    }

    return widget as TWidget;
}

Fluent.prototype.getWidget = function<TWidget>(this: Fluent, type?: { new (...args: any[]): TWidget }): TWidget {
    return getWidgetFrom(this, type);
}

Fluent.prototype.tryGetWidget = function<TWidget>(this: Fluent, type?: { new (...args: any[]): TWidget }): TWidget {
    return tryGetWidget(this, type);
}

export type IdPrefixType = { [key: string]: string, Form: string, Tabs: string, Toolbar: string, PropertyGrid: string };

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

