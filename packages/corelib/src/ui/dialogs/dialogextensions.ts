import { localText } from "@serenity-is/base";
import { Flexify } from "../widgets/flexify";

export namespace DialogExtensions {

    export function dialogResizable(dialog: JQuery, w?: any, h?: any, mw?: any, mh?: any): JQuery {
        var dlg = (dialog as any)?.dialog?.();
        dlg.dialog('option', 'resizable', true);
        if (mw != null) {
            dlg.dialog('option', 'minWidth', mw);
        }
        if (w != null) {
            dlg.dialog('option', 'width', w);
        }
        if (mh != null) {
            dlg.dialog('option', 'minHeight', mh);
        }
        if (h != null) {
            dlg.dialog('option', 'height', h);
        }
        return dialog;
    }

    export function dialogMaximizable(dialog: JQuery): JQuery {
        if (!dialog.length || typeof jQuery === "undefined" || !registerDialogExtendPlugin(jQuery))
            return dialog;
        (dialog as any).dialogExtend({
            closable: true,
            maximizable: true,
            dblclick: 'maximize',
            icons: { maximize: 'ui-icon-maximize-window' }
        });

        return dialog;
    }

    export function dialogFlexify(dialog: JQuery): JQuery {
        new Flexify(dialog.closest('.ui-dialog'), {});
        return dialog;
    }
}

function registerDialogExtendPlugin($: JQueryStatic) {
    if (!$ || !($ as any).widget)
        return false;

    if (($ as any).fn?.dialogExtend)
        return true;

    ($ as any).widget("ui.dialogExtend", {
        options: {
            dblclick: true,
            load: null,
            beforeRestore: null,
            restore: null,
            maximizable: false,
            beforeMaximize: null,
            maximize: null
        },
        _create: function () {
            this._state = "normal";
            this._initButtons();
            this._setState("normal");
            return this._trigger("load");
        },
        _setState: function (state: any) {
            this.element.removeClass("ui-dialog-" + this._state).addClass("ui-dialog-" + state);
            return this._state = state;
        },
        _initButtons: function () {
            var _this = this;

            this._addButton("maximize", this.options.maximizable, localText('Dialogs.MaximizeHint'), "fa fa-window-maximize");
            this._addButton("restore", false, localText('Dialogs.RestoreHint'), "fa fa-window-restore");

            var titlebar = this.element.closest('.ui-dialog').children('.ui-dialog-titlebar');
            titlebar.dblclick(function () {
                if (_this.options.dblclick) {
                    if (_this._state !== "normal") {
                        return _this.restore();
                    } else {
                        return _this[_this.options.dblclick]();
                    }
                }
            }).select(function () {
                return false;
            });
        },
        _addButton: function (name: string, show: boolean, hint: string, icon: string) {
            var _this = this;

            var titlebar = this.element.closest('.ui-dialog').children('.ui-dialog-titlebar');
            var closeButton = titlebar.find('.ui-dialog-titlebar-close').first();
            var button = $('<button class="ui-button ui-corner-all ui-button-icon-only ui-dialog-titlebar-'
                + name + '" href="javascript:;" tabindex="-1"><i class="' + icon + '"></i></a>')
                .attr('title', hint)
                .toggle(show)
                .click(function (e) {
                    e.preventDefault();
                    return _this[name]();
                });

            if (closeButton)
                button.insertBefore(closeButton);
            else
                button.appendTo(titlebar);

            return button;
        },
        maximize: function () {
            var newHeight, newWidth;

            newHeight = $(window).height() - 1;
            newWidth = $(window).width() - 1;
            this._trigger("beforeMaximize");
            if (this._state !== "normal") {
                this._restore();
            }
            this._saveSnapshot();
            var el = $(this.element[0]) as any;
            if (el.dialog("option", "draggable")) {
                el.dialog("widget").draggable("option", "handle", null).find(".ui-dialog-draggable-handle").css("cursor", "text").end();
            }
            el.dialog("widget").css("position", "fixed").find(".ui-dialog-content").show().dialog("widget")
                .find(".ui-dialog-buttonpane").show().end().find(".ui-dialog-content").dialog("option", {
                    resizable: false,
                    draggable: false,
                    height: newHeight,
                    width: newWidth,
                    position: {
                        of: window,
                        my: "left top",
                        at: "left top"
                    }
                });
            this._setState("maximized");
            this._toggleButtons();

            if (this.original_config_resizable)
                this.element.closest('.ui-dialog').triggerHandler("resize");

            return this._trigger("maximize");
        },
        _restore_maximized: function () {
            var el = $(this.element[0]) as any;
            var original = this._snapshot || { config: {}, size: {}, position: {}, titlebar: {} };
            el.dialog("widget").css("position", original.position.mode).find(".ui-dialog-titlebar").css("white-space", original.titlebar.wrap).end().find(".ui-dialog-content").dialog("option", {
                resizable: original.config.resizable,
                draggable: original.config.draggable,
                height: original.size.height,
                width: original.size.width,
                maxHeight: original.size.maxHeight,
                position: {
                    of: window,
                    my: "left top",
                    at: "left+" + original.position.left + " top+" + original.position.top
                }
            });
            if (el.dialog("option", "draggable")) {
                return el.dialog("widget").draggable("option", "handle", el.dialog("widget").find(".ui-dialog-draggable-handle").length ?
                    el.dialog("widget").find(".ui-dialog-draggable-handle") : ".ui-dialog-titlebar").find(".ui-dialog-draggable-handle").css("cursor", "move");
            }
        },
        state: function () {
            return this._state;
        },
        restore: function () {
            this._trigger("beforeRestore");
            this._restore();
            this._setState("normal");
            this._toggleButtons();

            if (this.original_config_resizable)
                this.element.closest('.ui-dialog').triggerHandler("resize");

            return this._trigger("restore");
        },
        _restore: function () {
            if (this._state !== "normal") {
                return this["_restore_" + this._state]();
            }
        },
        _saveSnapshot: function () {
            if (this._state === "normal") {
                var el = $(this.element[0]) as any;
                this._snapshot = {
                    config: {
                        resizable: el.dialog("option", "resizable"),
                        draggable: el.dialog("option", "draggable"),
                    },
                    size: {
                        height: el.dialog("widget").outerHeight(),
                        width: el.dialog("option", "width"),
                        maxHeight: el.dialog("option", "maxHeight")
                    },
                    position: {
                        mode: el.dialog("widget").css("position"),
                        left: el.dialog("widget").offset().left - $('body').scrollLeft(),
                        top: el.dialog("widget").offset().top - $('body').scrollTop()
                    },
                    titlebar: {
                        wrap: el.dialog("widget").find(".ui-dialog-titlebar").css("white-space")
                    }
                }
            }
        },
        _toggleButtons: function () {
            var uiDialog = this.element.closest('.ui-dialog');
            uiDialog.find(".ui-dialog-titlebar-restore").toggle(this._state !== "normal");
            uiDialog.find(".ui-dialog-titlebar-maximize").toggle(this._state !== "maximized");
        }
    });
}

if (typeof jQuery !== "undefined") {
    registerDialogExtendPlugin(jQuery) || jQuery(registerDialogExtendPlugin);
}