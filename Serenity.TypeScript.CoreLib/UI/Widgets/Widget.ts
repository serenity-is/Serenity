namespace Serenity {

    @Decorators.registerInterface('Serenity.IAsyncInit')
    export class IAsyncInit {
    }

    @Serenity.Decorators.registerClass()
    export class Widget<TOptions> {
        private static nextWidgetNumber = 0;
        public element: JQuery;
        protected options: TOptions;
        protected widgetName: string;
        protected uniqueName: string;
        protected asyncPromise: PromiseLike<void>;

        constructor(element: JQuery, options?: TOptions) {
            this.element = element;
            this.options = options || ({} as TOptions);

            this.widgetName = Widget.getWidgetName((ss as any).getInstanceType(this));
            this.uniqueName = this.widgetName + (Widget.nextWidgetNumber++).toString();

            if (element.data(this.widgetName)) {
                throw new ss.Exception(Q.format("The element already has widget '{0}'!", this.widgetName));
            }

            element.bind('remove.' + this.widgetName, e => {
                if (e.bubbles || e.cancelable) {
                    return;
                }
                this.destroy();
            }).data(this.widgetName, this);

            this.addCssClass();

            if (this.isAsyncWidget()) {
                window.setTimeout(() => {
                    if (element && !this.asyncPromise) {
                        this.asyncPromise = this.initializeAsync();
                    }
                }, 0);
            }
        }

        public destroy(): void {
            this.element.removeClass('s-' + (ss as any).getTypeName((ss as any).getInstanceType(this)));
            this.element.unbind('.' + this.widgetName).unbind('.' + this.uniqueName).removeData(this.widgetName);
            this.element = null;
            this.asyncPromise = null;
        }

        protected addCssClass(): void {
            this.element.addClass(this.getCssClass());
        }

        protected getCssClass(): string {
            var type = (ss as any).getInstanceType(this);
            var klass = 's-' + (ss as any).getTypeName(type);
            var fullClass = Q.replaceAll((ss as any).getTypeFullName(type), '.', '-');

            for (let k of Q.Config.rootNamespaces) {
                if (Q.startsWith(fullClass, k + '-')) {
                    fullClass = fullClass.substr(k.length + 1);
                    break;
                }
            }

            fullClass = 's-' + fullClass;
            if (klass === fullClass) {
                return klass;
            }

            return klass + ' ' + fullClass;
        }

        protected initializeAsync(): PromiseLike<void> {
            return Promise.resolve();
        }

        protected isAsyncWidget(): boolean {
            return (ss as any).isInstanceOfType(this, Serenity.IAsyncInit);
        }

        public static getWidgetName(type: Function): string {
            return Q.replaceAll((ss as any).getTypeFullName(type), '.', '_');
        }

        public static elementFor<TWidget>(editorType: { new (...args: any[]): TWidget }): JQuery {
            var elementAttr = (ss as any).getAttributes(editorType, Serenity.ElementAttribute, true);
            var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].value : '<input/>');
            return $(elementHtml);
        };

        public static create<TWidget extends Widget<TOpt>, TOpt>(params: CreateWidgetParams<TWidget, TOpt>) {
            let widget: TWidget;

            if ((ss as any).isAssignableFrom(Serenity.IDialog, params.type)) {
                widget = new (params.type as any)(params.options);
                if (params.container)
                    widget.element.appendTo(params.container);
                params.element && params.element(widget.element);
            }
            else {
                var e = Widget.elementFor(params.type);
                if (params.container)
                    e.appendTo(params.container);
                params.element && params.element(e);
                widget = new params.type(e, params.options);
            }

            widget.init(params.init);
            return widget;
        }

        public init(action?: (widget: any) => void): this {
            var promise = this.initialize();
            if (action) {
                promise.then(() => action(this));
            }
            return this;
        }

        public initialize(): PromiseLike<void> {
            if (!this.isAsyncWidget()) {
                return Promise.resolve();
            }

            if (!this.asyncPromise) {
                this.asyncPromise = this.initializeAsync();
            }

            return this.asyncPromise;
        }

        mount(props: any, content: Q.VNode[]): Node {
            throw "This method is only here for TypeScript VDOM to work!";
        }

        get props(): TOptions {
            return this.options;
        }
    }

    export declare interface Widget<TOptions> {
        addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery;
        getGridField(): JQuery;
        change(handler: (e: JQueryEventObject) => void): void;
        changeSelect2(handler: (e: JQueryEventObject) => void): void;
    }

    Widget.prototype.addValidationRule = function (eventClass: string, rule: (p1: JQuery) => string): JQuery {
        return VX.addValidationRule(this.element, eventClass, rule);
    }
}