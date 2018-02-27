namespace Serenity {
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
        separator?: boolean;
        disabled?: boolean;
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
                    my: Q.coalesce(this.options.positionMy, 'left top'),
                    at: Q.coalesce(this.options.positionAt, 'left bottom'),
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
}