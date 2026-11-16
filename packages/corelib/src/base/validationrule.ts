import { classTypeInfo, nsSerenity, registerType, type ClassTypeInfo, type StringLiteral } from "./system";

/** What a validation rule is checked with, besides the value and its parameters. */
export interface ValidationRuleContext {
    /** The element being validated, e.g. the input of an editor. */
    element?: HTMLElement;
    /** Gets the value of another field of the form, e.g. for a rule comparing two fields. */
    getValue?: (field: string) => any;
}

/**
 * The base of a validation rule that the forms check, for the fields whose properties have a validation attribute on the
 * server with the type key of the rule as its rule name, e.g. `Serenity.Pro.DataSources.DefinitionCodeValidationRule`. It is
 * found by its key in the type registry, see ValidationRuleTypeRegistry, so implementing LazyLoadInclude lets it be loaded on
 * demand by the forms that use it, wherever they are, instead of being imported by them.
 * @example
 * ```ts
 * export class EvenValidationRule extends ValidationRule implements LazyLoadInclude {
 *     static override [Symbol.typeInfo] = this.registerRule("MyApp.");
 *     override textKey = "Validation.Even";
 *     override defaultText = "Please enter an even number.";
 *     check(value: any) { return Number(value) % 2 === 0; }
 * }
 * ```
 */
export abstract class ValidationRule {
    /**
     * The local text key of the message of the rule, which can have the parameters of the rule as `{0}`, `{1}`, used when
     * the attribute gives no message of its own.
     */
    textKey: string = "Validation.Invalid";

    /** The text of the message for when its key has no text, e.g. the English one. */
    defaultText?: string = "Please enter a valid value.";

    /**
     * Checks a value by the rule. It is not called for an empty value, i.e. null, an empty string or an empty list, as
     * being required is a rule of its own.
     * @param value - The value, e.g. a number for a decimal editor, or a text.
     * @param params - The parameters of the rule, as the server sends them.
     * @param context - The element and the other fields.
     * @returns Whether the value is valid.
     */
    abstract check(value: any, params: any, context: ValidationRuleContext): boolean;

    /**
     * Gets the local text key of the message, by default {@link textKey}, which a rule can choose by its parameters, e.g.
     * a length rule by whether it has a minimum.
     * @param params - The parameters of the rule.
     */
    getTextKey(params: any): string {
        return this.textKey;
    }

    /**
     * Gets the text of the message for when its key has no text, by default {@link defaultText}.
     * @param params - The parameters of the rule.
     */
    getDefaultText(params: any): string | undefined {
        return this.defaultText;
    }

    /**
     * Gets what the message has as `{0}`, `{1}` and so on, by default the parameters of the rule.
     * @param params - The parameters of the rule.
     */
    getMessageArgs(params: any): any[] {
        return Array.isArray(params) ? params : params == null ? [] : [params];
    }

    /**
     * Registers a rule class by its type key, e.g. `this.registerRule("MyApp.")` for `MyApp.EvenValidationRule`.
     * @param typeName - The full name of the type, or its namespace ending with a dot.
     */
    protected static registerRule<TypeName>(typeName: StringLiteral<TypeName>): ClassTypeInfo<TypeName> {
        if (Object.prototype.hasOwnProperty.call(this, Symbol.typeInfo) && (this as any)[Symbol.typeInfo])
            throw new Error(`Type ${this.name} already has a typeInfo property!`);

        const typeInfo = (this as any)[Symbol.typeInfo] = classTypeInfo(typeName);
        registerType(this);
        return typeInfo;
    }

    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }
}
