import { MemberType, types } from "../Q/TypeSystem";
import { startsWith } from "../Q/Strings";
import { addAttribute, registerInterface } from "./Base";
import { DialogTypeAttribute, EditorAttribute, ElementAttribute, EnumKeyAttribute, FilterableAttribute,
         FlexifyAttribute, ItemNameAttribute, MaximizableAttribute, OptionsTypeAttribute,
         PanelAttribute, ResizableAttribute, ResponsiveAttribute, ServiceAttribute } from "../Types/Attributes";
import { ISlickFormatter } from "../UI/DataGrid/ISlickFormatter";
import { OptionAttribute } from "../Types/Attributes";

export function registerEnum(target: any, enumKey?: string, name?: string) {
    if (!target.__enum) {
        Object.defineProperty(target, '__enum', {
            get: function () {
                return true;
            }
        });

        target.prototype = target.prototype || {};
        for (var k of Object.keys(target))
            if (isNaN(parseInt(k)) && target[k] != null && !isNaN(parseInt(target[k])))
                target.prototype[k] = target[k];

        if (name != null) {
            target.__typeName = name;
            types[name] = target;
        }
        else if (!target.__typeName)
            target.__register = true;

        if (enumKey)
            addAttribute(target, new EnumKeyAttribute(enumKey));
    }
}

export function registerEnumType(target: any, name?: string, enumKey?: string) {
    registerEnum(target, enumKey ?? name, name);
}

export function enumKey(value: string) {
    return function (target: Function) {
        addAttribute(target, new EnumKeyAttribute(value));
    }
}

registerInterface('Serenity.ISlickFormatter')(ISlickFormatter);


enum SummaryType {
    Disabled = -1,
    None = 0,
    Sum = 1,
    Avg = 2,
    Min = 3,
    Max = 4
}

registerEnum(SummaryType, 'Serenity.SummaryType');

export function option() {
    return function (target: Object, propertyKey: string): void {

        var isGetSet = startsWith(propertyKey, 'get_') || startsWith(propertyKey, 'set_');
        var memberName = isGetSet ? propertyKey.substr(4) : propertyKey;

        var type: any = target.constructor;
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
            member = {
                attr: [new OptionAttribute()],
                name: memberName
            };

            if (isGetSet) {
                member.type = MemberType.property;

                member.getter = {
                    name: 'get_' + memberName
                };

                member.setter = {
                    name: 'set_' + memberName,
                };
            }
            else {
                member.type = MemberType.field;
            }

            type.__metadata.members.push(member);
        }
        else {
            member.attr = member.attr || [];
            member.attr.push(new OptionAttribute());
        }
    }
}

export function dialogType(value: any) {
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

export function filterable(value = true) {
    return function (target: Function) {
        addAttribute(target, new FilterableAttribute(value));
    }
}

export function flexify(value = true) {
    return function (target: Function) {
        addAttribute(target, new FlexifyAttribute(value));
    }
}

export function itemName(value: string) {
    return function (target: Function) {
        addAttribute(target, new ItemNameAttribute(value));
    }
}

export function maximizable(value = true) {
    return function (target: Function) {
        addAttribute(target, new MaximizableAttribute(value));
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