import { getInstanceType, isInstanceOfType } from "../../base";
import { TypeMember, TypeMemberKind, getTypeMembers } from "../../compat";
import { OptionAttribute } from "../../types/decorators";
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

        var props = getTypeMembers(type, TypeMemberKind.property);
        var propList = props.filter(x => !!x.setter && x?.attr?.some(a => isInstanceOfType(a, OptionAttribute)));
        var propByName: Record<string, TypeMember> = {};
        for (var k of propList) {
            propByName[ReflectionUtils.makeCamelCase(k.name)] = k;
        }

        var fields = getTypeMembers(type, TypeMemberKind.field);
        var fieldList = fields.filter(x => x.attr?.some(a => isInstanceOfType(a, OptionAttribute)));

        var fieldByName: Record<string, TypeMember> = {};
        for (var $t2 = 0; $t2 < fieldList.length; $t2++) {
            var k1 = fieldList[$t2];
            fieldByName[ReflectionUtils.makeCamelCase(k1.name)] = k1;
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