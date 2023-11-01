import { Config, endsWith, getType, getTypeNameProp, getTypes } from "../q";

export function commonTypeRegistry(isMatch: (type: any) => boolean, attrKey: (type: any) => string, suffix: string) {

    let knownTypes: { [key: string]: any };

    function reset() {
        knownTypes = null;
    }
    
    function search(key: string) {
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
    
    function tryGet(key: string): any {
        if (!key)
            return null;

        var type;
        if (knownTypes == null)
            init();

        var type = knownTypes[key];
        if (type)
            return type;

        type = search(key);

        if (type == null && suffix && !endsWith(key, suffix))
            type = knownTypes[key + suffix] ?? search(key + suffix);

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

    return {
        reset,
        tryGet
    }
}