import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { DialogTexts, formatterTypeInfo, iconClassName, IconClassName, localText, nsSerenity, registerType } from "../../base";
import { Formatter } from "../../slick";

/**
 * Renders a boolean value as localized text and/or an icon.
 * Falls back to `DialogTexts.YesButton` / `NoButton` when no explicit texts are provided.
 */
export class BooleanFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * Creates a new BooleanFormatter.
     * @param props - Formatter options.
     * @param props.falseText - Text for `false` values.
     * @param props.falseIcon - Icon class for `false` values.
     * @param props.nullText - Text for `null` values.
     * @param props.nullIcon - Icon class for `null` values.
     * @param props.trueText - Text for `true` values.
     * @param props.trueIcon - Icon class for `true` values.
     * @param props.showText - Whether to show text alongside icon (default `true`).
     * @param props.showHint - Whether to show text as `title` hint.
     */
    constructor(public readonly props: {
        falseText?: string,
        falseIcon?: IconClassName,
        nullText?: string,
        nullIcon?: IconClassName,
        trueText?: string,
        trueIcon?: IconClassName,
        showText?: boolean,
        showHint?: boolean
    } = {}) {
        this.props ??= {};
    }

    /**
     * Formats the boolean value for display.
     * @param ctx - Formatter context with value/item/column.
     * @returns Text, icon, or combined span per `props`.
     */
    format(ctx: FormatterContext): FormatterResult {

        const text = ctx.value == null ?
            localText(this.props.nullText, this.props.nullText ?? '') :
            (ctx.value ? localText(this.props.trueText, this.props.trueText ?? DialogTexts.YesButton) :
                localText(this.props.falseText, this.props.falseText ?? DialogTexts.NoButton));
        const showText = this.props?.showText ?? true;

        const hint = (this.props?.showHint ?? !showText) ? text : void 0;
        let iconClass = ctx.value == null ? this.props.nullIcon : (ctx.value ? this.props.trueIcon : this.props.falseIcon);
        if (iconClass)
            iconClass = iconClassName(iconClass);

        const icon = iconClass == "" ? "" : iconClass ? <i class={iconClass} title={hint} /> : null;

        if (!showText)
            return icon ?? "";

        if (icon == null && hint == null)
            return text;

        if (!icon)
            return <span title={hint}>{text}</span>;

        return <span title={hint}>{icon} {text}</span>;
    }

    /** Gets the text for `false` values. @returns The false text. */
    public get falseText() { return this.props.falseText; }
    /**
     * Sets the text for `false` values.
     * @param value - The false text.
     */
    public set falseText(value) { this.props.falseText = value; }

    /** Gets the text for `true` values. @returns The true text. */
    public get trueText() { return this.props.trueText; }
    /**
     * Sets the text for `true` values.
     * @param value - The true text.
     */
    public set trueText(value) { this.props.trueText = value; }
}

