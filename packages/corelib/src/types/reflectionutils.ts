import { isEmptyOrNull } from "../q";

export namespace ReflectionUtils {

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

    export function makeCamelCase(s: string): string {
        if (isEmptyOrNull(s)) {
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
            return s.substr(0, numUppercaseChars - 1).toLowerCase() + s.substr(numUppercaseChars - 1);
        }
        else if (s.length === 1) {
            return s.toLowerCase();
        }
        else {
            return s.substr(0, 1).toLowerCase() + s.substr(1);
        }
    }
}