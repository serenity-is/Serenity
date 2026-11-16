import { nsSerenity, Validator, ValidationRule, type ValidationRuleContext } from "../../base";

// The rules of the Validate attributes of the server, e.g. Range of ValidateRangeAttribute, which the server sends by their
// short keys, like the ones of the editors, e.g. "Range" for Serenity.RangeValidationRule.

function isEmpty(value: any): boolean {
    return value == null || value === "" || (Array.isArray(value) && value.length === 0);
}

/** Compares two values as numbers when both are, else as texts, e.g. dates in the ISO format. */
function compareValues(a: any, b: any): number {
    const x = typeof a === "number" ? a : (typeof a === "string" && a.trim() !== "" && !isNaN(+a) ? +a : NaN);
    const y = typeof b === "number" ? b : (typeof b === "string" && b.trim() !== "" && !isNaN(+b) ? +b : NaN);
    if (!isNaN(x) && !isNaN(y))
        return x - y;

    const s = String(a), t = String(b);
    return s < t ? -1 : s > t ? 1 : 0;
}

/** The element the methods of the validator are given, which they look at to tell whether the field is optional. */
function elementOf(context: ValidationRuleContext): any {
    return context.element ?? document.createElement("input");
}

/** Requires a value between a minimum and a maximum, both included: numbers, or texts like dates in the ISO format. */
export class RangeValidationRule extends ValidationRule {
    static override [Symbol.typeInfo] = this.registerRule(nsSerenity);
    override textKey = "Validation.Range";
    override defaultText = "Please enter a value between {0} and {1}.";

    check(value: any, params: any[]): boolean {
        return compareValues(value, params?.[0]) >= 0 && compareValues(value, params?.[1]) <= 0;
    }
}

/** Requires the length of a text, or of a list, to be between a minimum and a maximum, the parameters being `[min, max]`. */
export class LengthValidationRule extends ValidationRule {
    static override [Symbol.typeInfo] = this.registerRule(nsSerenity);
    override textKey = "Validation.MaxLength";
    override defaultText = "Please enter no more than {0} characters.";

    check(value: any, params: [number, number | null]): boolean {
        const length = Array.isArray(value) ? value.length : String(value).length;
        return length >= (params?.[0] ?? 0) && (params?.[1] == null || length <= params[1]);
    }

    override getTextKey(params: [number, number | null]): string {
        return params?.[0] > 0 ? (params[1] == null ? "Validation.MinLength" : "Validation.RangeLength") : "Validation.MaxLength";
    }

    override getDefaultText(params: [number, number | null]): string {
        return params?.[0] > 0 ? (params[1] == null ? "Please enter no less than {0} characters." :
            "Please enter a value between {0} and {1} characters long.") : "Please enter no more than {0} characters.";
    }

    override getMessageArgs(params: [number, number | null]): any[] {
        return params?.[0] > 0 ? (params[1] == null ? [params[0]] : [params[0], params[1]]) : [params?.[1]];
    }
}

const patterns = new Map<string, RegExp>();

/** Requires a text to match a regular expression as a whole, as the server checks it. */
export class PatternValidationRule extends ValidationRule {
    static override [Symbol.typeInfo] = this.registerRule(nsSerenity);
    override textKey = "Validation.Pattern";
    override defaultText = "Please enter a value in the expected format.";

    check(value: any, params: string): boolean {
        if (!params)
            return true;

        let regex = patterns.get(params);
        if (!regex)
            patterns.set(params, regex = new RegExp("^(?:" + params + ")$"));
        return regex.test(String(value));
    }

    override getMessageArgs(): any[] {
        return [];
    }
}

/** Requires an email address, as the email class of the validator does. */
export class EmailValidationRule extends ValidationRule {
    static override [Symbol.typeInfo] = this.registerRule(nsSerenity);
    override textKey = "Validation.Email";
    override defaultText = "Please enter a valid email address.";

    check(value: any, _: any, context: ValidationRuleContext): boolean {
        return Validator.methods.email(String(value), elementOf(context)) !== false;
    }
}

/** Requires an address, as the url class of the validator does. */
export class UrlValidationRule extends ValidationRule {
    static override [Symbol.typeInfo] = this.registerRule(nsSerenity);
    override textKey = "Validation.Url";
    override defaultText = "Please enter a valid URL (e.g. http://www.site.com).";

    check(value: any, _: any, context: ValidationRuleContext): boolean {
        return Validator.methods.url(String(value), elementOf(context)) !== false;
    }
}

/** Requires the same value as another field, the name of which is the parameter, an empty one being the same as another. */
export class CompareValidationRule extends ValidationRule {
    static override [Symbol.typeInfo] = this.registerRule(nsSerenity);
    override textKey = "Validation.Compare";
    override defaultText = "Please enter the same value again.";

    check(value: any, params: string, context: ValidationRuleContext): boolean {
        if (!params || !context.getValue)
            return true;

        const other = context.getValue(params);
        if (isEmpty(value) && isEmpty(other))
            return true;

        return value != null && other != null && String(value) === String(other);
    }

    override getMessageArgs(): any[] {
        return [];
    }
}
