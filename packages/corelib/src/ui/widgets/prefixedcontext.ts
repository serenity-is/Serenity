import sQuery from "@optionaldeps/squery";

export class PrefixedContext {
    constructor(public readonly idPrefix: string) {
    }

    byId(id: string): JQuery {
        return sQuery('#' + this.idPrefix + id);
    }

    w<TWidget>(id: string, type: { new (...args: any[]): TWidget }): TWidget {
        return sQuery('#' + this.idPrefix + id).getWidget<TWidget>(type);
    }
}