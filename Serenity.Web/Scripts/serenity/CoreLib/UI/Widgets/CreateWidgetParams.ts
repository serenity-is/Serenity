namespace Serenity {
    export interface CreateWidgetParams<TWidget extends Widget<TOptions>, TOptions> {
        type?: new (element: JQuery, options?: TOptions) => TWidget;
        options?: TOptions;
        container?: JQuery;
        element?: (e: JQuery) => void;
        init?: (w: TWidget) => void;
    }
}