import { getCustomAttribute, htmlEncode, isEnum, notifyError } from "../base";
import { EnumKeyAttribute } from "./attributes";
import { BaseTypeRegistry } from "./basetyperegistry";

class EnumTypeRegistryImpl extends BaseTypeRegistry<object> {
    constructor() {
        super({
            loadKind: "enum",
            defaultSuffix: null
        });
    }

    protected override getSecondaryTypeKey(type: any): string {
        return getCustomAttribute(type, EnumKeyAttribute, false)?.value;
    }

    protected override isMatchingType(type: any): boolean {
        return isEnum(type);
    }

    protected override loadError(key: string) {
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
            throw new Error(message);
    }
}

export const EnumTypeRegistry = new EnumTypeRegistryImpl();