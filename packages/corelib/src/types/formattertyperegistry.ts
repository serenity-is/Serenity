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
        const message = `"${htmlEncode(key)}" formatter class not found! 

Make sure the formatter type has a line like the following (with the correct full name):
static [Symbol.typeInfo] = formatterTypeInfo("MyProject.MyModule.MyEditor"); static { registerType(this); }
or a legacy decorator like @Decorators.registerFormatter('MyProject.MyModule.MyFormatter') with the full name         
and "side-effect-import" this formatter class from the current 
"page.ts/grid.ts/dialog.ts file (import "./path/to/MyFormatter.ts").

After applying fixes, build and run "node ./tsbuild.js" (or "tsc" if using namespaces) 
from the project folder.`;

        notifyError(message.replace(/\r?\n\r?\n/g, '<br/><br/>'), '', { escapeHtml: false, timeOut: 5000 });
        throw new Error(message);
    }
}

export const FormatterTypeRegistry = new FormatterTypeRegistryImpl();