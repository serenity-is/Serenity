import { addDisposingListener } from "@serenity-is/domwise";
import { classTypeInfo, Fluent, notifyError, nsSerenity, registerType } from "../../base";
import { findElementWithRelativeId } from "../../compat";
import { Widget } from "../widgets/widget";
import { tryGetWidget } from "../widgets/widgetutils";

/**
 * Links a widget to a parent widget so that it reacts to the parent's changes,
 * typically used for cascading select editors.
 * @typeParam TParent - The parent widget type.
 */
export class CascadedWidgetLink<TParent extends Widget<any>> {

    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * Creates a cascaded widget link.
     * @param parentType - Constructor of the parent widget type.
     * @param widget - The child widget to link.
     * @param parentChange - Callback invoked when the parent changes.
     */
    constructor(private parentType: { new(...args: any[]): TParent },
        private widget: Widget<any>,
        private parentChange: (p1: TParent) => void) {
        addDisposingListener(this.widget.domNode, () => {
            this.unbind();
            this.widget = null;
            this.parentChange = null;
        }, this.widget.uniqueName);
    }

    declare private _parentID: string;
    private _parentNode?: HTMLElement;

    /**
     * Binds the link to the parent widget and subscribes to its change event.
     * @returns The parent widget, or null if not found.
     */
    bind() {

        if (!this._parentID) {
            return null;
        }

        var parent = tryGetWidget(findElementWithRelativeId(this.widget.domNode, this._parentID), this.parentType);

        if (parent != null) {
            this._parentNode = parent.domNode;
            Fluent.on(this._parentNode, 'change.' + (this.widget as any).uniqueName, () => {
                this.parentChange(parent);
            });
            return parent;
        }
        else {
            notifyError("Can't find cascaded parent element with ID: " + this._parentID + '!', '', null);
            return null;
        }
    }

    /**
     * Unbinds the link from the parent widget.
     * @returns The parent node, or null.
     */
    unbind(): HTMLElement | null {

        if (!this._parentID) {
            return null;
        }

        const parentNode = this._parentNode ??
            (this._parentID ? findElementWithRelativeId(this.widget.domNode, this._parentID) : null);

        this._parentNode = null;

        if (parentNode != null) {
            Fluent.off(parentNode, '.' + (this.widget as any).uniqueName);
        }

        return parentNode;
    }

    /**
     * Returns the parent element id.
     * @returns The parent id.
     */
    get_parentID() {
        return this._parentID;
    }

    /**
     * Sets the parent element id and rebinds the link.
     * @param value - The parent id.
     */
    set_parentID(value: string) {

        if (this._parentID !== value) {
            this.unbind();
            this._parentID = value;
            this.bind();
        }
    }
}
