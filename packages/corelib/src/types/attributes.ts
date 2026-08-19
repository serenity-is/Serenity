import { CustomAttribute, classTypeInfo, nsSerenity, registerType } from "../base";

/**
 * Indicates whether a dialog should show a close button in its title bar.
 */
export class CloseButtonAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * @param value - True to show the close button (default `true`).
     */
    constructor(public value = true) {
        super();
    }
}

/**
 * Specifies the root element tag for a widget (e.g. `"div"`, `"span"`).
 */
export class ElementAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * @param value - Element tag name.
     */
    constructor(public value: string) {
        super();
    }
}

/**
 * Indicates whether a grid should expose the advanced filter editor.
 */
export class AdvancedFilteringAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /** @param value - True to enable advanced filtering (default `true`). */
    constructor(public value = true) {
        super();
    }
}


/**
 * Indicates that a dialog should be maximizable.
 * @remarks Requires jQuery UI and `jquery.dialogextend.js`; not applicable to Bootstrap modals.
 */
export class MaximizableAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /** @param value - True to allow maximizing (default `true`). */
    constructor(public value = true) {
        super();
    }
}

/**
 * Indicates that the property is an option. This is no longer used as JSX
 * does not support it, but it is kept for backward compatibility.
 */
export class OptionAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }
}

/**
 * Indicates that a dialog should open as a side panel by default.
 */
export class PanelAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /** @param value - True to prefer panel mode (default `true`). */
    constructor(public value = true) {
        super();
    }
}

/**
 * Indicates whether a dialog should be resizable (jQuery UI dialogs only).
 */
export class ResizableAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /** @param value - True to allow resizing (default `true`). */
    constructor(public value = true) {
        super();
    }
}

/**
 * Indicates that the widget should render as a static panel (plain div embedded
 * in the page without title bar / modal behavior).
 */
export class StaticPanelAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /** @param value - True to render as a static panel (default `true`). */
    constructor(public value = true) {
        super();
    }
}

/**
 * Factory helpers for common widget attributes. Each method creates an attribute instance
 * and is flagged with `isAttributeFactory` for reflection discovery.
 */
export namespace Attributes {
    /** Creates an {@link AdvancedFilteringAttribute}. @param value - True to enable (default `true`). */
    export function advancedFiltering(value = true) { return new AdvancedFilteringAttribute(value); }
    /** Creates a {@link CloseButtonAttribute}. @param value - True to show close button (default `true`). */
    export function closeButton(value = true) { return new CloseButtonAttribute(value); }
    /** Creates a {@link ResizableAttribute}. @param value - True to allow resizing (default `true`). */
    export function resizable(value = true) { return new ResizableAttribute(value); }
    /** Creates a {@link MaximizableAttribute}. @param value - True to allow maximizing (default `true`). */
    export function maximizable(value = true) { return new MaximizableAttribute(value); }
    /** Creates a {@link PanelAttribute}. @param value - True to prefer panel mode (default `true`). */
    export function panel(value = true) { return new PanelAttribute(value); }
    /** Creates a {@link StaticPanelAttribute}. @param value - True for static panel (default `true`). */
    export function staticPanel(value = true) { return new StaticPanelAttribute(value); }
    
    Object.keys(Attributes).forEach(key => (Attributes as any)[key].isAttributeFactory = true);
}

/** @deprecated Use Attributes.advancedFiltering() instead */
export const FilterableAttribute = AdvancedFilteringAttribute;