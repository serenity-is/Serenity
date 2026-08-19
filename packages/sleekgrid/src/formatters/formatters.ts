import { FormatterContext, FormatterResult } from "../core";

/**
 * Renders a numeric percent value as bold colored text (red < 50%, green otherwise).
 * Returns `"-"` when the value is empty/null.
 * @param ctx - Formatter context whose `value` is the numeric percentage (0–100).
 * @returns A `<span>` element with colored text, or `"-"` for empty values.
 */
export function PercentCompleteFormatter(ctx: FormatterContext): FormatterResult {
    if (ctx.value == null || ctx.value === "")
        return "-";
    const span = document.createElement('span');
    span.textContent = ctx.value + "%";
    span.style.fontWeight = 'bold';
    if (ctx.value < 50)
        span.style.color = 'red';
    else
        span.style.color = 'green';
    return span;
}

/**
 * Renders a numeric percent value as a horizontal bar whose color varies by
 * threshold (red < 30, silver < 70, green otherwise).
 * @param ctx - Formatter context whose `value` is the numeric percentage (0–100).
 * @returns A `<span>` bar element, or empty string for empty values.
 */
export function PercentCompleteBarFormatter(ctx: FormatterContext): FormatterResult {
    if (ctx.value == null || ctx.value === "")
        return "";

    var color;
    if (ctx.value < 30)
        color = "red";
    else if (ctx.value < 70)
        color = "silver";
    else
        color = "green";

    const span = document.createElement('span');
    span.className = 'percent-complete-bar slick-percentcomplete-bar';
    span.style.background = color;
    span.style.width = ctx.value + '%';
    span.title = ctx.value + '%';
    return span;
}

/**
 * Renders a boolean value as `"Yes"` or `"No"`.
 * @param ctx - Formatter context whose `value` is coerced to boolean.
 * @returns `"Yes"` when truthy, `"No"` otherwise.
 */
export function YesNoFormatter(ctx: FormatterContext): FormatterResult {
    return ctx.value ? 'Yes' : 'No';
}


/**
 * Renders a boolean value as a styled checkbox icon (`<i>` with
 * `slick-checkbox` / `checked` classes).
 * @param ctx - Formatter context whose `value` is coerced to boolean.
 * @returns An `<i>` element representing the checkbox state.
 */
export function CheckBoxFormatter(ctx: FormatterContext): FormatterResult {
    const i = document.createElement('i');
    i.className = 'slick-checkbox slick-edit-preclick' + (ctx.value ? ' checked' : '');
    return i;
}

/**
 * Renders a boolean value as a checkmark icon; nothing when falsy.
 * @param ctx - Formatter context whose `value` is coerced to boolean.
 * @returns An `<i>` with `slick-checkmark` when truthy, otherwise empty string.
 */
export function CheckmarkFormatter(ctx: FormatterContext): FormatterResult {
    if (!ctx.value)
        return '';

    const i = document.createElement('i');
    i.className = 'slick-checkmark';
    return i;
}

