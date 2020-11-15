namespace Serenity.DialogTypeRegistry {

    function search(typeName: string) {

        var dialogType = Q.getType(typeName);
        if (dialogType != null && Q.isAssignableFrom(Serenity.IDialog, dialogType)) {
            return dialogType;
        }

        for (var ns of Q.Config.rootNamespaces) {
            dialogType = Q.getType(ns + '.' + typeName);
            if (dialogType != null && Q.isAssignableFrom(Serenity.IDialog, dialogType)) {
                return dialogType;
            }
        }
        return null;
    }

    var knownTypes: Q.Dictionary<WidgetDialogClass> = {};

    export function tryGet(key: string): WidgetDialogClass {
        if (knownTypes[key] == null) {
            var typeName = key;
            var dialogType = search(typeName);

            if (dialogType == null && !Q.endsWith(key, 'Dialog')) {
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

    export function get(key: string): WidgetDialogClass {

        var type = tryGet(key);

        if (type == null) {
            var message = key + ' dialog class is not found! Make sure there is a dialog class with this name, ' +
                'it is under your project root namespace, and your namespace parts start with capital letters, ' +
                'e.g. MyProject.Pascal.Cased namespace. If you got this error from an editor with InplaceAdd option ' +
                'check that lookup key and dialog type name matches (case sensitive, excluding Dialog suffix). ' +
                "You need to change lookup key or specify DialogType property in LookupEditor attribute if that's not the case.";

            Q.notifyError(message, '', null);

            throw new Q.Exception(message);
        }

        return type;
    }
}