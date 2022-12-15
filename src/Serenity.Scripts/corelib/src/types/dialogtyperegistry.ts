import { Config, Exception, notifyError, endsWith, getType, isAssignableFrom, isEmptyOrNull, ArgumentNullException, htmlEncode } from "@serenity-is/corelib/q";
import { IDialog } from "../interfaces";

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

    let knownTypes: { [key: string]: any } = {};

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

        if (isEmptyOrNull(key)) 
            throw new ArgumentNullException('key');

        var type = tryGet(key);

        if (type)
            return type;

        var message = `"${htmlEncode(key)}" dialog class not found! 
Make sure there is such a dialog type under the project root namespace, 
and its namespace parts start with capital letters like MyProject.MyModule.MyDialog.

If using ES modules, make sure the dialog type has a decorator like 
@Decorators.registerClass('MyProject.MyModule.MyDialog') with the full name of 
your dialog type and "side-effect-import" this dialog class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyDialog.ts").

If you had this error from an editor with the InplaceAdd option, verify that the lookup key
and dialog type name match case-sensitively, excluding the Dialog suffix.
Specify the DialogType property in the LookupEditor attribute if it is not.

After applying fixes, build and run "node ./tsbuild.js" (or "tsc" if using namespaces) 
from the project folder.`;

        notifyError(message.replace(/\r?\n\r?\n/g, '<br/><br/>'), '', { escapeHtml: false, timeOut: 5000 });
        throw new Exception(message);
    }

    export function reset() {
        knownTypes = null;
    }
}