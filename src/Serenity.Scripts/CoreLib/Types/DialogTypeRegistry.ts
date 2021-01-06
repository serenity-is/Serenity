import { Config } from "../Q/Config";
import { Exception } from "../Q/System";
import { IDialog } from "../Interfaces";
import { notifyError } from "../Q/Notify";
import { endsWith } from "../Q/Strings";
import { getType, isAssignableFrom } from "../Q/System";

export namespace DialogTypeRegistry {

    function search(typeName: string) {

        var dialogType = getType(typeName);
        if (dialogType != null && isAssignableFrom(IDialog, dialogType)) {
            return dialogType;
        }

        for (var ns of Config.rootNamespaces) {
            dialogType = getType(ns + '.' + typeName);
            if (dialogType != null && isAssignableFrom(IDialog, dialogType)) {
                return dialogType;
            }
        }
        return null;
    }

    var knownTypes: { [key: string]: any } = {};

    export function tryGet(key: string): any {
        if (knownTypes[key] == null) {
            var typeName = key;
            var dialogType = search(typeName);

            if (dialogType == null && !endsWith(key, 'Dialog')) {
                typeName = key + 'Dialog';
                dialogType = search(typeName);
            }

            if (dialogType == null) {
                return null;
            }

            knownTypes[key] = dialogType as any;
            return dialogType as any;
        }

        return knownTypes[key];
    }

    export function get(key: string): any {

        var type = tryGet(key);

        if (type == null) {
            var message = key + ' dialog class is not found! Make sure there is a dialog class with this name, ' +
                'it is under your project root namespace, and your namespace parts start with capital letters, ' +
                'e.g. MyProject.Pascal.Cased namespace. If you got this error from an editor with InplaceAdd option ' +
                'check that lookup key and dialog type name matches (case sensitive, excluding Dialog suffix). ' +
                "You need to change lookup key or specify DialogType property in LookupEditor attribute if that's not the case.";

            notifyError(message, '', null);

            throw new Exception(message);
        }

        return type;
    }
}