
const hasOwnProperty = Object.hasOwnProperty;

function findPropertyDescriptor(target: any, property: string | symbol): PropertyDescriptor | undefined {
    let current = Object.getPrototypeOf(target);
    while (current) {
        const descriptor = Object.getOwnPropertyDescriptor(current, property);
        if (descriptor)
            return descriptor;
        current = Object.getPrototypeOf(current);
    }
    return undefined;
}

const bindThisHandler: ProxyHandler<any> = {
    get: (target: Record<string, any>, property: string | symbol) => {
        if (hasOwnProperty.call(target, property)) {
            return target[property as keyof typeof target];
        }

        const m = target[property as keyof typeof target];
        if (typeof m === 'function') {
            const bound = m.bind(target);
            const descriptor = findPropertyDescriptor(target, property);
            if (descriptor && (descriptor.get || descriptor.set)) {
                // inherited accessor (e.g. a DOM event-handler IDL attribute):
                // write through it instead of shadowing it with an own data
                // property, which would leave the accessor's own behavior dead
                (target as any)[property] = bound;
                return bound;
            }
            // cache the bound function as a non-enumerable own property so it
            // stays invisible to Object.keys/for...in, mirroring how class
            // prototype methods are non-enumerable
            Object.defineProperty(target, property, {
                value: bound,
                enumerable: false,
                writable: true,
                configurable: true
            });
            return bound;
        }

        return m;
    }
}

const bindThisProxyMap = new WeakMap<any, any>();

/**
 * Creates a proxy that automatically binds method calls to the given object.
 * Intended for use in classes, e.g. when attaching event handlers.
 * Instead of `someElement.addEventListener("click", this.onClick.bind(this))`
 * or an arrow function `(e) => this.onClick(e)` (both of which hurt performance
 * and complicate `removeEventListener` because the bound function must be stored),
 * you can write:
 *
 * ```ts
 * const boundThis = bindThis(this);
 * someElement.addEventListener("click", boundThis.onClick);
 * // later, in dispose:
 * someElement.removeEventListener("click", this.onClick);
 * ```
 *
 * The returned proxy lazily binds methods on first access and caches the bound
 * function in the target object. Subsequent accesses return the same cached
 * function, making it safe to use with `removeEventListener` by passing the
 * **original** method (e.g. `this.onClick`). There is no need to call `bindThis`
 * again in dispose — calling it a second time returns the same proxy.
 * Non-function properties are returned as-is.
 *
 * @param obj - The object whose methods should be auto-bound.
 * @returns A proxy wrapping the object with auto-bound method access.
 */
export function bindThis<T>(obj: T): T {
    let proxy = bindThisProxyMap.get(obj);
    if (!proxy) {
        proxy = new Proxy(obj as any, bindThisHandler);
        bindThisProxyMap.set(obj, proxy);
    }
    return proxy;
}