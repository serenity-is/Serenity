import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, FormatterTypeInfo, nsSerenity, registerType, StringLiteral, type AttributeSpecifier, type InterfaceType } from "../../base";
import { Formatter } from "../../slick";

/**
 * Base class for Serenity formatters. Provides the static `registerFormatter` helper
 * used by formatter subclasses with `static [Symbol.typeInfo] = this.registerFormatter(...)`.
 */
export abstract class FormatterBase implements Formatter {
    /**
     * Formats the cell value.
     * @param ctx - Formatter context containing item/column/value/grid.
     * @returns Rendered content for the cell.
     */
    abstract format(ctx: FormatterContext): FormatterResult;

    /**
     * Registers the formatter type under the given formal name.
     * @typeParam TypeName - String literal for the formatter's full name.
     * @param typeName - Full name (e.g. `"MyProject.MyFormatter"`).
     * @param intfAndAttr - Optional interfaces / attribute specifiers.
     * @returns The created type info.
     */
    protected static registerFormatter<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): FormatterTypeInfo<TypeName> {
        if (Object.prototype.hasOwnProperty.call(this, Symbol.typeInfo) && this[Symbol.typeInfo])
            throw new Error(`Type ${this.name} already has a typeInfo property!`);

        const typeInfo = this[Symbol.typeInfo] = formatterTypeInfo(typeName, intfAndAttr);
        registerType(this);
        return typeInfo;
    }

    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }
}
