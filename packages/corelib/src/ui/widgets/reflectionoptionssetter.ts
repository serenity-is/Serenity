import { getInstanceType, isInstanceOfType } from "../../base";
import { getTypeMembers, TypeMember, TypeMemberKind } from "../../compat";
import { OptionAttribute } from "../../types/attributes";

/**
 * Reflection-based helper that applies option objects to widgets using members
 * decorated with {@link OptionAttribute}. Handles PascalCase → camelCase mapping
 * and getter/setter (`get_*`/`set_*`) conventions.
 */
export namespace ReflectionOptionsSetter {

    /**
     * Gets a property value via `get_<property>` or direct field access.
     * @param o - Target object.
     * @param property - Property name (PascalCase or camelCase).
     * @returns Property value, or undefined if not found.
     */
    export function getPropertyValue(o: any, property: string): any {
        const d = o;
        let getter = d['get_' + property];
        if (typeof getter !== 'undefined') {
            return getter.apply(o);
        }
        const camelCase = makeCamelCase(property);
        getter = d['get_' + camelCase];
        if (typeof getter !== 'undefined') {
            return getter.apply(o);
        }
        return d[camelCase];
    }

    /**
     * Sets a property value via `set_<property>` or direct field assignment.
     * @param o - Target object.
     * @param property - Property name (PascalCase or camelCase).
     * @param value - Value to assign.
     */
    export function setPropertyValue(o: any, property: string, value: any): void {
        const d = o;
        let setter = d['set_' + property];
        if (typeof setter !== 'undefined') {
            setter.apply(o, [value]);
            return;
        }
        const camelCase = makeCamelCase(property);
        setter = d['set_' + camelCase];
        if (typeof setter !== 'undefined') {
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

        let hasNonUppercase = false;
        let numUppercaseChars = 0;
        for (let index = 0; index < s.length; index++) {
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

    /**
     * Applies an options bag to a widget instance by setting members
     * decorated with {@link OptionAttribute}.
     * @param target - Widget instance to configure.
     * @param options - Options object (keys are matched case-insensitively via camelCase conversion).
     */
    export function set(target: any, options: any): void {
        if (options == null) {
            return;
        }

        const type = getInstanceType(target);

        if (type === Object) {
            return;
        }

        const props = getTypeMembers(type, TypeMemberKind.property);
        const propList = props.filter(x => !!x.setter && x?.attr?.some(a => isInstanceOfType(a, OptionAttribute)));
        const propByName: Record<string, TypeMember> = {};
        for (const k of propList) {
            propByName[makeCamelCase(k.name)] = k;
        }

        const fields = getTypeMembers(type, TypeMemberKind.field);
        const fieldList = fields.filter(x => x.attr?.some(a => isInstanceOfType(a, OptionAttribute)));

        const fieldByName: Record<string, TypeMember> = {};
        for (let $t2 = 0; $t2 < fieldList.length; $t2++) {
            const k1 = fieldList[$t2];
            fieldByName[makeCamelCase(k1.name)] = k1;
        }

        const keys = Object.keys(options);
        for (const k2 of keys) {
            const v = options[k2];
            const cc = makeCamelCase(k2);
            const p = propByName[cc] || propByName[k2];
            if (p != null) {
                const func = (target[p.setter] as Function);
                func && func.call(target, v);
            }
            else {
                const f = fieldByName[cc] || fieldByName[k2];
                f && (target[f.name] = v);
            }
        }
    }
}