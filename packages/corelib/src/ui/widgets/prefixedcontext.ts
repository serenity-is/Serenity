import { Fluent } from "../../base";
import { getWidgetFrom } from "./widgetutils";

export class PrefixedContext {
    public readonly idPrefix: string;
    public readonly context: HTMLElement;

    constructor(prefixOrWidget: string | { idPrefix: string, domNode: HTMLElement }, context?: HTMLElement) {
        if (typeof prefixOrWidget === "string") {
            this.idPrefix = prefixOrWidget;
        } else {
            this.idPrefix = prefixOrWidget.idPrefix;
        }
        this.context = context ?? (typeof prefixOrWidget === "object" ? prefixOrWidget.domNode : void 0);
        this.initialize();
    }

    protected initialize() {
    }

    byId(id: string): Fluent {
        return Fluent((this.context ?? document).querySelector('#' + this.idPrefix + id));
    }

    w<TWidget>(id: string, type: { new(...args: any[]): TWidget }): TWidget {
        return getWidgetFrom<TWidget>('#' + this.idPrefix + id, type, this.context);
    }
}