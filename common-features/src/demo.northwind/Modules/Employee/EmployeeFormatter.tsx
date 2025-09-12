import { Formatter, IInitializeColumn, faIcon, formatterTypeInfo, isTrimmedEmpty, registerType } from "@serenity-is/corelib";
import { Column, FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { Gender } from "../ServerTypes/Demo";

export class EmployeeFormatter implements Formatter {
    static typeInfo = formatterTypeInfo("Serenity.Demo.Northwind.EmployeeFormatter", [IInitializeColumn]); static { registerType(this); }

    constructor(public readonly props: { genderProperty?: string } = {}) {
        this.props ??= {};
    }

    format(ctx: FormatterContext): FormatterResult {
        if (!this.props.genderProperty || isTrimmedEmpty(ctx.value))
            return <>{ctx.value}</>

        let female = ctx.item[this.props.genderProperty] === Gender.Female;
        return <><i class={[faIcon(female ? "female" : "male", female ? "danger" : "primary"), "text-opacity-75"]}></i> {ctx.value}</>;
    }

    public initializeColumn(column: Column) {
        column.referencedFields = column.referencedFields || [];
        if (this.props.genderProperty)
            column.referencedFields.push(this.props.genderProperty);
    }
}
