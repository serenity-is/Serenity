import { DialogTexts, faIcon, getjQuery, nsSerenity } from "../../base";
import { Widget, WidgetProps } from "../widgets/widget";

export namespace DialogExtensions {

    export function dialogResizable(dialog: HTMLElement | ArrayLike<HTMLElement>, w?: any, h?: any, mw?: any, mh?: any): void {
        let $ = getjQuery();
        if (!$)
            return;
        const dlg = $(dialog)?.dialog?.();
        if (!dlg)
            return;
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
    }

    export function dialogMaximizable(dialog: HTMLElement | ArrayLike<HTMLElement>): void {
        if (!getjQuery())
            return;
        new UIDialogMaximizer({
            element: dialog
        });
    }
}

export interface UIDialogMaximizerProps {
    dblclick?: boolean;
    showButton?: boolean;
}

/**
 * Adds maximize / restore functionality to a jQuery UI dialog.
 * Ported from the jquery.dialogextend plugin, converted from a jQuery UI widget
 * into a plain class. Requires jQuery UI dialogs; it throws an error without them.
 */
export class UIDialogMaximizer extends Widget<UIDialogMaximizerProps> {
    static override [Symbol.typeInfo] = this.registerClass(nsSerenity);

    static readonly defaults: UIDialogMaximizerProps = {
        dblclick: true,
        showButton: true
    };

    private maximized: boolean;
    private snapshot: any;

    constructor(props: WidgetProps<UIDialogMaximizerProps>) {
        super({ ...UIDialogMaximizer.defaults, ...props });
        const $ = getjQuery();
        if (!$)
            throw new Error("DialogMaximizer requires jQuery!");
        this.addButton("maximize", this.options.showButton, DialogTexts.MaximizeHint, faIcon("window-maximize"), () => this.maximize());
        this.addButton("restore", false, DialogTexts.RestoreHint, faIcon("window-restore"), () => this.restore());
        const titlebar = $(this.domNode).closest('.ui-dialog').children('.ui-dialog-titlebar');
        titlebar.dblclick(() => {
            if (this.options.dblclick) {
                return this.maximized ? this.restore() : this.maximize();
            }
        }).select(() => false);
    }

    private addButton(name: string, show: boolean, hint: string, icon: string, click: () => void): void {
        const $ = getjQuery();
        const titlebar = $(this.domNode).closest('.ui-dialog').children('.ui-dialog-titlebar');
        const closeButton = titlebar.find('.ui-dialog-titlebar-close').first();
        const button = $('<button class="ui-button ui-corner-all ui-button-icon-only ui-dialog-titlebar-'
            + name + '" tabindex="-1"><i class="' + icon + '"></i></button>')
            .attr('title', hint)
            .toggle(show)
            .click((e: any) => {
                e.preventDefault();
                return click();
            });

        if (closeButton)
            button.insertBefore(closeButton);
        else
            button.appendTo(titlebar);
    }

    /** Returns the current state, e.g. "normal" or "maximized" */
    get isMaximized(): boolean {
        return !!this.maximized;
    }

    private setMaximized(value: boolean) {
        this.maximized = !!value;
        const uiDialog = getjQuery()(this.domNode).closest('.ui-dialog');
        uiDialog.toggleClass("ui-dialog-maximized", this.maximized);
        uiDialog.find(".ui-dialog-titlebar-restore").toggle(this.maximized && this.options.showButton);
        uiDialog.find(".ui-dialog-titlebar-maximize").toggle(!this.maximized && this.options.showButton);
        this.snapshot?.config?.resizable && uiDialog.triggerHandler("resize");
    }

    maximize(): void {
        let $ = getjQuery();
        let newHeight = $(window).height() - 1;
        let newWidth = $(window).width() - 1;
        this.maximized ? this.restoreSnapshot() : this.saveSnapshot();
        let el = $(this.domNode);
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
        this.setMaximized(true);
    }

    restore(): void {
        this.restoreSnapshot();
        this.setMaximized(false);
    }
    
    private restoreSnapshot(): void {
        if (!this.maximized)
            return;

        const el = getjQuery()(this.domNode);
        const original = this.snapshot || { config: {}, size: {}, position: {}, titlebar: {} };
        const widget = el.dialog("widget");
        widget.css("position", original.position.mode).find(".ui-dialog-titlebar").css("white-space", original.titlebar.wrap).end().find(".ui-dialog-content").dialog("option", {
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
            widget.draggable("option", "handle", widget.find(".ui-dialog-draggable-handle").length ?
                widget.find(".ui-dialog-draggable-handle") : ".ui-dialog-titlebar").find(".ui-dialog-draggable-handle").css("cursor", "move");
        }
    }

    private saveSnapshot(): void {
        const $ = getjQuery();
        const el = $(this.domNode);
        const widget = el.dialog("widget");
        this.snapshot = {
            config: {
                resizable: el.dialog("option", "resizable"),
                draggable: el.dialog("option", "draggable"),
            },
            size: {
                height: widget.outerHeight(),
                width: el.dialog("option", "width"),
                maxHeight: el.dialog("option", "maxHeight")
            },
            position: {
                mode: widget.css("position"),
                left: widget.offset().left - $('body').scrollLeft(),
                top: widget.offset().top - $('body').scrollTop()
            },
            titlebar: {
                wrap: widget.find(".ui-dialog-titlebar").css("white-space")
            }
        };
    }
}