import { FormatterContext } from "@serenity-is/sleekgrid";
import { DialogTexts, formatterTypeInfo, localText, nsSerenity, registerType } from "../../base";
import { Formatter } from "../../slick";

export class BooleanFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public readonly props: { falseText?: string, trueText?: string } = {}) {
        this.props ??= {};
    }

    format(ctx: FormatterContext) {

        if (ctx.value == null)
            return '';

        if (!!ctx.value)
            return ctx.escape(localText(this.trueText, this.trueText ?? DialogTexts.YesButton));

        return ctx.escape(localText(this.falseText, this.falseText ?? DialogTexts.NoButton));
    }

    public get falseText() { return this.props.falseText; }
    public set falseText(value) { this.props.falseText = value; }

    public get trueText() { return this.props.trueText; }
    public set trueText(value) { this.props.trueText = value; }
}

