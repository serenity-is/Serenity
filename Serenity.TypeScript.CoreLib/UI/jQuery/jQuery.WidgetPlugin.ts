interface JQuery {
    getWidget<TWidget>(widgetType: { new (...args: any[]): TWidget }): TWidget;
    tryGetWidget<TWidget>(widgetType: { new (...args: any[]): TWidget }): TWidget;
}