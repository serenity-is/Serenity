import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, FormatterTypeInfo, nsSerenity, registerType, StringLiteral, type AttributeSpecifier, type InterfaceType } from "../../base";
import { Formatter } from "../../slick";

export abstract class FormatterBase implements Formatter {
    abstract format(ctx: FormatterContext): FormatterResult;

    protected static registerFormatter<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): FormatterTypeInfo<TypeName> {
        if (Object.prototype.hasOwnProperty.call(this, Symbol.typeInfo) && this[Symbol.typeInfo])
            throw new Error(`Type ${this.name} already has a typeInfo property!`);

        const typeInfo = this[Symbol.typeInfo] = formatterTypeInfo(typeName, intfAndAttr);
        registerType(this);
        return typeInfo;
    }

    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }
}
