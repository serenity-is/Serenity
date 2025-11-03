
const hasOwnProperty = Object.hasOwnProperty;

const bindThisHandler: ProxyHandler<any> = {
    get: (target: Record<string, any>, property: string | symbol) => {
        if (hasOwnProperty.call(target, property)) {
            return target[property as keyof typeof target];
        }

        const m = target[property as keyof typeof target];
        if (typeof m === 'function') {
            return (target[property as keyof typeof target] = m.bind(target));
        }

        return m;
    }
}

const bindThisProxyMap = new WeakMap<any, any>();

export function bindThis<T>(obj: T): T {
    let proxy = bindThisProxyMap.get(obj);
    if (!proxy) {
        proxy = new Proxy(obj as any, bindThisHandler);
        bindThisProxyMap.set(obj, proxy);
    }
    return proxy;
}