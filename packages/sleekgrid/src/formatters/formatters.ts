import { FormatterContext, FormatterResult } from "../core";

export function PercentCompleteFormatter(ctx: FormatterContext) {
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

export function YesNoFormatter(ctx: FormatterContext): FormatterResult {
    return ctx.value ? 'Yes' : 'No';
}


export function CheckboxFormatter(ctx: FormatterContext): FormatterResult {
    const i = document.createElement('i');
    i.className = 'slick-checkbox slick-edit-preclick' + (ctx.value ? ' checked' : '');
    return i;
}

export function CheckmarkFormatter(ctx: FormatterContext): FormatterResult {
    if (!ctx.value)
        return '';

    const i = document.createElement('i');
    i.className = 'slick-checkmark';
    return i;
}

