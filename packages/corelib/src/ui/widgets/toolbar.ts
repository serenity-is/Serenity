import { htmlEncode } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { Widget, WidgetComponent, WidgetProps } from "./widget";

export interface ToolButton {
    action?: string;
    title?: string;
    hint?: string;
    cssClass?: string;
    icon?: string;
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
    menu?: JQuery;
    onPopup?: () => void;
    positionMy?: string;
    positionAt?: string;
}

@Decorators.registerEditor('Serenity.PopupMenuButton')
export class PopupMenuButton<P extends PopupMenuButtonOptions = PopupMenuButtonOptions> extends WidgetComponent<P> {
    constructor(props: WidgetProps<P>) {
        super(props);

        let div = this.element;
        div.addClass('s-PopupMenuButton');
        div.click(e => {
            e.preventDefault();
            e.stopPropagation();
            if (this.options.onPopup != null) {
                this.options.onPopup();
            }

            var menu = this.options.menu;
            (menu.show() as any).position?.({
                my: this.options.positionMy ?? 'left top',
                at: this.options.positionAt ?? 'left bottom',
                of: this.element
            });

            var uq = this.uniqueName;
            $(document).one('click.' + uq, function (x) {
                menu.hide();
            });
        });

        (this.options.menu.hide().appendTo(document.body)
            .addClass('s-PopupMenu') as any).menu?.();
    }

    destroy() {
        if (this.options.menu != null) {
            this.options.menu.remove();
            this.options.menu = null;
        }

        super.destroy();
    }
}


export interface PopupToolButtonOptions extends PopupMenuButtonOptions {
}

@Decorators.registerEditor('Serenity.PopupToolButton')
export class PopupToolButton<P extends PopupToolButtonOptions = PopupToolButtonOptions> extends PopupMenuButton<P> {
    constructor(props?: WidgetProps<P>) {
        super(props);

        this.element.addClass('s-PopupToolButton');
        $('<b/>').appendTo(this.element.children('.button-outer').children('span'));
    }
}

export interface ToolbarOptions {
    buttons?: ToolButton[];
    hotkeyContext?: any;
}

@Decorators.registerClass('Serenity.Toolbar')
@Decorators.element("<div/>")
export class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {
    constructor(legacy: ArrayLike<HTMLElement>, opt?: WidgetProps<P>);
    constructor(props?: WidgetProps<P>);
    constructor(props?: any, opt?: any) {
        super(props, opt);
        
        this.element.addClass('s-Toolbar clearfix')
            .html('<div class="tool-buttons"><div class="buttons-outer">' +
                '<div class="buttons-inner"></div></div></div>');

        this.createButtons();
    }

    destroy() {
        this.element.find('div.tool-button').unbind('click');
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
        var container: JQuery = $('div.buttons-inner', this.element).last();
        var buttons = this.options.buttons || [];
        var currentCount = 0;
        for (var i = 0; i < buttons.length; i++) {
            var button = buttons[i];
            if (button.separator && currentCount > 0) {
                container = $('<div class="buttons-inner"></div>').appendTo(container.parent());
                currentCount = 0;
            }
            this.createButton(container, button);
            currentCount++;
        }
    }

    protected createButton(container: JQuery, b: ToolButton) {
        var cssClass = b.cssClass ?? '';

        var btn = $('<div class="tool-button"><div class="button-outer">' +
            '<span class="button-inner"></span></div></div>')
            .appendTo(container);

        if (b.action != null)
            btn.attr('data-action', b.action);

        if (b.separator === 'right' || b.separator === 'both') {
            $('<div class="separator"></div>').appendTo(container);
        }

        if (cssClass.length > 0) {
            btn.addClass(cssClass);
        }

        if (b.hint) {
            btn.attr('title', b.hint);
        }

        btn.click(function (e) {
            if (btn.hasClass('disabled')) {
                return;
            }
            b.onClick(e);
        });

        var text = b.title;
        if (b.htmlEncode !== false) {
            text = htmlEncode(b.title);
        }

        if (b.icon) {
            btn.addClass('icon-tool-button');
            var klass = b.icon;
            if (klass.startsWith('fa-')) {
                klass = 'fa ' + klass;
            }
            else if (klass.startsWith('glyphicon-')) {
                klass = 'glyphicon ' + klass;
            }
            text = "<i class='" + htmlEncode(klass) + "'></i> " + text;
        }
        if (text == null || text.length === 0) {
            btn.addClass('no-text');
        }
        else {
            btn.find('span').html(text);
        }

        if (b.visible === false)
            btn.hide();

        if (b.disabled != null && typeof b.disabled !== "function")
            btn.toggleClass('disabled', !!b.disabled);

        if (typeof b.visible === "function" || typeof b.disabled == "function") {
            btn.on('updateInterface', () => {
                if (typeof b.visible === "function")
                    btn.toggle(!!b.visible());

                if (typeof b.disabled === "function")
                    btn.toggleClass("disabled", !!b.disabled());
            });
        }

        if (b.hotkey && window['Mousetrap' as any] != null) {
            this.mouseTrap = this.mouseTrap || (window['Mousetrap' as any] as any)(
                b.hotkeyContext || this.options.hotkeyContext || window.document.documentElement);

            this.mouseTrap.bind(b.hotkey, function () {
                if (btn.is(':visible')) {
                    btn.triggerHandler('click');
                }
                return b.hotkeyAllowDefault;
            });
        }
    }

    findButton(className: string): JQuery {
        if (className != null && className.startsWith('.')) {
            className = className.substr(1);
        }
        return $('div.tool-button.' + className, this.element);
    }

    updateInterface() {
        this.element.find('.tool-button').each(function (i: number, el: Element) {
            $(el).triggerHandler('updateInterface')
        });
    }
}

export class ToolbarComponent<P extends ToolbarOptions = ToolbarOptions> extends Toolbar<P> {
    constructor(props?: WidgetProps<P>) {
        super(props);
    }
}
