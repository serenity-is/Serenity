namespace Serenity {

    @Decorators.registerClass('Serenity.Toolbar')
    export class Toolbar extends Widget<ToolbarOptions> {

        protected toolbar: UI.IntraToolbar;

        constructor(div: JQuery, options: ToolbarOptions) {
            super(div, options);

            if (div.length) {
                this.toolbar = ReactDOM.render(React.createElement(UI.IntraToolbar, options), div[0]);
            }
        }

        destroy() {
            if (this.toolbar != null && this.toolbar.el != null) {
                // we used to insert elements to toolbar element, and React expects it to be at start
                if ($(this.toolbar.el).index() != 0)
                    $(this.toolbar.el).prependTo(this.element);

                ReactDOM.unmountComponentAtNode(this.element[0]);
                this.toolbar = null;
            }

            super.destroy();
        }

        findButton(className: string): JQuery {
            if (className != null && Q.startsWith(className, '.')) {
                className = className.substr(1);
            }

            return $(UI.ToolButton.buttonSelector + '.' + className, this.element);
        }
    }
}