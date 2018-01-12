namespace Serenity {
    export namespace Decorators {

        export function addAttribute(type: any, attr: any) {
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }

        export function addMemberAttr(type: any, memberName: string, attr: any) {
            type.__metadata = type.__metadata || {};
            type.__metadata.members = type.__metadata.members || [];
            let member: any = undefined;
            for (var m of type.__metadata.members) {
                if (m.name == memberName) {
                    member = m;
                    break;
                }
            }

            if (!member) {
                member = { name: memberName, attr: [], type: 4, returnType: Object, sname: memberName };
                type.__metadata.members.push(member);
            }

            member.attr = member.attr || [];
            member.attr.push(attr);
        }

        export function columnsKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new ColumnsKeyAttribute(value));
            }
        }

        export function dialogType(value: Function) {
            return function (target: Function) {
                addAttribute(target, new DialogTypeAttribute(value));
            }
        }

        export function editor(key?: string) {
            return function (target: Function) {
                var attr = new EditorAttribute();
                if (key !== undefined)
                    attr.key = key;
                addAttribute(target, attr);
            }
        }

        export function element(value: string) {
            return function (target: Function) {
                addAttribute(target, new ElementAttribute(value));
            }
        }

        export function entityType(value: string) {
            return function (target: Function) {
                addAttribute(target, new EntityTypeAttribute(value));
            }
        }

        export function enumKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new EnumKeyAttribute(value));
            }
        }

        export function flexify(value = true) {
            return function (target: Function) {
                addAttribute(target, new FlexifyAttribute(value));
            }
        }

        export function formKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new FormKeyAttribute(value));
            }
        }

        export function generatedCode(origin?: string) {
            return function (target: Function) {
                addAttribute(target, new GeneratedCodeAttribute(origin));
            }
        }

        export function idProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new IdPropertyAttribute(value));
            }
        }

        function distinct(arr: any[]) {
            return arr.filter((item, pos) => arr.indexOf(item) === pos);
        }

        function merge(arr1: any[], arr2: any[]) {
            if (!arr1 || !arr2)
                return (arr1 || arr2 || []).slice();

            return distinct(arr1.concat(arr2));
        }

        function registerType(target: any, name: string, intf: any[]) {
            if (name != null)
                target.__typeName = name;
            else if (!target.__typeName)
                target.__register = true;

            if (intf)
                target.__interfaces = merge(target.__interfaces, intf);

            if (!target.__assembly)
                target.__assembly = ss.__assemblies['App'];

            if (target.__typeName)
                target.__assembly.__types[target.__typeName] = target;
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
                    return (ss as any).contains((ss as any).getInterfaces(type), this);
                };
            }
        }

        export function registerEnum(target: any, enumKey?: string, typeName?: string, asm?: ss.AssemblyReg) {
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

                if (Q.coalesce(typeName, enumKey))
                    target.__typeName = Q.coalesce(typeName, enumKey);
                else
                    target.__register = true;

                if (enumKey)
                    addAttribute(target, new Serenity.EnumKeyAttribute(enumKey));
            }
        }

        export function registerEditor(nameOrIntf?: string | any[], intf2?: any[]) {
            return registerClass(nameOrIntf, intf2);
        }

        export function registerFormatter(nameOrIntf: string | any[] = [Serenity.ISlickFormatter], intf2: any[] = [Serenity.ISlickFormatter]) {
            return registerClass(nameOrIntf, intf2);
        }

        export function filterable(value = true) {
            return function (target: Function) {
                addAttribute(target, new FilterableAttribute(value));
            }
        }

        export function itemName(value: string) {
            return function (target: Function) {
                addAttribute(target, new ItemNameAttribute(value));
            }
        }

        export function isActiveProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new IsActivePropertyAttribute(value));
            }
        }

        export function localTextPrefix(value: string) {
            return function (target: Function) {
                addAttribute(target, new LocalTextPrefixAttribute(value));
            }
        }

        export function maximizable(value = true) {
            return function (target: Function) {
                addAttribute(target, new MaximizableAttribute(value));
            }
        }

        export function nameProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new NamePropertyAttribute(value));
            }
        }

        export function option() {
            return function (target: Object, propertyKey: string): void {
                addMemberAttr(target.constructor, propertyKey, new OptionAttribute());
            }
        }

        export function optionsType(value: Function) {
            return function (target: Function) {
                addAttribute(target, new OptionsTypeAttribute(value));
            }
        }

        export function panel(value = true) {
            return function (target: Function) {
                addAttribute(target, new PanelAttribute(value));
            }
        }

        export function resizable(value = true) {
            return function (target: Function) {
                addAttribute(target, new ResizableAttribute(value));
            }
        }

        export function responsive(value = true) {
            return function (target: Function) {
                addAttribute(target, new ResponsiveAttribute(value));
            }
        }

        export function service(value: string) {
            return function (target: Function) {
                addAttribute(target, new ServiceAttribute(value));
            }
        }
    }
}

