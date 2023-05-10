declare global {
    interface JQuery {
        getWidget<TWidget>(widgetType: { new (...args: any[]): TWidget }): TWidget;
        tryGetWidget<TWidget>(widgetType: { new (...args: any[]): TWidget }): TWidget;
        flexHeightOnly(flexY?: number): JQuery;
        flexWidthOnly(flexX?: number): JQuery;
        flexWidthHeight(flexX: number, flexY: number): JQuery;
        flexX(flexX: number): JQuery;
        flexY(flexY: number): JQuery;
    }
}

export {}