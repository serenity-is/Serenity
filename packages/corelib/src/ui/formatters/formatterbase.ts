import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, FormatterTypeInfo, registerType, StringLiteral, typeInfoProperty } from "../../base";
import { Formatter } from "../../slick";

export abstract class FormatterBase implements Formatter {
    abstract format(ctx: FormatterContext): FormatterResult;

    protected static registerFormatter<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): FormatterTypeInfo<TypeName> {
        if (Object.prototype.hasOwnProperty.call(this, typeInfoProperty) && this[typeInfoProperty])
            throw new Error(`Type ${this.name} already has a typeInfo property!`);
                
        const typeInfo = this.typeInfo = formatterTypeInfo(typeName, intfAndAttr);
        registerType(this);
        return typeInfo;
    }

    static typeInfo: FormatterTypeInfo<"Serenity.FormatterBase">;
}
