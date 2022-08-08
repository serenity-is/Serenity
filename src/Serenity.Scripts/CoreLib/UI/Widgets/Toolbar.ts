import { Decorators } from "../../Decorators";
import { htmlEncode, isEmptyOrNull, startsWith } from "../../Q";
import { Widget } from "./Widget";

export interface ToolButton {
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
export class PopupMenuButton extends Widget<PopupMenuButtonOptions> {
    constructor(div: JQuery, opt: PopupMenuButtonOptions) {
        super(div, opt);

        div.addClass('s-PopupMenuButton');
        div.click(e => {
            e.preventDefault();
            e.stopPropagation();
            if (this.options.onPopup != null) {
                this.options.onPopup();
            }

            var menu = this.options.menu;
            menu.show().position({
                my: this.options.positionMy ?? 'left top',
                at: this.options.positionAt ?? 'left bottom',
                of: this.element
            });

            var uq = this.uniqueName;
            $(document).one('click.' + uq, function (x) {
                menu.hide();
            });
        });

        this.options.menu.hide().appendTo(document.body)
            .addClass('s-PopupMenu').menu();
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
export class PopupToolButton extends PopupMenuButton {
    constructor(div: JQuery, opt: PopupToolButtonOptions) {
        super(div, opt);

        div.addClass('s-PopupToolButton');
        $('<b/>').appendTo(div.children('.button-outer').children('span'));
    }
}

export interface ToolbarOptions {
    buttons?: ToolButton[];
    hotkeyContext?: any;
}

@Decorators.registerClass('Serenity.Toolbar')
export class Toolbar extends Widget<ToolbarOptions> {
    constructor(div: JQuery, options: ToolbarOptions) {
        super(div, options);

        this.element.addClass('s-Toolbar clearfix')
            .html('<div class="tool-buttons"><div class="buttons-outer">' +
                '<div class="buttons-inner"></div></div></div>');
        var container = $('div.buttons-inner', this.element);
        var buttons = this.options.buttons;
        for (var i = 0; i < buttons.length; i++) {
            this.createButton(container, buttons[i]);
        }
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

    protected createButton(container: JQuery, b: ToolButton) {
        var cssClass = b.cssClass ?? '';

        if (b.separator === true || b.separator === 'left' || b.separator === 'both') {
            $('<div class="separator"></div>').appendTo(container);
        }

        var btn = $('<div class="tool-button"><div class="button-outer">' +
            '<span class="button-inner"></span></div></div>')
            .appendTo(container);

        if (b.separator === 'right' || b.separator === 'both') {
            $('<div class="separator"></div>').appendTo(container);
        }

        if (cssClass.length > 0) {
            btn.addClass(cssClass);
        }

        if (!isEmptyOrNull(b.hint)) {
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

        if (!isEmptyOrNull(b.icon)) {
            btn.addClass('icon-tool-button');
            var klass = b.icon;
            if (startsWith(klass, 'fa-')) {
                klass = 'fa ' + klass;
            }
            else if (startsWith(klass, 'glyphicon-')) {
                klass = 'glyphicon ' + klass;
            }
            text = "<i class='" + klass + "'></i> " + text;
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

        if (!!(!isEmptyOrNull(b.hotkey) && window['Mousetrap'] != null)) {
            this.mouseTrap = this.mouseTrap || window['Mousetrap'](
                b.hotkeyContext || this.options.hotkeyContext || window.document.documentElement);

            this.mouseTrap.bind(b.hotkey, function (e1: BaseJQueryEventObject, action: any) {
                if (btn.is(':visible')) {
                    btn.triggerHandler('click');
                }
                return b.hotkeyAllowDefault;
            });
        }
    }

    findButton(className: string): JQuery {
        if (className != null && startsWith(className, '.')) {
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
