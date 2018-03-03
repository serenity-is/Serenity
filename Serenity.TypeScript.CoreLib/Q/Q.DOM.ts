namespace Q {

    export interface VNode {
        _vnode?: boolean;
        _text?: string;
        type?: ITemplate<any> | TemplateFunction<any> | string;
        isSVG?: boolean;
        props?: any;
        children?: JSX.Children;
    }

    export interface ITemplate<P = any> {
        render(props?: P, children?: JSX.Children): JSX.Element | null;
        mounted?(node: Node): void;
        unmounted?(): void;
    }

    export abstract class Template<P = any> implements ITemplate<P> {
        constructor(props: P, children?: JSX.Children) {
            this.props = props;
            this.children = children;
        }

        abstract render(): JSX.Element | null;

        static defaultProps?: any;
        props: P & TemplateProps<this>
        children?: JSX.Children;
    }

    export interface TemplateFunction<P = any> {
        (props?: P, children?: JSX.Children): JSX.Element | null;
        defaultProps?: any;
    }

    export interface TemplateProps<C extends TemplateFunction<any> | Template<any>> {
        ref?: (el: C) => void;
    }

    export interface WidgetProps<W extends Serenity.Widget<any>> {
        id?: string;
        name?: string;
        class?: string;
        maxLength?: number;
        required?: boolean;
        readOnly?: boolean;
        ref?: (el: W) => void;
    }

    export interface VDomHtmlAttrs {
        setInnerHTML?: string;
        ref?: (el?: JSX.Element) => void;
    }

    // a mixture of preact / petit-dom for one time mount only, update / removal with jQuery
    const isArray = Array.isArray;
    const EMPTYO = {};
    const EMPTYAR: any[] = [];
    const isVNode = (c: any) => c && (c._vnode != null || c._text != null);

    export function createElement(type: any, props: any, contArg?: any): JSX.Element {
        var children: JSX.Children,
            args,
            i,
            isSVG = false;
        var len = arguments.length - 2;

        if (typeof type !== "string") {
            if (len === 1) {
                if (isArray(contArg))
                    children = contArg;
                else
                    children = [contArg]
            } else if (len > 1) {
                args = Array(len);
                for (i = 0; i < len; i++) {
                    args[i] = arguments[i + 2];
                }
                children = args;
            }
        } else {
            isSVG = type === "svg";
            if (len === 1) {
                if (isArray(contArg)) {
                    children = maybeFlatten(contArg, isSVG);
                } else if (isVNode(contArg)) {
                    (contArg as VNode).isSVG = isSVG;
                    children = [contArg];
                } else {
                    children = [{ _text: contArg == null ? "" : contArg }];
                }
            } else if (len > 1) {
                args = Array(len);
                for (i = 0; i < len; i++) {
                    args[i] = arguments[i + 2];
                }
                children = maybeFlatten(args, isSVG);
            } else {
                children = EMPTYAR;
            }
        }

        return {
            _vnode: true,
            isSVG,
            type,
            props: props || EMPTYO,
            children
        };
    }

    export function maybeFlatten(arr: any[], isSVG?: boolean): JSX.Element[] {
        for (var i = 0; i < arr.length; i++) {
            var ch = arr[i];
            if (isArray(ch)) {
                return flattenChildren(arr, i, arr.slice(0, i), isSVG);
            } else if (!isVNode(ch)) {
                arr[i] = { _text: ch == null ? "" : ch };
            } else if (isSVG && !ch.isSVG) {
                ch.isSVG = true;
            }
        }
        return arr;
    }

    function flattenChildren(children: any[], start: number, arr: VNode[], isSVG?: boolean) {
        for (var i = start; i < children.length; i++) {
            var ch = children[i];
            if (isArray(ch)) {
                flattenChildren(ch, 0, arr, isSVG);
            } else if (isVNode(ch)) {
                if (isSVG && !ch.isSVG) {
                    ch.isSVG = true;
                }
                arr.push(ch);
            } else {
                arr.push({ _text: ch == null ? "" : ch });
            }
        }
        return arr;
    }

    const SVG_NS = "http://www.w3.org/2000/svg";
    const SELECT = "select";
    const SELECT_DELAYED_PROPS = { selectedIndex: true };
    const INPUT = "input";
    const INPUT_DELAYED_PROPS = { type: true, value: true };

    const XLINK_NS = "http://www.w3.org/1999/xlink";
    const NS_ATTRS = {
        show: XLINK_NS,
        actuate: XLINK_NS,
        href: XLINK_NS
    };

    const IS_NON_DIMENSIONAL = /acit|ex(?:s|g|n|p|$)|rph|ows|mnc|ntw|ine[ch]|zoo|^ord/i;

    const IGNORE_KEYS = {
        "ref": true,
        "children": true
    }

    function setProps(el: Element, props: any, ignoreKeys?: any) {
        for (var key in props) {

            if (IGNORE_KEYS[key] === true)
                continue;

            if (ignoreKeys != null && ignoreKeys[key] === true)
                continue;

            var val = props[key];
            if (val == null || val == false)
                continue;

            if (key === 'class')
                key = 'className';
            else if (key === "for")
                key = 'htmlFor';

            if (key == 'style') {
                if (!val || typeof val === 'string') {
                    (el as HTMLElement).style.cssText = val || '';
                }
                if (val && typeof val === 'object') {
                    for (let i in val) {
                        (el as HTMLElement).style[i] = typeof val[i] === 'number' &&
                            IS_NON_DIMENSIONAL.test(i) === false ? (val[i] + 'px') : val[i];
                    }
                }
                continue;
            }
            else if (key == "className") {
                if (val && typeof val === 'object') {
                    var classes: string[] = [];
                    for (var key of Object.getOwnPropertyNames(val)) {
                        if (val[key]) {
                            classes.push(key);
                        }
                    }
                    val = classes.join(' ');
                }
                el.className = val;
                continue;
            }
            else if (key == "setInnerHTML") {
                el.innerHTML = val;
                continue;
            }
            else if (key.charAt(0) == 'o' && key.charAt(1) == 'n') {
                key = key.toLowerCase();
            }

            try {
                el[key] = val;
            } catch (e) { }
        }
    }

    function appendChildren(
        parent: Element,
        children: VNode[],
        start = 0,
        end = children.length - 1,
        beforeNode?: Element
    ) {
        while (start <= end) {
            var ch = children[start++];
            parent.insertBefore(mount(ch), beforeNode);
        }
    }

    function updateSelect(node: HTMLSelectElement, props: any, children: VNode[], oldProps?: any, oldChildren?: any[]) {
        const isMount = oldProps == null;
        const hasSelIndex = props.selectedIndex != null;
        const hasValue = !hasSelIndex && "value" in props;
        const ignoreKeys = hasSelIndex || hasValue ? SELECT_DELAYED_PROPS : null;
        setProps(node, props, ignoreKeys);
        appendChildren(node, children);
        if (!props.multiple) {
            if (hasSelIndex) {
                if (isMount || props.selectedIndex !== oldProps.selectedIndex) {
                    node.selectedIndex = props.selectedIndex;
                }
            } else if (hasValue) {
                if (isMount || props.value !== oldProps.value) {
                    node.value = props.value;
                }
            }
        }
        return node;
    }

    function updateInput(node: HTMLInputElement, props: any) {
        const hasValue = props.value != null;

        const ignoreKeys = hasValue ? INPUT_DELAYED_PROPS : null;
        if (props.type != null) {
            node.type = props.type;
        }

        setProps(node, props, ignoreKeys);

        if (hasValue)
            node.value = props.value;

        return node;
    }

    function setAttributes(el: Element, attrs: any) {
        for (var key in attrs) {

            if (IGNORE_KEYS[key] === true)
                continue;

            var newv = attrs[key];
            if (newv == null || newv === false)
                continue;

            setDOMAttr(el, key, newv);
        }
    }

    function setDOMAttr(el: Element, attr: string, value: any) {
        if (value === true) {
            el.setAttribute(attr, "");
        } else {
            var ns = NS_ATTRS[attr];
            if (ns !== undefined) {
                el.setAttributeNS(ns, attr, value);
            } else {
                el.setAttribute(attr, value);
            }
        }
    }

    export type Empty = null | void | boolean;

    export function Fragment(props?: any, children?: JSX.Element[]): JSX.Element | null {
        return null;
    }

    export function extend(obj: any, props: any) {
        for (let i in props)
            obj[i] = props[i];

        return obj;
    }

    function getNodeProps(vnode: VNode) {
        let defaultProps = (vnode.type as any).defaultProps;
        if (defaultProps === undefined)
            return vnode.props;

        let props = extend({}, vnode.props);
        for (let i in defaultProps) {
            if (props[i] === undefined) {
                props[i] = defaultProps[i];
            }
        }

        return props;
    }

    var uniqueId = 0;
    var mountQueue: any[] = [];
    var mountDepth = 0;

    export function withUniqueID<T>(action: (uid: (id: string) => string) => T): T{
        var prefix = "uid_" + (++uniqueId) + "_";
        return action(function (s) {
            return prefix + s;
        });
    }

    function processQueue() {
        var m = mountQueue;
        mountQueue = [];
        for (var x of m)
            x();
    }

    export function render(vnode: VNode, parent: Node): void {
        mountDepth++;
        try {
            parent.appendChild(mount(vnode));
        }
        finally {
            mountDepth--;
        }

        if (mountDepth == 0)
            processQueue();
    }

    export function mount(vnode: VNode, node?: Node): Node {
        mountDepth++;
        try {
            var ref = vnode.props && vnode.props.ref;
            var component: any, vnode: VNode;
            if (vnode._text != null) {
                node = node || document.createTextNode(vnode._text);
            } else if (vnode._vnode === true) {
                const type = vnode.type;
                const children = vnode.children;
                const isSVG = vnode.isSVG;
                var props = vnode.props;
                if (typeof type === "string") {
                    const isSelect =
                        !isSVG && type.length === 6 && type.toLowerCase() === SELECT;
                    const isInput =
                        !isSelect &&
                        !isSVG &&
                        type.length === 5 &&
                        type.toLowerCase() === INPUT;
                    if (isSelect) {
                        node = node || document.createElement(type);
                        updateSelect(node as HTMLSelectElement, props, children as JSX.Element[]);
                    } else if (isInput) {
                        node = node || document.createElement(type);
                        updateInput(node as HTMLInputElement, props);
                    } else {
                        if (!isSVG) {
                            node = node || document.createElement(type);
                            setProps(node as HTMLElement, props);
                        } else {
                            node = node || document.createElementNS(SVG_NS, type);
                            setAttributes(node as Element, props);
                        }
                        if (isArray(children)) {
                            appendChildren(node as Element, children as VNode[]);
                        } else {
                            node.appendChild(mount(children));                            
                        }
                    }
                }
                else if ((type as ITemplate).render) {
                    component = type;
                    props = getNodeProps(vnode);
                    var vnode = (type as ITemplate).render(props, children);
                    node = mount(vnode);
                } else if (typeof type === "function") {
                    if (type.prototype.__isWidget) {
                        node = Serenity.Widget.elementFor(type as any)[0];
                        var mref = ref;
                        ref = null;
                        mountQueue.push(function () {
                            var $node = $(node);
                            var e = node as Element;

                            if (props.name != null)
                                e.setAttribute("name", props.name);

                            if (props.id != null) {
                                if (typeof props.id === "function") {
                                    if (props.name != null)
                                        e.setAttribute("id", props.id(props.name));
                                }
                                else
                                    e.setAttribute("id", props.id);
                            }

                            if ($node.is(':input'))
                                $node.addClass("editor");

                            if (props.class != null)
                                $node.addClass(props.class);

                            var widget = new (type as any)($node, props);

                            if (props.maxLength != null)
                                e.setAttribute("maxLength", props.maxLength);

                            if (props.required)
                                Serenity.EditorUtils.setRequired(widget, true);

                            if (props.readOnly)
                                Serenity.EditorUtils.setReadOnly(widget, true);

                            if (mref) {
                                mref(widget);

                                $node.on('remove', function () {
                                    mref(null);
                                });
                            }
                        });
                    }
                    else if (type.prototype.render) {
                        props = getNodeProps(vnode);
                        var instance = new (type as any)(props, children);
                        component = instance;
                        vnode = (instance as ITemplate).render(props, children);
                        node = mount(vnode);
                    }
                    else if (type === Fragment) {
                        node = document.createDocumentFragment();
                        if (isArray(children)) {
                            var start = 0;
                            var end = (children as any).length - 1;
                            while (start <= end) {
                                var ch = children[start++];
                                node.appendChild(mount(ch as JSX.Element));
                            }
                        }
                        else 
                            node.appendChild(mount(children));
                    }
                    else {
                        props = getNodeProps(vnode);
                        vnode = (type as TemplateFunction)(props, children);
                        node = mount(vnode);
                    }
                }
            }
            if (node == null) {
                throw new Error("Unknown node type!");
            }

            if (ref != null) {
                mountQueue.push(function () {
                    ref(component || node);
                });

                $(node).on('remove', function () {
                    ref(null);
                });
            }

            if (component != null && (component as ITemplate).mounted != null) {
                mountQueue.push(function () {
                    (component as ITemplate).mounted(node);
                });
            }

            if (component != null && (component as ITemplate).unmounted != null) {
                mountQueue.push(function () {
                    $(node).one('remove', function () {
                        (component as ITemplate).unmounted();
                    });
                });
            }
        }
        finally {
            mountDepth--;
        }

        if (!mountDepth)
            processQueue();

        return node;
    }
}

const H = Q.createElement;