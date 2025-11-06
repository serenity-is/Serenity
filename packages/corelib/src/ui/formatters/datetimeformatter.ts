import { Culture, formatterTypeInfo, nsSerenity, registerType } from "../../base";
import { DateFormatter } from "./dateformatter";

export class DateTimeFormatter extends DateFormatter {
    static override[Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }
    constructor(props: { displayFormat?: string } = {}) {
        super({ displayFormat: Culture.dateTimeFormat, ...props });
    }
}
