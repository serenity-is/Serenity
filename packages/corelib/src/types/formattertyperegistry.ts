import { ISlickFormatter, htmlEncode, isAssignableFrom, notifyError } from "../base";
import { BaseTypeRegistry } from "./basetyperegistry";
import { FormatterType } from "./formattertype";

class FormatterTypeRegistryImpl extends BaseTypeRegistry<FormatterType> {
    constructor() {
        super({
            loadKind: "formatter",
            defaultSuffix: "Formatter"
        });
    }

    protected override isMatchingType(type: any): boolean {
        return isAssignableFrom(ISlickFormatter, type);
    }

    protected override loadError(key: string) {
        const message = `The formatter class "${key}" was not found.

Ensure that the formatter type includes a line similar to the following (using the correct full name):
static [Symbol.typeInfo] = formatterTypeInfo("MyProject.MyModule.MyEditor"); static { registerType(this); }

Alternatively, use a legacy decorator such as:
@Decorators.registerFormatter('MyProject.MyModule.MyFormatter')

Make sure to side-effect import this formatter class from the current page.ts, grid.ts, or dialog.ts file. For example:
import "./path/to/MyFormatter.ts";

After applying the fixes, build the project by running "npm run build" from the project folder.`;

        notifyError(message, '', { preWrap: true, timeOut: 5000 });
        throw new Error(message);
    }
}

export const FormatterTypeRegistry = new FormatterTypeRegistryImpl();