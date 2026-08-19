import { DialogTexts, faIcon, getjQuery, nsSerenity } from "../../base";
import { Widget, WidgetProps } from "../widgets/widget";

/**
 * Helper functions for extending jQuery UI dialogs.
 */
export namespace DialogExtensions {

    /**
     * Makes a jQuery UI dialog resizable and applies optional size constraints.
     * @param dialog - The dialog element or jQuery collection.
     * @param w - Optional width.
     * @param h - Optional height.
     * @param mw - Optional minimum width.
     * @param mh - Optional minimum height.
     */
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

    /**
     * Adds a maximize button to a jQuery UI dialog.
     * @param dialog - The dialog element or jQuery collection.
     */
    export function dialogMaximizable(dialog: HTMLElement | ArrayLike<HTMLElement>): void {
        if (!getjQuery())
            return;
        new UIDialogMaximizer({
            element: dialog
        });
    }
}

/**
 * Options for the {@link UIDialogMaximizer} widget.
 */
export interface UIDialogMaximizerProps {
    /** Whether double-clicking the title bar toggles maximize. */
    dblclick?: boolean;
    /** Whether to show the maximize/restore buttons. */
    showButton?: boolean;
}

/**
 * Adds maximize / restore functionality to a jQuery UI dialog.
 * Ported from the jquery.dialogextend plugin, converted from a jQuery UI widget
 * into a plain class. Requires jQuery UI dialogs; it throws an error without them.
 */
export class UIDialogMaximizer extends Widget<UIDialogMaximizerProps> {
    static override [Symbol.typeInfo] = this.registerClass(nsSerenity);

    /** Default options for the maximizer. */
    static readonly defaults: UIDialogMaximizerProps = {
        dblclick: true,
        showButton: true
    };

    private maximized: boolean;
    private snapshot: any;

    /**
     * Creates a maximizer for the dialog containing the given element.
     * @param props - Widget props including the dialog element.
     */
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

        if (closeButton.length)
            button.insertBefore(closeButton);
        else
            button.appendTo(titlebar);
    }

    /**
     * Returns the current state, e.g. "normal" or "maximized".
     * @returns True when the dialog is maximized.
     */
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

    /**
     * Maximizes the dialog to fill the window.
     */
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

    /**
     * Restores the dialog to its previous size and position.
     */
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