import { Fluent, IconClassName, addClass, getjQuery, htmlEncode, iconClassName, isArrayLike } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { Widget, WidgetProps } from "./widget";

export interface ToolButton {
    action?: string;
    title?: string;
    hint?: string;
    cssClass?: string;
    icon?: IconClassName;
    onClick?: any;
    htmlEncode?: any;
    hotkey?: string;
    hotkeyAllowDefault?: boolean;
    hotkeyContext?: any;
    separator?: (false | true | 'left' | 'right' | 'both');
    visible?: boolean | (() => boolean);
    disabled?: boolean | (() => boolean);
}

export interface PopupMenuButtonOptions {
    menu?: HTMLElement | ArrayLike<HTMLElement>;
    onPopup?: () => void;
    positionMy?: string;
    positionAt?: string;
}

@Decorators.registerEditor('Serenity.PopupMenuButton')
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
        if (menu) {
            let $ = getjQuery();
            $ ? $(menu).remove() : menu.remove();
        }
        super.destroy();
    }
}


export interface PopupToolButtonOptions extends PopupMenuButtonOptions {
}

@Decorators.registerEditor('Serenity.PopupToolButton')
export class PopupToolButton<P extends PopupToolButtonOptions = PopupToolButtonOptions> extends PopupMenuButton<P> {
    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add('s-PopupToolButton');
        this.domNode.querySelector(":scope > .button-outer > span").appendChild(document.createElement("b"));
    }
}

export interface ToolbarOptions {
    buttons?: ToolButton[];
    hotkeyContext?: any;
}

@Decorators.registerClass('Serenity.Toolbar')
@Decorators.element("<div/>")
export class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {
    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add("s-Toolbar");
        this.domNode.classList.add("clearfix");
        this.domNode.innerHTML = '<div class="tool-buttons"><div class="buttons-outer"><div class="buttons-inner"></div></div></div>';

        this.createButtons();
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

    protected createButtons() {
        var containers = this.domNode.querySelectorAll<HTMLElement>('div.buttons-inner');
        var container = containers[containers.length - 1];
        var buttons = this.options.buttons || [];
        var currentCount = 0;
        for (var i = 0; i < buttons.length; i++) {
            var button = buttons[i];
            if (button.separator && currentCount > 0) {
                let parent = container.parentElement;
                container = document.createElement("div");
                container.classList.add("buttons-inner");
                parent.append(container);
                currentCount = 0;
            }
            this.createButton(container, button);
            currentCount++;
        }
    }

    protected createButton(container: HTMLElement, tb: ToolButton) {
        var cssClass = tb.cssClass ?? '';

        let btn = document.createElement("div");
        btn.classList.add("tool-button");
        let outer = btn.appendChild(document.createElement("div"));
        outer.classList.add("button-outer");
        let inner = outer.appendChild(document.createElement("div"));
        inner.classList.add("button-inner");

        if (tb.action != null)
            btn.setAttribute('data-action', tb.action);

        if (tb.separator === 'right' || tb.separator === 'both') {
            let sep = container.appendChild(document.createElement("div"));
            sep.classList.add("separator");
        }

        if (cssClass.length > 0)
            addClass(btn, cssClass);

        if (tb.hint)
            btn.setAttribute('title', tb.hint);

        Fluent.on(btn, "click", e => {
            if (btn.classList.contains('disabled'))
                return;
            tb.onClick(e);
        });

        var text = tb.title;
        if (tb.htmlEncode !== false) {
            text = htmlEncode(tb.title);
        }

        if (tb.icon) {
            btn.classList.add('icon-tool-button');
            text = "<i class='" + htmlEncode(iconClassName(tb.icon)) + "'></i> " + text;
        }
        if (text == null || text.length === 0) {
            btn.classList.add('no-text');
        }
        else {
            let span = btn.querySelector('span');
            span && (span.innerHTML = text);
        }

        if (tb.visible === false)
            btn.style.display = "none";

        if (tb.disabled != null && typeof tb.disabled !== "function")
            btn.classList.toggle('disabled', !!tb.disabled);

        if (typeof tb.visible === "function" || typeof tb.disabled == "function") {
            Fluent.on(btn, 'updateInterface', () => {
                if (typeof tb.visible === "function")
                    btn.style.display = !tb.visible() ? "none" : "";

                if (typeof tb.disabled === "function")
                    btn.classList.toggle("disabled", !!tb.disabled());
            });
        }

        if (tb.hotkey && window['Mousetrap' as any] != null) {
            this.mouseTrap = this.mouseTrap || (window['Mousetrap' as any] as any)(
                tb.hotkeyContext || this.options.hotkeyContext || window.document.documentElement);

            this.mouseTrap.bind(tb.hotkey, function () {
                if (btn.style.display != "none") {
                    Fluent.trigger(btn, "click");
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

export interface ToolButtonInstance {
    element: HTMLElement,
    hide(): this,
    show(): this,
    toggle(value?: boolean): this,
    toggleClass(klass: string): this
}