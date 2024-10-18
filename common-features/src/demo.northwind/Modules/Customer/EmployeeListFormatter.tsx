import { Decorators, Formatter, Lookup, faIcon } from "@serenity-is/corelib";
import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { EmployeeRow } from "../ServerTypes/Demo";

let lookup: Lookup<EmployeeRow>;
let promise: Promise<Lookup<EmployeeRow>>;

@Decorators.registerFormatter('Serenity.Demo.Northwind.EmployeeListFormatter')
export class EmployeeListFormatter implements Formatter {

    format(ctx: FormatterContext): FormatterResult {

        let idList = ctx.value as string[];
        if (!idList || !idList.length)
            return "";

        let byId = lookup?.itemById;
        if (byId) {
            return <>{idList.map(x => {
                var z = byId[x];
                return z == null ? x : z.FullName;
            }).join(", ")}</>;
        }

        promise ??= EmployeeRow.getLookupAsync().then(l => {
            lookup = l;
            try {
                ctx.grid?.invalidate();
            }
            finally {
                lookup = null;
                promise = null;
            }
        }).catch(() => promise = null);

        return (<i class={faIcon("spinner")}></i>);
    }
}
