import { classTypeInfo, EnumKeyAttribute, nsSerenity, registerType } from "../base";

/**
 * Indicates if a dialog should have a close button in its title bar (default true)
 */
export class CloseButtonAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

/**
 * Indicates the element type of a widget like "div", "span" etc.
 */
export class ElementAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value: string) {
    }
}

/**
 * Indicates if a grid should have an advanced filter editor
 */
export class AdvancedFilteringAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}


/**
 * Indicates that a dialog or panel should be maximizable.
 * Requires jquery ui dialogs and jquery.dialogextend.js.
 * It does not work with current bootstrap modals.
 */
export class MaximizableAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

/**
 * Indicates that the property is an option. This is no longer used as JSX
 * does not support it, but it is kept for backward compatibility.
 */
export class OptionAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }
}

/**
 * Indicates if a dialog should be opened as a panel
 */
export class PanelAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

/**
 * Indicates if a dialog should be resizable, only for jquery ui dialogs.
 */
export class ResizableAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

/**
 * Indicates if a dialog should be a static panel, which is not a dialog at all,
 * but a simple div element embedded in the page.
 * It does not have a title bar, close button or modal behavior.
 * It is just a way to show a form inside a page, without any dialog stuff.
 */
export class StaticPanelAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

export namespace Attributes {
    /** Indicates if a grid should have an advanced filter editor */
    export function advancedFiltering(value = true) { return new AdvancedFilteringAttribute(value); }
    /** Indicates if a dialog should have a close button in its title bar (default true) */
    export function closeButton(value = true) { return new CloseButtonAttribute(value); }
    /** Indicates if a dialog should be resizable, only for jquery ui dialogs. */
    export function resizable(value = true) { return new ResizableAttribute(value); }
    /** Indicates if a dialog should be maximizable, only for jquery ui dialogs. */
    export function maximizable(value = true) { return new MaximizableAttribute(value); }
    /** Indicates if a dialog should be opened as a panel by default (default null) */
    export function panel(value = true) { return new PanelAttribute(value); }
    /** Indicates if a dialog should be a static panel, which is not a dialog at all. */
    export function staticPanel(value = true) { return new StaticPanelAttribute(value); }
}


/** @deprecated Use Attributes.advancedFiltering() instead */
export const FilterableAttribute = AdvancedFilteringAttribute;