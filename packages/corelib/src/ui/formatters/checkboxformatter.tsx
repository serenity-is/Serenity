import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { DialogTexts, formatterTypeInfo, iconClassName, IconClassName, localText, nsSerenity, registerType } from "../../base";
import { Formatter } from "../../slick";

export class CheckboxFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

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

    format(ctx: FormatterContext): FormatterResult {
        const text = ctx.value == null ?
            localText(this.props.nullText, this.props.nullText ?? '') :
            (ctx.value ? localText(this.props.trueText, this.props.trueText ?? DialogTexts.YesButton) :
                localText(this.props.falseText, this.props.falseText ?? DialogTexts.NoButton));

        const showText = this.props?.showText ?? ctx.purpose === "headerfilter";

        const hint = (this.props?.showHint ?? (!showText && (this.props?.trueText || this.props?.falseText || this.props?.nullText))) ? text : void 0;
        let iconClass = ctx.value == null ? this.props.nullIcon : (ctx.value ? this.props.trueIcon : this.props.falseIcon);
        if (iconClass != null)
            iconClass = iconClassName(iconClass);

        const icon = iconClass == "" ? "" : iconClass ? <i class={[iconClass, "slick-edit-preclick"]} title={hint} /> :
            (ctx.value == null || ctx.purpose === "headerfilter" ? null :
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
