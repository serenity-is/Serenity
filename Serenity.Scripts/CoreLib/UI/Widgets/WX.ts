namespace Serenity {

    export namespace WX {
        export function getWidget<TWidget>(element: JQuery, type: Function): any {
            if (element == null) {
                throw new Q.ArgumentNullException('element');
            }
            if (element.length === 0) {
                throw new Q.Exception(Q.format("Searching for widget of type '{0}' on a non-existent element! ({1})",
                    Q.getTypeFullName(type), element.selector));
            }
            var widget = element.tryGetWidget(type as any);
            if (widget == null) {
                var message = Q.format("Element has no widget of type '{0}'! If you have recently changed editor " +
                    "type of a property in a form class, or changed data type in row (which also changes editor type)" +
                    " your script side Form definition might be out of date. Make sure your project builds successfully " +
                    "and transform T4 templates", Q.getTypeFullName(type));
                Q.notifyError(message, '', null);
                throw new Q.Exception(message);
            }
            return widget;
        }

        export function getWidgetName(type: Function): string {
            return Q.replaceAll(Q.getTypeFullName(type), '.', '_');
        }

        export function hasOriginalEvent(e: any): boolean {
            return !!!(typeof (e.originalEvent) === 'undefined');
        }

        export function change(widget: any, handler: any): void {
            widget.element.bind('change.' + widget.uniqueName, handler);
        }
        
        export function changeSelect2(widget: any, handler: any): void {
            widget.element.bind('change.' + widget.uniqueName, function (e: JQueryEventObject, x: boolean) {
                if (!!(hasOriginalEvent(e) || !x)) {
                    handler(e);
                }
            });
        }

        export function getGridField(widget: Serenity.Widget<any>): JQuery {
            return widget.element.closest('.field');
        }
    }

    Serenity.Widget.prototype['changeSelect2'] = function (handler) {
        var widget = this;
        widget.element.bind('change.' + widget.uniqueName, function (e: JQueryEventObject, x: any) {
            if (!!(WX.hasOriginalEvent(e) || !x)) {
                handler(e);
            }
        });
    };

    Serenity.Widget.prototype['change'] = function (handler1) {
        var widget1 = this;
        widget1.element.bind('change.' + widget1.uniqueName, handler1);
    };

    Serenity.Widget.prototype['getGridField'] = function () {
        return this.element.closest('.field');
    };

    if (typeof $ != "undefined") {
        $.fn.tryGetWidget = function (widgetType: any) {
            var element = this;
            var widget2;
            if (Q.isAssignableFrom(Serenity.Widget, widgetType)) {
                var widgetName = WX.getWidgetName(widgetType);
                widget2 = element.data(widgetName);
                if (widget2 != null && !Q.isAssignableFrom(widgetType, Q.getInstanceType(widget2))) {
                    widget2 = null;
                }
                if (widget2 != null) {
                    return widget2;
                }
            }

            var data = element.data();
            if (data == null) {
                return null;
            }

            for (var key of Object.keys(data)) {
                widget2 = data[key];
                if (widget2 != null && Q.isAssignableFrom(widgetType, Q.getInstanceType(widget2))) {
                    return widget2;
                }
            }

            return null;
        };

        $.fn.getWidget = function (widgetType1: any) {
            var element1 = this;
            if (element1 == null) {
                throw new Q.ArgumentNullException('element');
            }
            if (element1.length === 0) {
                throw new Q.Exception(Q.format("Searching for widget of type '{0}' on a non-existent element! ({1})",
                    Q.getTypeFullName(widgetType1), element1.selector));
            }

            var widget3 = element1.tryGetWidget(widgetType1);
            if (widget3 == null) {
                var message = Q.format("Element has no widget of type '{0}'! If you have recently changed " +
                    "editor type of a property in a form class, or changed data type in row (which also changes " +
                    "editor type) your script side Form definition might be out of date. Make sure your project " +
                    "builds successfully and transform T4 templates", Q.getTypeFullName(widgetType1));
                Q.notifyError(message, '', null);
                throw new Q.Exception(message);
            }
            return widget3;
        };
    }
}

