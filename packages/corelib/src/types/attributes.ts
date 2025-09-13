import { classTypeInfo, nsSerenity, registerType } from "../base";

/**
 * Indicates the enum key of an enum type (by default the name of the enum type is used as key)
 */
export class EnumKeyAttribute {
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value: string) {
    }
}

/**
 * Indicates if a dialog should have a close button in its title bar (default true)
 */
export class CloseButtonAttribute {
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

/**
 * Indicates the element type of a widget like "div", "span" etc.
 */
export class ElementAttribute {
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value: string) {
    }
}

/**
 * Indicates if a grid should have an advanced filter editor
 */
export class FilterableAttribute {
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

/**
 * Indicates that a dialog or panel should be maximizable.
 * Requires jquery ui dialogs and jquery.dialogextend.js.
 * It does not work with current bootstrap modals.
 */
export class MaximizableAttribute {
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

/**
 * Indicates that the property is an option. This is no longer used as JSX
 * does not support it, but it is kept for backward compatibility.
 */
export class OptionAttribute {
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }
}

/**
 * Indicates if a dialog should be opened as a panel
 */
export class PanelAttribute {
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}

/**
 * Indicates if a dialog should be resizable, only for jquery ui dialogs.
 */
export class ResizableAttribute {
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }

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
    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public value = true) {
    }
}
