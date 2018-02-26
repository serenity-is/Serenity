namespace Serenity {
    export class ToolBarButton extends preact.Component<ToolButton, void> {
        constructor(props: ToolButton) {
            super(props);
        }

        render(): JSX.Element {
            var p = this.props;
            var klass = p.icon;
            var htext = p.title;

            if (Q.startsWith(klass, 'fa-')) {
                klass = 'fa ' + klass;
            }
            else if (Q.startsWith(klass, 'glyphicon-')) {
                klass = 'glyphicon ' + klass;
            }

            if (p.htmlEncode !== false)
                htext = Q.htmlEncode(htext);

            var htext = '<i class="' + Q.attrEncode(klass) + '"></i> ' + htext;

            return (
                <div class={{
                    "tool-button": true,
                    "icon-tool-button": !p.icon,
                    "no-text": !p.title,
                    disabled: p.disabled,
                    [p.cssClass]: !!p.cssClass,
                }} title={p.hint} onClick={(e) => {
                    if (p.disabled || !p.onClick)
                        return;

                    p.onClick(e);
                }}>
                    <div class="button-outer">
                        <span dangerouslySetInnerHTML={{ __html: htext }}></span>
                    </div>
                </div>
            )
        }

        protected mouseTrap: any;

        componentDidMount() {

            if (!!(!Q.isEmptyOrNull(this.props.hotkey) && window['Mousetrap'] != null)) {
                this.mouseTrap = this.mouseTrap || window['Mousetrap'](
                    this.props.hotkeyContext || window.document.documentElement);

                this.mouseTrap.bind(this.props.hotkey,
                    (e1: any, action: any) => {
                        if ($(this.base).is(':visible')) {
                            var event = new Event('click', { bubbles: true });
                            this.base.dispatchEvent(event);
                        }
                        return this.props.hotkeyAllowDefault;
                    });
            }
        }

        componentWillUnmount() {
            if (this.mouseTrap) {
                if (!!this.mouseTrap.destroy) {
                    this.mouseTrap.destroy();
                }
                else {
                    this.mouseTrap.reset();
                }
                this.mouseTrap = null;
            }
        }
    }

    export class ToolBar extends preact.Component<ToolbarOptions, void> {
        constructor(props?: ToolbarOptions) {
            super(props);
        }

        render() {
            return (
                <div class="s-Toolbar clearfix">
                    <div class="tool-buttons">
                        <div class="buttons-outer">
                            <div class="buttons-inner">
                                {(this.props.buttons || []).map(button => {
                                    { button.separator && <div class="separator" /> }
                                    <Serenity.ToolBarButton hotkeyContext={this.props.hotkeyContext} {...button} />
                                })}
                            </div>
                        </div>
                    </div>
                </div>
            );
        }
    }
}