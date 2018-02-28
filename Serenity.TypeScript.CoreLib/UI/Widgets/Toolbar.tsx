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

    export interface ToolbarOptions {
        buttons?: ToolButton[];
        hotkeyContext?: any;
    }

    @Decorators.registerClass('Serenity.Toolbar')
    export class Toolbar extends Widget<ToolbarOptions> {

        constructor(div: JQuery, options: ToolbarOptions) {
            super(div, options);

            Q.mount(this.render(options), div[0]);
            this.setupMouseTrap();
        }

        protected mouseTrap: any;

        destroy() {
            this.element.find(Toolbar.buttonSelector).unbind('click');

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

        protected setupMouseTrap() {
            if (!window['Mousetrap'])
                return;

            var buttons;
            for (var b of this.options.buttons || []) {
                if (Q.isEmptyOrNull(b.hotkey))
                    continue;

                this.mouseTrap = this.mouseTrap || window['Mousetrap'](
                    this.options.hotkeyContext || window.document.documentElement);

                ((x) => {
                    var btn = (buttons = buttons || this.element.find(Toolbar.buttonSelector))
                        .filter("." + x.cssClass);

                    this.mouseTrap.bind(x.hotkey, function (e: BaseJQueryEventObject, action: any) {
                        if (btn.is(':visible')) {
                            btn.triggerHandler('click');
                        }
                        return x.hotkeyAllowDefault;
                    });
                })(b);
            }
        }


        static buttonSelector = "div.tool-button";

        adjustIconClass(icon: string): string {
            if (!icon)
                return icon;

            if (Q.startsWith(icon, 'fa-'))
                return 'fa ' + icon;

            if (Q.startsWith(icon, 'glyphicon-'))
                return 'glyphicon ' + icon;

            return icon;
        }

        buttonClass(btn: ToolButton) {
            return {
                "tool-button": true,
                "icon-tool-button": !!btn.icon,
                "no-text": !btn.title,
                disabled: btn.disabled,
                [btn.cssClass]: !!btn.cssClass,
            }
        }

        buttonClick(e: MouseEvent, btn: ToolButton) {
            if (!btn.onClick || $(e.currentTarget).hasClass('disabled'))
                return;

            btn.onClick(e);
        }

        findButton(className: string): JQuery {
            if (className != null && Q.startsWith(className, '.')) {
                className = className.substr(1);
            }

            return $(Toolbar.buttonSelector + '.' + className, this.element);
        }

        render(options: ToolbarOptions & Q.ComponentProps<this>): Q.VNode {
            return (
                <div class="s-Toolbar clearfix">
                    <div class="tool-buttons">
                        <div class="buttons-outer">
                            <div class="buttons-inner">
                                {this.renderButtons(options.buttons)}
                                {options.children}
                            </div>
                        </div>
                    </div>
                </div>
            );
        }

        renderButtons(buttons: ToolButton[]) {
            var result: Q.VNode[] = [];
            for (var btn of buttons) {
                if (btn.separator)
                    result.push(<div class="separator" />);

                result.push(this.renderButton(btn));
            }

            return <Q.Fragment>{result}</Q.Fragment>;
        }

        renderButton(btn: ToolButton) {
            return (
                <div class={this.buttonClass(btn)} title={btn.hint}
                    onClick={(e) => this.buttonClick(e, btn)}>
                    <div class="button-outer">
                        {this.renderButtonText(btn)}
                    </div>
                </div>
            );
        }

        renderButtonText(btn: ToolButton) {
            var klass = this.adjustIconClass(btn.icon);
            if (!klass && !btn.title)
                return <span class="button-inner"></span>;
            
            if (!btn.htmlEncode) {
                return (<span class="button-inner" setInnerHTML={(klass ? '<i class="' +
                    Q.attrEncode(klass) + '"></i> ' : '') + btn.title}></span>)
            }

            if (!klass)
                return <span class="button-inner">{btn.title}</span>

            return <span class="button-inner"><i class={klass} ></i>{btn.title}</span>
        }
    }
}