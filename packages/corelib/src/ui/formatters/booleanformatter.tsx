import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { DialogTexts, formatterTypeInfo, iconClassName, IconClassName, localText, nsSerenity, registerType } from "../../base";
import { Formatter } from "../../slick";

export class BooleanFormatter implements Formatter {
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

    public get falseText() { return this.props.falseText; }
    public set falseText(value) { this.props.falseText = value; }

    public get trueText() { return this.props.trueText; }
    public set trueText(value) { this.props.trueText = value; }
}

