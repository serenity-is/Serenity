import { nsSerenity, type PropertyItem } from "../../base";
import { IGetEditValue, ISetEditValue } from "../../interfaces";
import { Widget } from "../widgets/widget";

/**
 * Stands in for a quick filter widget while its type is loading, so that the quick filter can be found, saved and
 * restored, and list requests include its value. The value is transferred to the actual widget that replaces it.
 */
export class QuickFilterPlaceholder extends Widget<{}> implements IGetEditValue, ISetEditValue {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [IGetEditValue, ISetEditValue]);

    static override createDefaultElement(): HTMLElement {
        const input = document.createElement("input");
        input.type = "hidden";
        return input;
    }

    /** The value that will be transferred to the actual widget. */
    declare public value: any;

    getEditValue(property: PropertyItem, target: any) {
        target[property.name] = this.value;
    }

    setEditValue(source: any, property: PropertyItem) {
        this.value = source[property.name];
    }
}
