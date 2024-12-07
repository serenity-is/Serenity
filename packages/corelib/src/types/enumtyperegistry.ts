import { getCustomAttribute, htmlEncode, isEnum, notifyError } from "../base";
import { Exception } from "../compat";
import { EnumKeyAttribute } from "./attributes";
import { commonTypeRegistry } from "./commontyperegistry";

export namespace EnumTypeRegistry {

    let registry = commonTypeRegistry<object>({
        attrKey: type => getCustomAttribute(type, EnumKeyAttribute, false)?.value,
        isMatch: isEnum,
        kind: "enum",
        suffix: null,
        loadError: function (key: string) {
            var message = `Can't find "${htmlEncode(key)}" enum type! 

If you have recently defined this enum type in server side code, 
make sure your project builds successfully and run "dotnet sergen t".

Also verify there is such an enum type under the project root namespace,
and its namespace parts start with capital letters like MyProject.MyModule.MyEnum.

If using ES modules, make sure the enum type is registered with 
Serenity.registerEnum('MyProject.MyModule.MyDialog') with the full name of 
your enum type and "side-effect-import" this enum from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyEnum.ts").

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