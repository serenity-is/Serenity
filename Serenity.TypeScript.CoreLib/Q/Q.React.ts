namespace Q {
    export function extend(obj: any, props: any) {
        for (let i in props)
            obj[i] = props[i];

        return obj;
    }

    var uniqueId = 0;

    export function uidGenerator(): () => (() => string) {
        var prefix = "uid_" + (++uniqueId) + "_";
        return function () {
            var counter = 0;
            return function () {
                return prefix + (++counter);
            }
        }
    }

    var hasOwn = {}.hasOwnProperty;

    export function cssClass(...args: any[]): string {
        var classes = [];
        
        for (var i = 0; i < arguments.length; i++) {
            var arg = arguments[i];
            if (!arg) continue;

            var argType = typeof arg;

            if (argType === 'string' || argType === 'number') {
                classes.push(arg);
            } else if (Array.isArray(arg) && arg.length) {
                var inner = cssClass.apply(null, arg);
                if (inner) {
                    classes.push(inner);
                }
            } else if (argType === 'object') {
                for (var key in arg) {
                    if (hasOwn.call(arg, key) && arg[key]) {
                        classes.push(key);
                    }
                }
            }
        }

        return classes.join(' ');
    }

    if (typeof React === "undefined" &&
        window['preact'] != null) {
        window['React'] = window['ReactDOM'] = window['preact'];
        (React as any).Fragment = "x-fragment";
    }

    function widgetComponentFactory(widgetType: any) {

        return (function (_super) {
            __extends(Wrapper, _super);

            function Wrapper() {
                return _super !== null && _super.apply(this, arguments) || this;
            }

            Wrapper.prototype.render = function () {
                return React.createElement("div", {
                    ref: ((el: any) => this.el = el),
                    className: 'widget-wrapper'
                });
            };

            Wrapper.prototype.componentWillReceiveProps = function (nextProps: Serenity.WidgetComponentProps<any>) {
                var props: Serenity.WidgetComponentProps<any> = this.props;


                var widget = this.widget;
                if (widget == null || widget.element == null)
                    return;

                var $node = widget.element;
                var node = $node[0];

                if (nextProps.id !== props.id) {
                    node.id = nextProps.id;
                }

                if (nextProps.name !== props.name && $node.is(':input')) {
                    (node as any).name = nextProps.name;
                }

                if (nextProps.placeholder !== props.placeholder && $node.is(':input')) {
                    (node as any).placeholder = nextProps.placeholder;
                }

                if (nextProps.className !== props.className) {
                    $node.removeClass(props.className || '').addClass(nextProps.className);
                }

                if (nextProps.oneWay !== props.oneWay) {
                    node.dataset.oneWay = nextProps.oneWay ? "1" : undefined;
                }

                if (nextProps.maxLength != props.maxLength)
                    node.setAttribute("maxLength", nextProps.maxLength || 0);

                if (nextProps.required !== props.required)
                    Serenity.EditorUtils.setRequired(this.widget, nextProps.required);

                if (props.readOnly !== props.readOnly)
                    Serenity.EditorUtils.setReadOnly(this.widget, nextProps.readOnly);

                if (nextProps.setOptions != props.setOptions) {
                    Serenity.ReflectionOptionsSetter.set(this.widget, nextProps.setOptions);
                }
            }

            Wrapper.prototype.componentDidMount = function () {

                if (this.widget != null)
                    return;

                var $node = Serenity.Widget.elementFor(widgetType);
                var node = $node[0];
                this.el.appendChild(node);

                var props: Serenity.WidgetComponentProps<any> = this.props;

                if (props.id != null)
                    node.id = props.id;

                if ($node.is(':input')) {
                    $node.addClass("editor");
                    if (props.name != null)
                        (node as any).name = props.name;

                    if (props.placeholder != null)
                        (node as any).placeholder = props.placeholder;
                }

                if (props.className != null)
                    $node.addClass(props.className);

                if (props.oneWay)
                    node.dataset.oneWay = "1";

                this.widget = new (widgetType as any)($node, props);

                if (props.maxLength != null)
                    node.setAttribute("maxLength", props.maxLength.toString());

                if (props.required)
                    Serenity.EditorUtils.setRequired(this.widget, true);

                if (props.readOnly)
                    Serenity.EditorUtils.setReadOnly(this.widget, true);

                if (props.setOptions != null) {
                    Serenity.ReflectionOptionsSetter.set(this.widget, props.setOptions);
                }
            }

            Wrapper.prototype.componentWillUnmount = function () {
                this.widget && this.widget.destroy();
                this.widget = null;
            };

            Wrapper.prototype.shouldComponentUpdate = function () {
                return false;
            };

            Wrapper.prototype.isWidgetWrapper = true;

            (Wrapper as any).displayName = "Wrapped<" + (ss as any).getTypeFullName(widgetType) + ">";

            return Wrapper;
        }(React.Component));
    }

    var reactCreateElement = React.createElement;
    (React as any).createElement = function () {
        var arg = arguments[0];
        if (typeof arg === "function" && arg.__isWidgetType === true) {
            if (arg.__componentFactory === undefined)
                arguments[0] = arg.__componentFactory = widgetComponentFactory(arg);
            else
                arguments[0] = arg.__componentFactory;
        }

        return reactCreateElement.apply(this, arguments);
    };
}