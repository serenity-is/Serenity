import { htmlEncode, isAssignableFrom, notifyError } from "../base";
import { IDialog } from "../interfaces";
import { BaseTypeRegistry } from "./basetyperegistry";
import { DialogType } from "./dialogtype";

class DialogTypeRegistryImpl extends BaseTypeRegistry<DialogType> {
    constructor() {
        super({
            loadKind: "dialog",
            defaultSuffix: "Dialog"
        });
    }

    protected override isMatchingType(type: any): boolean {
        return isAssignableFrom(IDialog, type);
    }

    protected override loadError(key: string) {
        const message = `"${htmlEncode(key)}" dialog class not found! 
Make sure the dialog type has a line like the following (with the correct full name):
static [Symbol.typeInfo] = this.registerClass("MyProject.MyModule.MyDialog");
and "side-effect-import" this dialog class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyDialog.ts").

If you had this error from an editor with the InplaceAdd option, verify that the lookup key
and dialog type name match case-sensitively, excluding the Dialog suffix.
Specify the DialogType property in the LookupEditor attribute if it is not.

After applying fixes, build and run "node ./tsbuild.js" (or "tsc" if using namespaces) 
from the project folder.`;

        notifyError(message.replace(/\r?\n\r?\n/g, '<br/><br/>'), '', { escapeHtml: false, timeOut: 5000 });
        throw new Error(message);
    }
}

export const DialogTypeRegistry = new DialogTypeRegistryImpl();