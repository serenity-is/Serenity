import { isPromiseLike, localText, stringFormat, tryGetText, ValidationRule, type ClientValidationRule, type ValidationRuleContext } from "../../base";
import { ValidationRuleTypeRegistry } from "../../types/validationruletyperegistry";
// the rules of the Validate attributes of the server, which are always available
import "./builtinvalidationrules";

/**
 * Checks a value by a validation rule registered with {@link registerValidationRule}. It is not called for an empty value,
 * i.e. null, an empty string or an empty list, as being required is a rule of its own.
 * @param value - The value, e.g. a number for a decimal editor, or a text.
 * @param params - The parameters of the rule, as the server sends them.
 * @param context - The element and the other fields.
 * @returns Whether the value is valid.
 */
export type ValidationRuleCheck = (value: any, params: any, context: ValidationRuleContext) => boolean;

/** The options of a validation rule registered with {@link registerValidationRule}. */
export interface ValidationRuleOptions {
    /**
     * The local text key of the message of the rule, e.g. `Validation.Even`, which can have the parameters of the rule as
     * `{0}`, `{1}`, used when the attribute gives no message of its own. It is `Validation.Invalid` when it is not set.
     */
    textKey?: string;
    /** The text of the message for when its key has no text, e.g. the English one. */
    defaultText?: string;
}

/** A rule registered by a function, as an instance of the rule class it would be. */
class FunctionValidationRule extends ValidationRule {
    constructor(private readonly checkFn: ValidationRuleCheck, options?: ValidationRuleOptions) {
        super();
        if (options?.textKey) {
            this.textKey = options.textKey;
            this.defaultText = options.defaultText;
        }
        else if (options?.defaultText)
            this.defaultText = options.defaultText;
    }

    check(value: any, params: any, context: ValidationRuleContext): boolean {
        return this.checkFn(value, params, context);
    }
}

const functionRules = new Map<string, ValidationRule>();
const classInstances = new Map<Function, ValidationRule>();
const warned = new Set<string>();

/**
 * Registers a validation rule by a function, for the fields whose properties have a validation attribute of the same rule
 * name on the server. A rule class deriving from {@link ValidationRule} is usually better, as it is found by its type key,
 * and can be loaded on demand by the forms that use it, while a rule registered by a function has to be registered by the
 * page before a form uses it, e.g. at the start of the application.
 * @param name - The name of the rule, as the attribute gives it.
 * @param check - Checks a value by the rule.
 * @param options - The message of the rule.
 */
export function registerValidationRule(name: string, check: ValidationRuleCheck, options?: ValidationRuleOptions): void {
    if (!name)
        throw new Error("The name of a validation rule is required.");

    functionRules.set(name, new FunctionValidationRule(check, options));
}

/**
 * Gets the rule of a name, registered by a function, or a rule class that is loaded, or null when it is neither.
 * @param name - The name of the rule, or the type key of its class.
 */
function getRule(name: string): ValidationRule | null {
    if (!name)
        return null;

    const registered = functionRules.get(name);
    if (registered)
        return registered;

    const type = ValidationRuleTypeRegistry.tryGet(name) as any;
    if (!type)
        return null;

    let instance = classInstances.get(type);
    if (!instance)
        classInstances.set(type, instance = new type());
    return instance;
}

/**
 * Gets whether a validation rule can be checked now, i.e. it is registered by a function, or its class is loaded.
 * @param name - The name of the rule, or the type key of its class.
 */
export function isValidationRuleRegistered(name: string): boolean {
    return !!getRule(name);
}

/**
 * Loads the classes of the validation rules that are not loaded yet, which implement LazyLoadInclude, so they can be checked
 * by the time the form is submitted. A form calls it once it is created.
 * @param validationRules - The rules, as the server sends them.
 * @returns A promise that resolves when they are loaded, or nothing when they are all loaded already.
 */
export function loadValidationRules(validationRules: ClientValidationRule[] | null | undefined): Promise<void> | void {
    const promises: PromiseLike<any>[] = [];
    for (const rule of validationRules ?? []) {
        if (!rule?.rule || getRule(rule.rule))
            continue;

        const loaded = ValidationRuleTypeRegistry.tryGetOrLoad(rule.rule);
        if (isPromiseLike(loaded))
            promises.push(loaded);
    }

    if (promises.length)
        return Promise.all(promises).then(() => { }, () => { });
}

function isEmpty(value: any): boolean {
    return value == null || value === "" || (Array.isArray(value) && value.length === 0);
}

function formatMessage(text: string, args: any[]): string {
    if (!text || !/\{\d+/.test(text))
        return text;

    return stringFormat(text, ...args);
}

/**
 * Gets the message of a validation rule whose check failed: the text of its key, else its message, else the default message
 * of the rule, with the parameters of the rule.
 * @param rule - The rule, as the server sends it.
 */
export function getValidationRuleMessage(rule: ClientValidationRule): string {
    const registered = getRule(rule.rule);
    const textKey = registered?.getTextKey(rule.params);
    const text = (rule.textKey ? tryGetText(rule.textKey) : undefined) ?? rule.message ??
        (registered ? localText(textKey, registered.getDefaultText(rule.params) ?? textKey) :
            localText("Validation.Invalid", "Please enter a valid value."));
    const args = registered ? registered.getMessageArgs(rule.params) :
        Array.isArray(rule.params) ? rule.params : rule.params == null ? [] : [rule.params];
    return formatMessage(text, args);
}

/**
 * Checks a value by validation rules, in their order, stopping at the first one it is not valid by. An empty value is
 * valid. A rule that is neither registered by a function nor a loaded class is skipped, as the server still checks it,
 * with a warning unless its class is still being loaded, see {@link loadValidationRules}.
 * @param validationRules - The rules, as the server sends them.
 * @param value - The value.
 * @param context - The element and the other fields.
 * @returns The message of the first rule the value is not valid by, or null.
 */
export function checkValidationRules(validationRules: ClientValidationRule[] | null | undefined, value: any,
    context?: ValidationRuleContext): string | null {

    if (!validationRules?.length || isEmpty(value))
        return null;

    for (const rule of validationRules) {
        const registered = getRule(rule?.rule);
        if (!registered) {
            if (rule?.rule && !warned.has(rule.rule) && !ValidationRuleTypeRegistry.isAvailable(rule.rule)) {
                warned.add(rule.rule);
                console.warn(`The validation rule "${rule.rule}" is not registered, so it is only checked on the server.`);
            }
            continue;
        }

        if (!registered.check(value, rule.params, context ?? {}))
            return getValidationRuleMessage(rule);
    }

    return null;
}
