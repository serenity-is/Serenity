import { ArgumentNullException, EditorAttribute, Exception, getAttributes, htmlEncode, isAssignableFrom, isEmptyOrNull, notifyError } from "../q";
import { Widget } from "../ui/widgets/widget";
import { commonTypeRegistry } from "./commontyperegistry";

let knownTypes: { [key: string] : any };

export namespace EditorTypeRegistry {
    
    let registry = commonTypeRegistry(
        type => !!getAttributes(type, EditorAttribute).length || isAssignableFrom(Widget, type), 
        null, "Editor");

    export function get(key: string): any {
        if (isEmptyOrNull(key)) 
            throw new ArgumentNullException('key');
        
        var type = registry.tryGet(key);
        if (type)
            return type;

        var message = `"${htmlEncode(key)}" editor class not found! 
Make sure there is such a editor type under the project root namespace,
and its namespace parts start with capital letters like MyProject.MyModule.MyEditor.

If using ES modules, make sure the editor type has a decorator like 
@Decorators.registerEditor('MyProject.MyModule.MyFormatter') with the full name 
and "side-effect-import" this editor class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyEditor.ts").

After applying fixes, build and run "node ./tsbuild.js" (or "tsc" if using namespaces) 
from the project folder.`;

        notifyError(message.replace(/\r?\n\r?\n/g, '<br/><br/>'), '', { escapeHtml: false, timeOut: 5000 });
        throw new Exception(message);
    }

    export function reset() {
        registry.reset();
    }

    export function tryGet(key: string) {
        return registry.tryGet(key);
    }
}