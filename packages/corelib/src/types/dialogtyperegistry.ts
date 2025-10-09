import { isAssignableFrom, notifyError } from "../base";
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
        const message = `The dialog class "${key}" was not found.

Ensure that the dialog type includes a line similar to the following (using the correct full name):
static [Symbol.typeInfo] = this.registerClass("MyProject.MyModule.MyDialog");

Also, side-effect import this dialog class from the current page.ts, grid.ts, or dialog.ts file. For example:
import "./path/to/MyDialog.ts";

If you encounter this error from an editor with the InplaceAdd option, verify that the lookup key and dialog type name match case-sensitively, excluding the "Dialog" suffix. Specify the DialogType property in the LookupEditor attribute if it does not match.

After applying the fixes, build the project by running "npm run build" from the project folder.`;

        notifyError(message, '', { preWrap: true, timeOut: 5000 });
        throw new Error(message);
    }
}

export const DialogTypeRegistry = new DialogTypeRegistryImpl();