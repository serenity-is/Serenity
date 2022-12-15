import { ArgumentNullException, Config, endsWith, Exception, getType, isAssignableFrom, isEmptyOrNull, ISlickFormatter, notifyError } from "@serenity-is/corelib/q";


export namespace FormatterTypeRegistry {

    function search(typeName: string) {

        var type = getType(typeName);
        if (type != null && isAssignableFrom(ISlickFormatter, type)) {
            return type;
        }

        for (var ns of Config.rootNamespaces) {
            type = getType(ns + '.' + typeName);
            if (type != null && isAssignableFrom(ISlickFormatter, type)) {
                return type;
            }
        }
        return null;
    }

    let knownTypes: { [key: string]: any } = {};

    export function tryGet(key: string): any {
        if (knownTypes[key] == null) {
            var typeName = key;
            var type = search(typeName);

            if (type == null && !endsWith(key, 'Formatter')) {
                typeName = key + 'Formatter';
                type = search(typeName);
            }

            if (type == null) {
                return null;
            }

            knownTypes[key] = type as any;
            return type as any;
        }

        return knownTypes[key];
    }

    export function get(key: string): Function {
        if (isEmptyOrNull(key)) 
            throw new ArgumentNullException('key');
        
        var type = tryGet(key);
        if (type)
            return type;

        var message = `"${key}" formatter class not found! 
Make sure there is such a formatter type under the project root namespace,
and its namespace parts start with capital letters like MyProject.MyModule.MyFormatter.

If using ES modules, make sure the formatter type has a decorator like 
@Decorators.registerFormatter('MyProject.MyModule.MyFormatter') with the full name 
of your formatter type and "side-effect-import" this formatter class from the current 
"page.ts/grid.ts/dialog.ts file (import "./thepath/to/MyFormatter.ts").

After applying fixes, build and run "node ./tsbuild.js" (or "tsc" if using namespaces) 
from the project folder.`;

        notifyError(message.replace(/\r?\n\r?\n/g, '<br/><br/>'), '', { escapeHtml: false, timeOut: 5000 });
        throw new Exception(message);
    }

    export function reset() {
        knownTypes = null;
    }
}