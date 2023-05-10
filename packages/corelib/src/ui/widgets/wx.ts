import { Widget } from "./widget";

export namespace WX {
    export function getWidget<TWidget>(element: JQuery, type: any) {
        return element.getWidget<TWidget>(type);
    }

    export var getWidgetName = Widget.getWidgetName;

    export function hasOriginalEvent(e: any): boolean {
        return !!!(typeof (e.originalEvent) === 'undefined');
    }

    export function change(widget: any, handler: any): void {
        widget.change(handler)
    }
    
    export function changeSelect2(widget: any, handler: any): void {
        widget.changeSelect2(handler);
    }

    export function getGridField(widget: Widget<any>): JQuery {
        return widget.getGridField();
    }
}

