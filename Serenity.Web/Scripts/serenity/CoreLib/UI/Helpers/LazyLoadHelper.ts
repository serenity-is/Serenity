namespace Serenity {
    export namespace LazyLoadHelper {
        let autoIncrement = 0;

        export function executeOnceWhenShown(element: JQuery, callback: Function) {
            autoIncrement++;
            var eventClass = 'ExecuteOnceWhenShown' + autoIncrement;
            var executed = false;
            if (element.is(':visible')) {
                callback();
            }
            else {
                var uiTabs = element.closest('.ui-tabs');
                if (uiTabs.length > 0) {
                    uiTabs.bind('tabsshow.' + eventClass, function (e) {
                        if (element.is(':visible')) {
                            uiTabs.unbind('tabsshow.' + eventClass);
                            if (!executed) {
                                executed = true;
                                element.unbind('shown.' + eventClass);
                                callback();
                            }
                        }
                    });
                }
                var dialog: JQuery;
                if (element.hasClass('ui-dialog')) {
                    dialog = element.children('.ui-dialog-content');
                }
                else {
                    dialog = element.closest('.ui-dialog-content');
                }
                if (dialog.length > 0) {
                    dialog.bind('dialogopen.' + eventClass, function () {
                        dialog.unbind('dialogopen.' + eventClass);
                        if (element.is(':visible') && !executed) {
                            executed = true;
                            element.unbind('shown.' + eventClass);
                            callback();
                        }
                    });
                }
                element.bind('shown.' + eventClass, function () {
                    if (element.is(':visible')) {
                        element.unbind('shown.' + eventClass);
                        if (!executed) {
                            executed = true;
                            callback();
                        }
                    }
                });
            }
        }

        export function executeEverytimeWhenShown(element: JQuery, callback: Function, callNowIfVisible: boolean) {
            autoIncrement++;
            var eventClass = 'ExecuteEverytimeWhenShown' + autoIncrement;
            var wasVisible = element.is(':visible');

            if (wasVisible && callNowIfVisible) {
                callback();
            }

            var check = function (e: any) {
                if (element.is(':visible')) {
                    if (!wasVisible) {
                        wasVisible = true;
                        callback();
                    }
                }
                else {
                    wasVisible = false;
                }
            };

            var uiTabs = element.closest('.ui-tabs');
            if (uiTabs.length > 0) {
                uiTabs.bind('tabsactivate.' + eventClass, check);
            }

            var dialog = element.closest('.ui-dialog-content');
            if (dialog.length > 0) {
                dialog.bind('dialogopen.' + eventClass, check);
            }

            element.bind('shown.' + eventClass, check);
        }
    }
}