import { Fluent } from "../../base";
import { getWidgetFrom } from "./widgetutils";

export class PrefixedContext {
    constructor(public readonly idPrefix: string) {
    }

    byId(id: string): Fluent {
        return Fluent(document.querySelector('#' + this.idPrefix + id));
    }

    w<TWidget>(id: string, type: { new (...args: any[]): TWidget }): TWidget {
        return getWidgetFrom<TWidget>('#' + this.idPrefix + id, type);
    }
}