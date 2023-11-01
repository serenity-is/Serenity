import { Exception, getAttributes, htmlEncode, isEnum, notifyError } from "../q";
import { EnumKeyAttribute } from "../decorators";
import { commonTypeRegistry } from "./commontyperegistry";

export namespace EnumTypeRegistry {

    let registry = commonTypeRegistry(
        isEnum, 
        type => getAttributes(type, EnumKeyAttribute)[0]?.value,
        null);

    export function get(key: string): Function {
        var type = registry.tryGet(key);
        if (type)
            return type;

        var message = `Can't find "${htmlEncode(key)}" enum type! 
If you have recently defined this enum type in server side code, make sure 
your project builds successfully and run "dotnet sergen t". 

Also verify there is such an enum type under the project root namespace, 
and its namespace parts start with capital letters like MyProject.MyModule.MyEnum.

If using ES modules, make sure the enum type is registered with 
Q.registerEnum('MyProject.MyModule.MyDialog') with the full name of 
your enum type and "side-effect-import" this enum from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyEnum.ts").

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