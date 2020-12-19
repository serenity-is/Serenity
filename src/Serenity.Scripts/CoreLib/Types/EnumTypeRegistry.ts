import { Config } from "../Q/Config";
import { Exception } from "../Q/Exceptions";
import { notifyError } from "../Q/Notify";
import { format } from "../Q/Formatting";
import { startsWith } from "../Q/Strings";
import { getAttributes, getTypes, getTypeFullName, isEnum } from "../Q/TypeSystem";
import { EnumKeyAttribute } from "./Attributes";

export namespace EnumTypeRegistry {

    let knownTypes: { [key: string]: Function }

    export function tryGet(key: string): Function {

        if (knownTypes == null) {
            knownTypes = {};
            for (var type of getTypes()) {
                if (isEnum(type)) {
                    var fullName = getTypeFullName(type);
                    knownTypes[fullName] = type;
                    var enumKeyAttr = getAttributes(type, EnumKeyAttribute, false);
                    if (enumKeyAttr != null && enumKeyAttr.length > 0) {
                        knownTypes[enumKeyAttr[0].value] = type;
                    }

                    for (var k of Config.rootNamespaces) {
                        if (startsWith(fullName, k + '.')) {
                            knownTypes[fullName.substr(k.length + 1)] = type;
                        }
                    }
                }
            }
        }

        if (knownTypes[key] == null)
            return null;

        return knownTypes[key];
    }

    export function get(key: string): Function {
        var type = EnumTypeRegistry.tryGet(key);
        if (type == null) {
            var message = format("Can't find {0} enum type! If you have recently defined this enum type " + 
                "in server side code, make sure your project builds successfully and transform T4 templates. " +
                "Also make sure that enum is under your project root namespace, and your namespace parts starts " + 
                "with capital letters, e.g.MyProject.Pascal.Cased namespace", key);

            notifyError(message, '', null);
            throw new Exception(message);
        }

        return type;
    }
}