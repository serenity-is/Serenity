import { ISlickFormatter, htmlEncode, isAssignableFrom, notifyError } from "../base";
import { Exception } from "../compat";
import { commonTypeRegistry } from "./commontyperegistry";
import { FormatterType } from "./formattertype";

export namespace FormatterTypeRegistry {

    const registry = commonTypeRegistry<FormatterType>({
        attrKey: null,
        isMatch: type => isAssignableFrom(ISlickFormatter, type),
        kind: "formatter",
        suffix: "Formatter",
        loadError: function (key: string) {
            var message = `"${htmlEncode(key)}" formatter class not found! 

Make sure the formatter type has a line like the following (with the correct full name):
static typeInfo = formatterTypeInfo("MyProject.MyModule.MyEditor"); static { registerType(this); }
or a legacy decorator like @Decorators.registerFormatter('MyProject.MyModule.MyFormatter') with the full name         
and "side-effect-import" this formatter class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyFormatter.ts").

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