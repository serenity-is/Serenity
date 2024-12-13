import { Config, getType, getTypeNameProp, isPromiseLike } from "../base";
import { getTypes } from "../compat";

export function commonTypeRegistry<TType = any>(props: {
    kind: string,
    attrKey: (type: any) => string,
    isMatch: (type: any) => boolean,
    suffix: string,
    loadError: (key: string) => void
}) {

    const { kind, attrKey, isMatch, suffix, loadError } = props;

    let knownTypes: { [key: string]: any };

    function reset() {
        knownTypes = null;
    }

    function searchSystemTypes(key: string) {
        var type = getType(key);
        if (type != null && isMatch(type))
            return type;

        for (var ns of Config.rootNamespaces) {
            var k = ns + "." + key;
            type = knownTypes[k];
            if (type)
                return type;

            type = getType(ns + '.' + key);
            if (type != null && isMatch(type))
                return type;
        }
    }

    function init() {
        knownTypes = {};
        for (var type of getTypes()) {
            if (!isMatch(type))
                continue;

            var fullName = getTypeNameProp(type);
            knownTypes[fullName] = type;

            var akey = attrKey && attrKey(type);
            if (akey && akey !== fullName)
                knownTypes[akey] = type;
        }

        if (suffix) {
            for (var key of Object.keys(knownTypes)) {
                if (key.endsWith(suffix)) {
                    var p = key.substring(0, key.length - suffix.length);
                    if (p && !knownTypes[p])
                        knownTypes[p] = knownTypes[key];
                }
            }
        }
    }

    function tryGet(key: string): TType {
        if (!key)
            return null;

        var type;
        if (knownTypes == null)
            init();

        var type = knownTypes[key];
        if (type)
            return type;

        type = searchSystemTypes(key);

        if (type == null && suffix && !key.endsWith(suffix))
            type = knownTypes[key + suffix] ?? searchSystemTypes(key + suffix);

        if (type) {
            knownTypes[key] = type;

            var akey = attrKey && attrKey(type);
            if (akey && key != akey)
                knownTypes[akey] = type;

            if (suffix && key.endsWith(suffix)) {
                var p = key.substring(0, key.length - suffix.length);
                if (p && !knownTypes[p])
                    knownTypes[p] = knownTypes[key];
            }

            return type;
        }

        return type;
    }

    function tryGetOrLoad(key: string): TType | PromiseLike<TType> {
        let type = tryGet(key);
        if (type)
            return type;

        if (key && Config.lazyTypeLoader) {
            let promise = Config.lazyTypeLoader(key, kind as any);

            if (!promise && suffix) {
                promise = Config.lazyTypeLoader(key + suffix, kind as any);
            }

            if (!promise) {
                for (var ns of Config.rootNamespaces) {
                    const k = ns + "." + key;
                    if (promise = Config.lazyTypeLoader(k, kind as any))
                        break;
                    if (suffix && (promise = Config.lazyTypeLoader(k + suffix, kind as any)))
                        break;
                }                
            }
            
            if (isPromiseLike(promise)) {
                return promise.then(t => {
                    if (t && isMatch(t)) {
                        knownTypes[key] = t;
                        return t;
                    }
                    return null;
                });
            }

            if (promise && isMatch(promise)) {
                knownTypes[key] = promise;
                return promise;
            }

            return null;
        }

        return type;
    }

    function get(key: string): TType {
        var type = tryGet(key);
        if (type)
            return type;

        loadError(key);
    }

    function getOrLoad(key: string): TType | PromiseLike<TType> {
        var type = tryGetOrLoad(key);
        if (type) {
            if (isPromiseLike(type)) {
                return type.then(t => {
                    if (!t)
                        loadError(key);
                    return t;
                });
            }

            return type;
        }

        loadError(key);
    }

    return {
        get,
        getOrLoad,
        reset,
        tryGet,
        tryGetOrLoad
    }
}