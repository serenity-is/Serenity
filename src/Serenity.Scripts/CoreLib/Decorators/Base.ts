import { setTypeName, types } from "../Q/TypeSystem"
import { ISlickFormatter } from "../UI/DataGrid/ISlickFormatter";

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
        setTypeName(target, name);
        types[name] = target;
    }
    else if (!target.__typeName)
        target.__register = true;
    else 
        types[target.__typeName] = target;

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

export function addAttribute(type: any, attr: any) {
    type.__metadata = type.__metadata || {};
    type.__metadata.attr = type.__metadata.attr || [];
    type.__metadata.attr.push(attr);
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

export function registerEditor(nameOrIntf?: string | any[], intf2?: any[]) {
    return registerClass(nameOrIntf, intf2);
}


export function registerFormatter(nameOrIntf: string | any[] = [ISlickFormatter], intf2: any[] = [ISlickFormatter]) {
    return registerClass(nameOrIntf, intf2);
}