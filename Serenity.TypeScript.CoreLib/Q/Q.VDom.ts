namespace Q {
    // modified version of https://github.com/yelouafi/petit-dom
    const isArray = Array.isArray;
    const EMPTYO = {};
    const EMPTYAR: any[] = [];
    const isVNode = (c: any) => c && (c._vnode != null || c._text != null);
    const isComponent = (c: any) => c && c.mount && c.patch && c.unmount;

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

    function defShouldUpdate(p1: any, p2: any, c1: any, c2: any) {
        if (c1 !== c2) return true;
        for (var key in p1) {
            if (p1[key] !== p2[key]) return true;
        }
        return false;
    }

    const IS_NON_DIMENSIONAL = /acit|ex(?:s|g|n|p|$)|rph|ows|mnc|ntw|ine[ch]|zoo|^ord/i;

    function setProps(el: Element, props: any, oldProps?: any, ignoreKeys?: any) {
        for (var key in props) {
            if (ignoreKeys != null && ignoreKeys[key] === true) continue;
            var oldv = oldProps && oldProps[key];
            var newv = props[key];

            if (oldv !== newv) {

                if (key === 'class')
                    key = 'className';

                if (key == 'style') {
                    if (!newv || typeof newv === 'string' || typeof oldv === 'string') {
                        (el as HTMLElement).style.cssText = newv || '';
                    }
                    if (newv && typeof newv === 'object') {
                        if (typeof oldv !== 'string') {
                            for (let i in oldv) if (!(i in newv)) (el as HTMLElement).style[i] = '';
                        }
                        for (let i in newv) {
                            (el as HTMLElement).style[i] = typeof newv[i] === 'number' &&
                                IS_NON_DIMENSIONAL.test(i) === false ? (newv[i] + 'px') : newv[i];
                        }
                    }
                    continue;
                }
                else if (key == "className") {
                    if (newv && typeof newv === 'object') {
                        var classes: string[] = [];
                        for (var key of Object.getOwnPropertyNames(newv)) {
                            if (newv[key]) {
                                classes.push(key);
                            }
                        }
                        newv = classes.join(' ');
                    }
                    el.className = newv;
                    continue;
                }
                else if (key == "setInnerHTML") {
                    el.innerHTML = newv;
                    continue;
                }
                else if (key.charAt(0) == 'o' && key.charAt(1) == 'n') {
                    key = key.toLowerCase();
                }

                el[key] = newv;
            }
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
        setProps(node, props, null, ignoreKeys);
        //if (isMount) {
        appendChildren(node, children);
        //} else {
        //    patchContent(node, children, oldChildren);
        //}
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

    function updateInput(node: HTMLInputElement, props: any, oldProps?: any) {
        const isMount = oldProps == null;
        const hasValue = props.value != null;

        const ignoreKeys = hasValue ? INPUT_DELAYED_PROPS : null;
        if (props.type != null && (isMount || props.type !== oldProps.type)) {
            node.type = props.type;
        }

        setProps(node, props, null, ignoreKeys);

        if (hasValue && (isMount || props.value !== oldProps.value)) {
            node.value = props.value;
        }
        return node;
    }

    function setAttributes(el: Element, attrs: any, oldAttrs?: any) {
        for (var key in attrs) {
            var oldv = oldAttrs != null ? oldAttrs[key] : undefined;
            var newv = attrs[key];
            if (oldv !== newv) {
                setDOMAttr(el, key, newv);
            }
        }
        for (key in oldAttrs) {
            if (!(key in attrs)) {
                el.removeAttribute(key);
            }
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

    export function mountTo(parent: Element, c: VNode | VNode[]) {
        if (Array.isArray(c))
            appendChildren(parent, c);
        else
            parent.appendChild(mount(c));
    }

    export type Empty = null | void | boolean;

    export class Fragment implements IComponent {
        constructor(props?: any, context?: any) {
        }

        mount(props: any, content: VNode[]): Node {
            var node = document.createDocumentFragment();
            if (content) {
                for (var x of content) {
                    node.appendChild(Q.mount(x));
                }
            }
            return node;
        }

        patch(el: Node, newProps: any, oldProps: any, newContent: VNode[], oldContent: VNode[]): Node {
            for (var i = el.childNodes.length - 1; i >= 0; i--)
                el.removeChild(el.childNodes[i]);

            return el;
        }

        unmount(el: Node): void {
        }
    }

    export function mount(c: VNode, node?: Node): Node {
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
                    // TODO : {is} for custom elements
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
            } else if (isComponent(type)) {
                node = (type as IComponent).mount(props, content);
            } else if (typeof type === "function") {
                if (isComponent(type.prototype)) {
                    var instance = new (type as any)(props, content);
                    node = instance.mount(props, content);
                    c._data = instance;
                } else {
                    var vnode = type(props, content);
                    node = mount(vnode);
                    c._data = vnode;
                }
            }
        }
        if (node == null) {
            throw new Error("Unknown node type!");
        }
        c._node = node;

        if (c.props && c.props.ref)
            c.props.ref(node);

        return node;
    }
}

const H = Q.h;