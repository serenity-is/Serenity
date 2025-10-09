import { EditorAttribute, hasCustomAttribute, htmlEncode, isAssignableFrom, notifyError } from "../base";
import { Widget } from "../ui/widgets/widget";
import { BaseTypeRegistry } from "./basetyperegistry";
import { EditorType } from "./editortype";

class EditorTypeRegistryImpl extends BaseTypeRegistry<EditorType> {
    constructor() {
        super({
            loadKind: "editor",
            defaultSuffix: "Editor"
        });
    }

    protected override isMatchingType(type: any): boolean {
        return hasCustomAttribute(type, EditorAttribute, false) ||
            isAssignableFrom(Widget, type);
    }

    protected override loadError(key: string) {
        const message = `The editor class "${key}" was not found.

Ensure that the editor type includes a line similar to the following (using the correct full name):
static [Symbol.typeInfo] = this.registerEditor("MyProject.MyModule.MyEditor");

Also, side-effect import this editor class from the current page.ts, grid.ts, or dialog.ts file. For example:
import "./path/to/MyEditor.ts";

After applying the fixes, build the project by running "npm run build" from the project folder.`;

        notifyError(message, '', { preWrap: true, timeOut: 5000 });
        throw new Error(message);
    }
}

export const EditorTypeRegistry = new EditorTypeRegistryImpl();
