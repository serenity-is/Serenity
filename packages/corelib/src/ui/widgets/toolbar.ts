import { Fluent, IconClassName, getjQuery, htmlEncode, iconClassName, isArrayLike } from "@serenity-is/base";
import { Decorators } from "../../types/decorators";
import { Widget, WidgetProps } from "./widget";

export interface ToolButtonProps {
    action?: string;
    title?: string;
    hint?: string;
    cssClass?: string;
    icon?: IconClassName;
    onClick?: any;
    hotkey?: string;
    hotkeyAllowDefault?: boolean;
    hotkeyContext?: any;
    separator?: (false | true | 'left' | 'right' | 'both');
    visible?: boolean | (() => boolean);
    disabled?: boolean | (() => boolean);
}


export interface ToolButton extends ToolButtonProps {
    hotkey?: string;
    hotkeyAllowDefault?: boolean;
    hotkeyContext?: any;
    separator?: (false | true | 'left' | 'right' | 'both');
}

export function ToolbarButton(tb: ToolButtonProps): HTMLElement {
    var cssClass = tb.cssClass ?? '';

    let span = Fluent("span").addClass("button-inner");
    let btn = Fluent("div")
        .addClass("tool-button")
        .append(span);

    if (tb.action != null)
        btn.attr('data-action', tb.action);

    if (cssClass.length > 0)
        btn.addClass(cssClass);

    if (tb.hint)
        btn.attr('title', tb.hint);

    btn.on("click", e => {
        if (btn.hasClass('disabled'))
            return;
        tb.onClick(e);
    });

    if (tb.icon) {
        btn.addClass('icon-tool-button');
        span.append(Fluent("i").addClass(iconClassName(tb.icon)));
        tb.title && span.append(" ").append(tb.title);
    }
    else if (tb.title)
        span.append(tb.title);
    
    if (!tb.title)
        btn.addClass('no-text');

    if (tb.visible === false)
        btn.getNode().style.display = "none";

    if (tb.disabled != null && typeof tb.disabled !== "function")
        btn.toggleClass('disabled', !!tb.disabled);

    if (typeof tb.visible === "function" || typeof tb.disabled == "function") {
        btn.on('updateInterface', () => {
            if (typeof tb.visible === "function")
                btn.toggle(tb.visible());

            if (typeof tb.disabled === "function")
                btn.toggleClass("disabled", !!tb.disabled());
        });
    }

    return btn.getNode();
}

export interface PopupMenuButtonOptions {
    menu?: HTMLElement | ArrayLike<HTMLElement>;
    onPopup?: () => void;
    positionMy?: string;
    positionAt?: string;
}

export class PopupMenuButton<P extends PopupMenuButtonOptions = PopupMenuButtonOptions> extends Widget<P> {
    constructor(props: WidgetProps<P>) {
        super(props);

        let div = this.domNode;
        div.classList.add('s-PopupMenuButton');
        Fluent.on(div, 'click', e => {
            e.preventDefault();
            e.stopPropagation();
            if (this.options.onPopup != null) {
                this.options.onPopup();
            }

            let $ = getjQuery();
            if (!$)
                return;
            var menu = $(this.options.menu);
            (menu.show() as any).position?.({
                my: this.options.positionMy ?? 'left top',
                at: this.options.positionAt ?? 'left bottom',
                of: div
            });

            var uq = this.uniqueName;
            Fluent.one(document, 'click.' + uq, function (x) {
                menu.hide();
            });
        });

        // TODO: Convert to bootstrap dropdown
        let menu = isArrayLike(this.options.menu) ? this.options.menu[0] : this.options.menu;
        menu.style.display = 'none';
        menu.classList.add('s-PopupMenu');
        document.body.append(menu);
        let $ = getjQuery();
        $ && $(menu).menu?.();
    }

    destroy() {
        let menu = isArrayLike(this.options.menu) ? this.options.menu[0] : this.options.menu;
        Fluent(menu).remove();
        super.destroy();
    }
}

export interface PopupToolButtonOptions extends PopupMenuButtonOptions {
}

export class PopupToolButton<P extends PopupToolButtonOptions = PopupToolButtonOptions> extends PopupMenuButton<P> {
    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add('s-PopupToolButton');
        this.domNode.querySelector(".button-inner")?.appendChild(document.createElement("b"));
    }
}

export interface ToolbarOptions {
    buttons?: ToolButton[];
    hotkeyContext?: any;
}

@Decorators.registerClass('Serenity.Toolbar')
export class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {

    protected renderContents() {

        let group = Fluent("div").addClass("tool-group");

        Fluent(this.domNode)
            .addClass("s-Toolbar clearfix")
            .append(group);

        var buttons = this.options.buttons || [];
        var currentCount = 0;
        for (var i = 0; i < buttons.length; i++) {
            var button = buttons[i];
            if (button.separator && currentCount > 0) {
                group = Fluent("div")
                    .addClass("tool-group")
                    .appendTo(group.parent());
                currentCount = 0;
            }
            this.createButton(group, button);
            currentCount++;
        }

        return group.getNode();
    }

    destroy() {
        this.domNode.querySelectorAll('div.tool-button').forEach(el => Fluent.off(el, 'click'));
        if (this.mouseTrap) {
            if (!!this.mouseTrap.destroy) {
                this.mouseTrap.destroy();
            }
            else {
                this.mouseTrap.reset();
            }
            this.mouseTrap = null;
        }

        super.destroy();
    }

    protected mouseTrap: any;

    createButton(container: Fluent, tb: ToolButton) {

        if (tb.separator === 'right' || tb.separator === 'both') {
            container.append(Fluent("div")).addClass("separator");
        }

        let button = ToolbarButton(tb);
        container.append(button);

        if (tb.hotkey && window['Mousetrap' as any] != null) {
            this.mouseTrap = this.mouseTrap || (window['Mousetrap' as any] as any)(
                tb.hotkeyContext || this.options.hotkeyContext || window.document.documentElement);

            this.mouseTrap.bind(tb.hotkey, function () {
                if (button.style.display != "none") {
                    Fluent.trigger(button, "click");
                }
                return tb.hotkeyAllowDefault;
            });
        }
    }

    findButton(className: string) {
        if (className != null && className.startsWith('.')) {
            className = className.substring(1);
        }

        return Fluent(this.domNode.querySelector<HTMLElement>('div.tool-button.' + className));
    }

    updateInterface() {
        this.domNode.querySelectorAll('.tool-button').forEach(function (el: Element) {
            Fluent.trigger(el, 'updateInterface', { bubbles: false });
        });
    }
}