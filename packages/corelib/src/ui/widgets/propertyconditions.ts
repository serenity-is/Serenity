import type { PropertyCondition } from "../../base";

/**
 * Gets whether a value is one, as a condition without values checks it: it is not null, an empty string, an empty list
 * or false, so a checkbox has a value while it is checked.
 * @param value - The value of a field.
 */
export function hasPropertyValue(value: any): boolean {
    if (value == null || value === "" || value === false)
        return false;

    return !Array.isArray(value) || value.length > 0;
}

/**
 * Gets the text a value is compared by in a condition, e.g. `"2"` for an enum value of 2, and `"true"` for a checked
 * checkbox, which the server sends the values of the condition as too.
 * @param value - The value.
 */
function conditionText(value: any): string | null {
    if (value == null || value === "")
        return null;

    return String(value);
}

/**
 * Gets whether the value of a field meets a condition: it has one of the values of the condition, compared as text,
 * or a list value has one of them as an item, or, without values, it has a value at all, see {@link hasPropertyValue}.
 * The result is the opposite for a condition with `isNot`.
 * @param condition - The condition.
 * @param value - The value of the field the condition is on.
 */
export function isPropertyConditionMet(condition: PropertyCondition, value: any): boolean {
    let met: boolean;
    if (condition.values?.length) {
        const texts = condition.values.map(conditionText);
        const matches = (x: any) => texts.includes(conditionText(x));
        met = Array.isArray(value) ? value.some(matches) : matches(value);
    }
    else
        met = hasPropertyValue(value);

    return met !== !!condition.isNot;
}
