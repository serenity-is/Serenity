import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { DialogTexts, formatterTypeInfo, iconClassName, IconClassName, localText, nsSerenity, registerType } from "../../base";
import { Formatter } from "../../slick";

/**
 * Renders a boolean as a checkbox-like visual (with optional text). In grid display
 * the checkbox is read-only; in header-filter context it falls back to text/icon.
 */
export class CheckboxFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * @param props.falseText - Text for `false`.
     * @param props.falseIcon - Icon for `false`.
     * @param props.nullText - Text for `null`.
     * @param props.nullIcon - Icon for `null`.
     * @param props.trueText - Text for `true`.
     * @param props.trueIcon - Icon for `true`.
     * @param props.showText - Whether to show text (defaults to `true`, or `false` in grid cells).
     * @param props.showHint - Whether to surface text as `title` hint.
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

    /** @param ctx - Formatter context. @returns Checkbox / icon / text markup. */
    format(ctx: FormatterContext): FormatterResult {
        const text = ctx.value == null ?
            localText(this.props.nullText, this.props.nullText ?? '') :
            (ctx.value ? localText(this.props.trueText, this.props.trueText ?? DialogTexts.YesButton) :
                localText(this.props.falseText, this.props.falseText ?? DialogTexts.NoButton));

        const showText = this.props?.showText ?? ctx.purpose === "header-filter";

        const hint = (this.props?.showHint ?? (!showText && (this.props?.trueText || this.props?.falseText || this.props?.nullText))) ? text : void 0;
        let iconClass = ctx.value == null ? this.props.nullIcon : (ctx.value ? this.props.trueIcon : this.props.falseIcon);
        if (iconClass != null)
            iconClass = iconClassName(iconClass);

        const icon = iconClass == "" ? "" : iconClass ? <i class={[iconClass, "slick-edit-preclick"]} title={hint} /> :
            (ctx.value == null || ctx.purpose === "header-filter" ? null :
                <span class={["check-box no-float readonly slick-edit-preclick", ctx.value && 'checked']} title={hint} />);

        if (!showText)
            return icon ?? "";

        if (icon == null && hint == null)
            return text;

        if (!icon)
            return <span title={hint}>{text}</span>;

        return <span title={hint}>{icon} {text}</span>;

    }
}
