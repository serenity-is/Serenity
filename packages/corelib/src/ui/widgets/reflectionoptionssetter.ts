import { DisplayNameAttribute, OptionAttribute } from "../../decorators";
import { getInstanceType, getMembers, isInstanceOfType, MemberType } from "@serenity-is/corelib/q";
import { ReflectionUtils } from "../../types/reflectionutils";

export namespace ReflectionOptionsSetter {
    export function set(target: any, options: any): void {
        if (options == null) {
            return;
        }

        var type = getInstanceType(target);

        if (type === Object) {
            return;
        }

        var propByName = type.__propByName;
        var fieldByName = type.__fieldByName;
        if (propByName == null) {
            var props = getMembers(type, MemberType.property);
            var propList = props.filter(function (x: any) {
                return !!x.setter && ((x.attr || []).filter(function (a: any) {
                    return isInstanceOfType(a, OptionAttribute);
                }).length > 0 || (x.attr || []).filter(function (a: any) {
                    return isInstanceOfType(a, DisplayNameAttribute);
                }).length > 0);
            });

            propByName = {};
            for (var k of propList) {
                propByName[ReflectionUtils.makeCamelCase(k.name)] = k;
            }

            type.__propByName = propByName;
        }

        if (fieldByName == null) {
            var fields = getMembers(type, MemberType.field);
            var fieldList = fields.filter(function (x1: any) {
                return (x1.attr || []).filter(function (a: any) {
                    return isInstanceOfType(a, OptionAttribute);
                }).length > 0 || (x1.attr || []).filter(function (a: any) {
                    return isInstanceOfType(a, DisplayNameAttribute);
                }).length > 0;
            });

            fieldByName = {};
            for (var $t2 = 0; $t2 < fieldList.length; $t2++) {
                var k1 = fieldList[$t2];
                fieldByName[ReflectionUtils.makeCamelCase(k1.name)] = k1;
            }
            type.__fieldByName = fieldByName;
        }

        var keys = Object.keys(options);
        for (var k2 of keys) {
            var v = options[k2];
            var cc = ReflectionUtils.makeCamelCase(k2);
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