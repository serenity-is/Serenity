import { EditorAttribute } from "../Decorators";
import { ArgumentNullException, Config, endsWith, Exception, format, getAttributes, getType, getTypeFullName, getTypes, isEmptyOrNull, startsWith } from "../Q";

// @ts-ignore
let globalObj: any = typeof (global) !== "undefined" ? global : (typeof (window) !== "undefined" ? window : (typeof (self) !== "undefined" ? self : null));

let knownTypes: { [key: string] : any };

export namespace EditorTypeRegistry {
    export function get(key: string): any {

        if (isEmptyOrNull(key)) {
            throw new ArgumentNullException('key');
        }

        initialize();

        var editorType = knownTypes[key.toLowerCase()];
        if (editorType == null) {

            var type = getType(key) ?? getType(key, globalObj);
            if (type != null) {
                knownTypes[key.toLowerCase()] = type as any;
                return type as any;
            }

            throw new Exception(format("Can't find {0} editor type!", key));
        }
        return editorType;

    }

    function initialize(): void {

        if (knownTypes != null)
            return;

        knownTypes = {};

        for (var type of getTypes()) {

            var fullName = getTypeFullName(type).toLowerCase();
            knownTypes[fullName] = type;

            var editorAttr = getAttributes(type, EditorAttribute, false);
            if (editorAttr != null && editorAttr.length > 0) {
                var attrKey = editorAttr[0].key;
                if (!isEmptyOrNull(attrKey)) {
                    knownTypes[attrKey.toLowerCase()] = type;
                }
            }

            for (var k of Config.rootNamespaces) {
                if (startsWith(fullName, k.toLowerCase() + '.')) {
                    var kx = fullName.substr(k.length + 1).toLowerCase();
                    if (knownTypes[kx] == null) {
                        knownTypes[kx] = type;
                    }
                }
            }
        }

        setTypeKeysWithoutEditorSuffix();
    }

    export function reset(): void {
        knownTypes = null;
    }

    function setTypeKeysWithoutEditorSuffix() {
        var keys = Object.keys(knownTypes);
        for (var k of keys) {
            setWithoutSuffix(k, knownTypes[k]);
        }
    }

    function setWithoutSuffix(key: string, t: any) {
        var suffix = 'editor';

        if (!endsWith(key, suffix))
            return;

        var p = key.substr(0, key.length - suffix.length);

        if (isEmptyOrNull(p))
            return;

        if (knownTypes[p] != null)
            return;

        knownTypes[p] = knownTypes[key];
    }
}