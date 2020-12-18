export type Dictionary<TItem> = { [key: string]: TItem };

export function coalesce(a: any, b: any): any {
    return a != null ? a : b;
}

export function isValue(a: any): boolean {
    return a != null;
}

export let today = (): Date => {
    var d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

export function extend<T = any>(a: T, b: T): T {
    for (var key in b)
        if (b.hasOwnProperty(key))
            a[key] = b[key];
    return a;
}

export function deepClone<T = any>(a: T, a2?: any, a3?: any): T {
    // for backward compatibility
    if (a2 != null || a3 != null) {
        return extend(extend(deepClone(a || {}), deepClone(a2 || {})), deepClone(a3 || {}));
    }

    if (!a)
        return a;
    
    let v: any;
    let b: T = Array.isArray(a) ? [] : {} as any;
    for (const k in a) {
        v = a[k];
        b[k] = (typeof v === "object") ? deepClone(v) : v;
    }
    
    return b;
}