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
        const message = `"${htmlEncode(key)}" editor class not found! 
Make sure the editor type has a line like the following (with the correct full name):
static [Symbol.typeInfo] = this.registerEditor("MyProject.MyModule.MyEditor");            
and "side-effect-import" this editor class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyEditor.ts").

After applying fixes, build and run "node ./tsbuild.js" (or "tsc" if using namespaces) 
from the project folder.`;

        notifyError(message.replace(/\r?\n\r?\n/g, '<br/><br/>'), '', { escapeHtml: false, timeOut: 5000 });
        throw new Error(message);
    }
}

export const EditorTypeRegistry = new EditorTypeRegistryImpl();
