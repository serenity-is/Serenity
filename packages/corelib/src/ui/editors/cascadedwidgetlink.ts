import { addDisposingListener } from "@serenity-is/sleekdom";
import { classTypeInfo, Fluent, notifyError, nsSerenity, registerType } from "../../base";
import { findElementWithRelativeId } from "../../compat";
import { Widget } from "../widgets/widget";
import { tryGetWidget } from "../widgets/widgetutils";

export class CascadedWidgetLink<TParent extends Widget<any>> {

    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    constructor(private parentType: { new(...args: any[]): TParent },
        private widget: Widget<any>,
        private parentChange: (p1: TParent) => void) {
        this.bind();
        addDisposingListener(this.widget.domNode, () => {
            this.unbind();
            this.widget = null;
            this.parentChange = null;
        }, this.widget.uniqueName);
    }

    declare private _parentID: string;

    bind() {

        if (!this._parentID) {
            return null;
        }

        var parent = tryGetWidget(findElementWithRelativeId(this.widget.domNode, this._parentID), this.parentType);

        if (parent != null) {
            Fluent.on(parent.domNode, 'change.' + (this.widget as any).uniqueName, () => {
                this.parentChange(parent);
            });
            return parent;
        }
        else {
            notifyError("Can't find cascaded parent element with ID: " + this._parentID + '!', '', null);
            return null;
        }
    }

    unbind() {

        if (!this._parentID) {
            return null;
        }

        var parent = tryGetWidget(findElementWithRelativeId(this.widget.domNode, this._parentID), this.parentType);

        if (parent != null) {
            Fluent.off(parent.domNode, '.' + (this.widget as any).uniqueName);
        }

        return parent;
    }

    get_parentID() {
        return this._parentID;
    }

    set_parentID(value: string) {

        if (this._parentID !== value) {
            this.unbind();
            this._parentID = value;
            this.bind();
        }
    }
}
