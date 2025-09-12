import { EditorAttribute, hasCustomAttribute, htmlEncode, isAssignableFrom, notifyError } from "../base";
import { Exception } from "../compat";
import { Widget } from "../ui/widgets/widget";
import { commonTypeRegistry } from "./commontyperegistry";
import { EditorType } from "./editortype";

export namespace EditorTypeRegistry {

    const registry = commonTypeRegistry<EditorType>({
        attrKey: null,
        isMatch: type => hasCustomAttribute(type, EditorAttribute, false) || isAssignableFrom(Widget, type),
        kind: "editor",
        suffix: "Editor",
        loadError: function (key: string) {
            var message = `"${htmlEncode(key)}" editor class not found! 
Make sure the editor type has a line like the following (with the correct full name):
static override typeInfo = this.registerEditor("MyProject.MyModule.MyEditor");            
and "side-effect-import" this editor class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyEditor.ts").

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