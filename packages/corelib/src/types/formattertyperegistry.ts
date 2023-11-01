import { ArgumentNullException, Exception, htmlEncode, isAssignableFrom, isEmptyOrNull, ISlickFormatter, notifyError } from "../q";
import { commonTypeRegistry } from "./commontyperegistry";

export namespace FormatterTypeRegistry {

    let registry = commonTypeRegistry(
        type => isAssignableFrom(ISlickFormatter, type), 
        null, "Formatter");

    export function get(key: string): any {
        if (isEmptyOrNull(key)) 
            throw new ArgumentNullException('key');
        
        var type = registry.tryGet(key);
        if (type)
            return type;

        var message = `"${htmlEncode(key)}" formatter class not found! 
Make sure there is such a formatter type under the project root namespace,
and its namespace parts start with capital letters like MyProject.MyModule.MyFormatter.

If using ES modules, make sure the formatter type has a decorator like 
@Decorators.registerFormatter('MyProject.MyModule.MyFormatter') with the full name 
of your formatter type and "side-effect-import" this formatter class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyFormatter.ts").

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