export {}

export class PrefixedContext {
    constructor(public readonly idPrefix: string) {
    }

    byId(id: string): JQuery {
        return $('#' + this.idPrefix + id);
    }

    w<TWidget>(id: string, type: { new (...args: any[]): TWidget }): TWidget {
        return $('#' + this.idPrefix + id).getWidget<TWidget>(type);
    }
}