import { getInstanceType, isInstanceOfType } from "../../base";
import { getTypeMembers, TypeMember, TypeMemberKind } from "../../compat";
import { OptionAttribute } from "../../types/attributes";

export namespace ReflectionOptionsSetter {

    export function getPropertyValue(o: any, property: string): any {
        var d = o;
        var getter = d['get_' + property];
        if (!!!(typeof (getter) === 'undefined')) {
            return getter.apply(o);
        }
        var camelCase = makeCamelCase(property);
        getter = d['get_' + camelCase];
        if (!!!(typeof (getter) === 'undefined')) {
            return getter.apply(o);
        }
        return d[camelCase];
    }

    export function setPropertyValue(o: any, property: string, value: any): void {
        var d = o;
        var setter = d['set_' + property];
        if (!!!(typeof (setter) === 'undefined')) {
            setter.apply(o, [value]);
            return;
        }
        var camelCase = makeCamelCase(property);
        setter = d['set_' + camelCase];
        if (!!!(typeof (setter) === 'undefined')) {
            setter.apply(o, [value]);
            return;
        }
        d[camelCase] = value;
    }

    function makeCamelCase(s: string): string {
        if (!s) {
            return s;
        }

        if (s === 'ID') {
            return 'id';
        }

        var hasNonUppercase = false;
        var numUppercaseChars = 0;
        for (var index = 0; index < s.length; index++) {
            if (s.charCodeAt(index) >= 65 && s.charCodeAt(index) <= 90) {
                numUppercaseChars++;
            }
            else {
                hasNonUppercase = true;
                break;
            }
        }

        if (!hasNonUppercase && s.length !== 1 || numUppercaseChars === 0) {
            return s;
        }
        else if (numUppercaseChars > 1) {
            return s.substring(0, numUppercaseChars - 1).toLowerCase() + s.substring(numUppercaseChars - 1);
        }
        else if (s.length === 1) {
            return s.toLowerCase();
        }
        else {
            return s.substring(0, 1).toLowerCase() + s.substring(1);
        }
    }

    export function set(target: any, options: any): void {
        if (options == null) {
            return;
        }

        var type = getInstanceType(target);

        if (type === Object) {
            return;
        }

        var props = getTypeMembers(type, TypeMemberKind.property);
        var propList = props.filter(x => !!x.setter && x?.attr?.some(a => isInstanceOfType(a, OptionAttribute)));
        var propByName: Record<string, TypeMember> = {};
        for (var k of propList) {
            propByName[makeCamelCase(k.name)] = k;
        }

        var fields = getTypeMembers(type, TypeMemberKind.field);
        var fieldList = fields.filter(x => x.attr?.some(a => isInstanceOfType(a, OptionAttribute)));

        var fieldByName: Record<string, TypeMember> = {};
        for (var $t2 = 0; $t2 < fieldList.length; $t2++) {
            var k1 = fieldList[$t2];
            fieldByName[makeCamelCase(k1.name)] = k1;
        }

        var keys = Object.keys(options);
        for (var k2 of keys) {
            var v = options[k2];
            var cc = makeCamelCase(k2);
            var p = propByName[cc] || propByName[k2];
            if (p != null) {
                var func = (target[p.setter] as Function);
                func && func.call(target, v);
            }
            else {
                var f = fieldByName[cc] || fieldByName[k2];
                f && (target[f.name] = v);
            }
        }
    }
}