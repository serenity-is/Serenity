import { Fluent, addClass, getTypeFullName, isArrayLike, toggleClass } from "@serenity-is/base";
import { ElementAttribute } from "../../decorators";
import { getAttributes } from "../../q/system-compat";
import { EditorUtils } from "../editors/editorutils";
import { EditorProps, WidgetProps } from "./widgetutils";

export let isFragmentWorkaround = Symbol();

export function ensureParentOrFragment(node: HTMLElement): HTMLElement {
    if (!node || node.parentNode)
        return node;
    let fragment = document.createDocumentFragment();
    fragment.appendChild(node);
    (fragment as any)[isFragmentWorkaround] = true;
    return node;
}

export function handleElementProp(type: { createDefaultElement(): HTMLElement }, props: WidgetProps<{}>): HTMLElement {
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

export function createDefaultElement(type: any) {
    var elementAttr = getAttributes(type, ElementAttribute, true);
    if (elementAttr.length) {
        let node: HTMLElement;
        let wrap = document.createElement("div");
        wrap.innerHTML = elementAttr[0].value;
        node = wrap.children[0] as HTMLElement;
        if (!node)
            return document.createElement(type.defaultTagName);
        node.parentNode?.removeChild(node);
        return node;
    }
    else {
        return document.createElement(type.defaultTagName);
    }
}

export function setElementProps(widget: { domNode: HTMLElement }, props: EditorProps<{}>): void {
    let el = widget.domNode;
    if (!el || !props)
        return;

    if (typeof props.id === "string")
        el.id = props.id;

    if (typeof props.name === "string")
        el.setAttribute("name", props.name);

    if (typeof props.placeholder === "string")
        el.setAttribute("placeholder", props.placeholder);

    if (typeof props.class === "string")
        addClass(el, props.class);

    if (typeof (props as any).className === "string")
        addClass(el, (props as any).className);

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
            EditorUtils.setRequired(widget as any, props.required);
        }
        catch {
        }
    }

    if (props.readOnly != null) {
        try {
            EditorUtils.setReadOnly(widget as any, props.readOnly);
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
