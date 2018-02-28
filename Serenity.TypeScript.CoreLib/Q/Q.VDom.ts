namespace Q {
    // modified version of https://github.com/yelouafi/petit-dom for one time mount only, update / removal with jQuery
    const isArray = Array.isArray;
    const EMPTYO = {};
    const EMPTYAR: any[] = [];
    const isVNode = (c: any) => c && (c._vnode != null || c._text != null);
    const isComponent = (c: any) => c && c.mount;

    export function h<PropsType = any>(type: Q.FunctionalComponent<PropsType>, props: PropsType, children?: (JSX.Element | JSX.Element[] | string)[]): JSX.Element;
    export function h(type: string, props: JSX.HTMLAttributes & JSX.SVGAttributes & { [propName: string]: any }, children?: (JSX.Element | JSX.Element[] | string)[]): JSX.Element;
    export function h(type: any, props: any, contArg?: (JSX.Element | JSX.Element[] | string)[]): any {
        var content,
            args,
            i,
            isSVG = false;
        var len = arguments.length - 2;

        if (typeof type !== "string") {
            if (len === 1) {
                content = contArg;
            } else if (len > 1) {
                args = Array(len);
                for (i = 0; i < len; i++) {
                    args[i] = arguments[i + 2];
                }
                content = args;
            }
        } else {
            isSVG = type === "svg";
            if (len === 1) {
                if (Array.isArray(contArg)) {
                    content = maybeFlatten(contArg, isSVG);
                } else if (isVNode(contArg)) {
                    (contArg as VNode).isSVG = isSVG;
                    content = [contArg];
                } else {
                    content = [{ _text: contArg == null ? "" : contArg }];
                }
            } else if (len > 1) {
                args = Array(len);
                for (i = 0; i < len; i++) {
                    args[i] = arguments[i + 2];
                }
                content = maybeFlatten(args, isSVG);
            } else {
                content = EMPTYAR;
            }
        }

        return {
            _vnode: true,
            isSVG,
            type,
            key: (props && props.key) || null,
            props: props || EMPTYO,
            content
        };
    }

    export function maybeFlatten(arr: any[], isSVG?: boolean) {
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

    function setProps(el: Element, props: any, ignoreKeys?: any) {
        for (var key in props) {
            if (ignoreKeys != null && ignoreKeys[key] === true) continue;
            var val = props[key];

            if (key === 'class')
                key = 'className';

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

            el[key] = val;
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

            if (key === "ref")
                continue;

            var newv = attrs[key];
            setDOMAttr(el, key, newv);
        }
    }

    function setDOMAttr(el: Element, attr: string, value: any) {
        if (value === true) {
            el.setAttribute(attr, "");
        } else if (value === false) {
            el.removeAttribute(attr);
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

    export class Fragment implements IComponent {
        mount(props: any, content: VNode[]): Node {
            var node = document.createDocumentFragment();
            if (content) {
                for (var x of content) {
                    node.appendChild(Q.mount(x));
                }
            }
            return node;
        }
    }

    var mountQueue: any[] = [];
    var mountDepth = 0;

    export function mount(c: VNode, node?: Node): Node {
        mountDepth++;
        try {
            if (c._text != null) {
                node = node || document.createTextNode(c._text);
            } else if (c._vnode === true) {
                const { type, props, content, isSVG } = c;
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
                        updateSelect(node as HTMLSelectElement, props, content);
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
                        if (!isArray(content)) {
                            node.appendChild(mount(content));
                        } else {
                            appendChildren(node as Element, content);
                        }
                    }
                }
                else if (isComponent(type)) {
                    node = (type as IComponent).mount(props, content);
                } else if (typeof type === "function") {
                    if ((ss as any).isAssignableFrom(Serenity.Widget, type)) {
                        node = Serenity.Widget.elementFor(type as any)[0];
                        mountQueue.push(function () {
                            new (type as any)($(node), props);
                        });
                    }
                    else if (isComponent(type.prototype)) {
                        var instance = new (type as any)(props, content);
                        node = instance.mount(props, content);
                    }
                    else {
                        var vnode = type(props, content);
                        node = mount(vnode);
                    }
                }
            }
            if (node == null) {
                throw new Error("Unknown node type!");
            }

            if (c.props && c.props.ref) {
                var ref = c.props.ref;
                mountQueue.push(function () {
                    ref(node);
                    $(node).on('remove', function () {
                        ref(null);
                    });
                });
            }
        }
        finally {
            mountDepth--;
        }

        if (!mountDepth) {
            var m = mountQueue;
            mountQueue = [];
            for (var x of m)
                x();
        }

        return node;
    }
}

const H = Q.h;