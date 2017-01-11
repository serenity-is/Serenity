declare namespace Serenity {

    namespace WX {
        function getWidget<TWidget>(element: JQuery, type: Function): any;
        function tryGetWidget(element: JQuery): any;
        function getWidgetName(type: Function): string;
        function hasOriginalEvent(e: any): boolean;
        function change(widget: any, handler: any): void;
        function changeSelect2(widget: any, handler: any): void;
        function getGridField(widget: Serenity.Widget<any>): JQuery;
        function create(initElement: (p1: JQuery) => void, options?: any): any;
    }
}