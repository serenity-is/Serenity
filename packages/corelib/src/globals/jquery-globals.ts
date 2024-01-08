declare global {
    interface JQuery {
        getWidget<TWidget>(widgetType: { new (...args: any[]): TWidget }): TWidget;
        tryGetWidget<TWidget>(widgetType?: { new (...args: any[]): TWidget }): TWidget;
        validate(...args: any[]): any;
        valid(): boolean;
    }
}

export {}