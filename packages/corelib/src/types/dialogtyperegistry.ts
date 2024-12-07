import { htmlEncode, isAssignableFrom, notifyError } from "../base";
import { IDialog } from "../interfaces";
import { Exception } from "../compat";
import { commonTypeRegistry } from "./commontyperegistry";
import { DialogType } from "./dialogtype";

export namespace DialogTypeRegistry {

    const registry = commonTypeRegistry<DialogType>({
        attrKey: null,
        isMatch: type => isAssignableFrom(IDialog, type),
        kind: "dialog",
        suffix: "Dialog",
        loadError: function (key: string) {
            var message = `"${htmlEncode(key)}" dialog class not found! 
Make sure there is such a dialog type under the project root namespace, 
and its namespace parts start with capital letters like MyProject.MyModule.MyDialog.

If using ES modules, make sure the dialog type has a decorator like 
@Decorators.registerClass('MyProject.MyModule.MyDialog') with the full name
and "side-effect-import" this dialog class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyDialog.ts").

If you had this error from an editor with the InplaceAdd option, verify that the lookup key
and dialog type name match case-sensitively, excluding the Dialog suffix.
Specify the DialogType property in the LookupEditor attribute if it is not.

After applying fixes, build and run "node ./tsbuild.js" (or "tsc" if using namespaces) 
from the project folder.`;

            notifyError(message.replace(/\r?\n\r?\n/g, '<br/><br/>'), '', { escapeHtml: false, timeOut: 5000 });
            throw new Exception(message);
        }
    });

    export let get = registry.get;
    export let getOrLoad = registry.getOrLoad;
    export let reset = registry.reset;
    export let tryGet = registry.tryGet;
    export let tryGetOrLoad = registry.tryGetOrLoad;
}