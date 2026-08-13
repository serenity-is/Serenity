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
            element: dialog,
            dblclick: 'maximize'
        });
    }
}

export interface UIDialogMaximizerProps {
    dblclick?: boolean | string;
    load?: (() => void) | null;
    beforeRestore?: (() => void) | null;
    restore?: (() => void) | null;
    showButton?: boolean;
    beforeMaximize?: (() => void) | null;
    maximize?: (() => void) | null;
}

/**
 * Adds maximize / restore functionality to a jQuery UI dialog.
 * Ported from the jquery.dialogextend plugin, converted from a jQuery UI widget
 * into a plain class. Requires jQuery UI dialogs; it throws an error without them.
 */
export class UIDialogMaximizer extends Widget<UIDialogMaximizerProps> {
    static override [Symbol.typeInfo] = this.registerClass(nsSerenity);

    static readonly defaults: Required<UIDialogMaximizerProps> = {
        dblclick: true,
        load: null,
        beforeRestore: null,
        restore: null,
        showButton: true,
        beforeMaximize: null,
        maximize: null
    };

    private currentState = "normal";
    private snapshot: any;
    private originalConfigResizable: boolean | undefined;

    constructor(props: WidgetProps<UIDialogMaximizerProps>) {
        super({ ...UIDialogMaximizer.defaults, ...props });
        if (!getjQuery())
            throw new Error("DialogMaximizer requires jQuery!");
        this.initButtons();
        this.setState("normal");
        this.options.load?.();
    }

    /** Returns the current state, e.g. "normal" or "maximized" */
    state(): string {
        return this.currentState;
    }

    maximize(): void {
        let $ = getjQuery();
        let newHeight = $(window).height() - 1;
        let newWidth = $(window).width() - 1;
        this.options.beforeMaximize?.();
        if (this.currentState !== "normal") {
            this.restorePreviousState();
        }
        this.saveSnapshot();
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
        this.setState("maximized");
        this.toggleButtons();

        if (this.originalConfigResizable)
            $(this.domNode).closest('.ui-dialog').triggerHandler("resize");

        this.options.maximize?.();
    }

    restore(): void {
        this.options.beforeRestore?.();
        this.restorePreviousState();
        this.setState("normal");
        this.toggleButtons();

        if (this.originalConfigResizable)
            getjQuery()(this.domNode).closest('.ui-dialog').triggerHandler("resize");

        this.options.restore?.();
    }

    private setState(state: string): string {
        getjQuery()(this.domNode).removeClass("ui-dialog-" + this.currentState).addClass("ui-dialog-" + state);
        return this.currentState = state;
    }

    private initButtons(): void {
        this.addButton("maximize", this.options.showButton, DialogTexts.MaximizeHint, faIcon("window-maximize"));
        this.addButton("restore", false, DialogTexts.RestoreHint, faIcon("window-restore"));

        const titlebar = getjQuery()(this.domNode).closest('.ui-dialog').children('.ui-dialog-titlebar');
        titlebar.dblclick(() => {
            if (this.options.dblclick) {
                if (this.currentState !== "normal") {
                    return this.restore();
                } else {
                    return (this as any)[this.options.dblclick as string]();
                }
            }
        }).select(() => {
            return false;
        });
    }

    private addButton(name: string, show: boolean, hint: string, icon: string): void {
        const titlebar = getjQuery()(this.domNode).closest('.ui-dialog').children('.ui-dialog-titlebar');
        const closeButton = titlebar.find('.ui-dialog-titlebar-close').first();
        const button = getjQuery()('<button class="ui-button ui-corner-all ui-button-icon-only ui-dialog-titlebar-'
            + name + '" tabindex="-1"><i class="' + icon + '"></i></button>')
            .attr('title', hint)
            .toggle(show)
            .click((e: any) => {
                e.preventDefault();
                return (this as any)[name]();
            });

        if (closeButton)
            button.insertBefore(closeButton);
        else
            button.appendTo(titlebar);
    }

    private restoreMaximized(): void {
        const el = getjQuery()(this.domNode);
        const original = this.snapshot || { config: {}, size: {}, position: {}, titlebar: {} };
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
            el.dialog("widget").draggable("option", "handle", el.dialog("widget").find(".ui-dialog-draggable-handle").length ?
                el.dialog("widget").find(".ui-dialog-draggable-handle") : ".ui-dialog-titlebar").find(".ui-dialog-draggable-handle").css("cursor", "move");
        }
    }

    private restorePreviousState(): void {
        if (this.currentState === "maximized") {
            this.restoreMaximized();
        }
    }

    private saveSnapshot(): void {
        if (this.currentState === "normal") {
            const el = getjQuery()(this.domNode);
            this.snapshot = {
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
                    left: el.dialog("widget").offset().left - getjQuery()('body').scrollLeft(),
                    top: el.dialog("widget").offset().top - getjQuery()('body').scrollTop()
                },
                titlebar: {
                    wrap: el.dialog("widget").find(".ui-dialog-titlebar").css("white-space")
                }
            };
            this.originalConfigResizable = this.snapshot.config.resizable;
        }
    }

    private toggleButtons(): void {
        const uiDialog = getjQuery()(this.domNode).closest('.ui-dialog');
        uiDialog.find(".ui-dialog-titlebar-restore").toggle(this.currentState !== "normal" && this.options.showButton);
        uiDialog.find(".ui-dialog-titlebar-maximize").toggle(this.currentState !== "maximized" && this.options.showButton);
    }
}