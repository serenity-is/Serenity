namespace Serenity {

    export namespace Decorators {
        function distinct(arr: any[]) {
            return arr.filter((item, pos) => arr.indexOf(item) === pos);
        }

        function merge(arr1: any[], arr2: any[]) {
            if (!arr1 || !arr2)
                return (arr1 || arr2 || []).slice();

            return distinct(arr1.concat(arr2));
        }

        function registerType(target: any, name: string, intf: any[]) {
            if (name != null) {
                target.__typeName = name;
                Q.types[name] = target;
            }
            else if (!target.__typeName)
                target.__register = true;

            if (intf)
                target.__interfaces = merge(target.__interfaces, intf);
        }

        export function registerClass(nameOrIntf?: string | any[], intf2?: any[]) {
            return function (target: Function) {
                if (typeof nameOrIntf == "string")
                    registerType(target, nameOrIntf, intf2);
                else
                    registerType(target, null, nameOrIntf);

                (target as any).__class = true;
            }
        }

        export function registerInterface(nameOrIntf?: string | any[], intf2?: any[]) {
            return function (target: Function) {

                if (typeof nameOrIntf == "string")
                    registerType(target, nameOrIntf, intf2);
                else
                    registerType(target, null, nameOrIntf);

                (target as any).__interface = true;
                (target as any).isAssignableFrom = function (type: any) {
                    return type.__interfaces != null && type.__interfaces.indexOf(this) >= 0;
                };
            }
        }

        export function addAttribute(type: any, attr: any) {
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }
    }

    
    @Decorators.registerInterface('Serenity.ISlickFormatter')
    export class ISlickFormatter {
    }

    function Attr(name: string) {
        return Decorators.registerClass('Serenity.' + name + 'Attribute')
    }

    export namespace Decorators {

        export function enumKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new EnumKeyAttribute(value));
            }
        }

        export function registerEnum(target: any, enumKey?: string, name?: string) {
            if (!target.__enum) {
                Object.defineProperty(target, '__enum', {
                    get: function () {
                        return true;
                    }
                });

                target.prototype = target.prototype || {};
                for (var k of Object.keys(target))
                    if (isNaN(Q.parseInteger(k)) && target[k] != null && !isNaN(Q.parseInteger(target[k])))
                        target.prototype[k] = target[k];

                if (name != null) {
                    target.__typeName = name;
                    Q.types[name] = target;
                }
                else if (!target.__typeName)
                    target.__register = true;

                if (enumKey)
                    addAttribute(target, new EnumKeyAttribute(enumKey));
            }
        }

        export function registerEnumType(target: any, name?: string, enumKey?: string) {
            registerEnum(target, Q.coalesce(enumKey, name), name);
        }
    }
    
    @Attr('EnumKey')
    export class EnumKeyAttribute {
        constructor(public value: string) {
        }
    }
}

