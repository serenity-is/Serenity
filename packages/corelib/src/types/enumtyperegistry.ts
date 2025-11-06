import { EnumKeyAttribute, getCustomAttribute, htmlEncode, isEnum, notifyError } from "../base";
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
        const message = `The enum type "${key}" was not found.

If you have recently defined this enum type in server-side code, ensure that your project builds successfully, then run "dotnet sergen servertypings".

Also, verify that there is such an enum type under the project root namespace, and its namespace parts start with capital letters, like MyProject.MyModule.MyEnum.

If using ES modules, make sure the enum type is registered with Serenity.registerEnum('MyProject.MyModule.MyEnum') (using the full name of your enum type), and side-effect import this enum from the current page.ts, grid.ts, or dialog.ts file. For example:
import "./path/to/MyEnum.ts";

After applying the fixes, build the project by running "npm run build" from the project folder.`;

        notifyError(message, '', { preWrap: true, timeOut: 5000 });
        throw new Error(message);
    }
}

export const EnumTypeRegistry = new EnumTypeRegistryImpl();