import { Culture, formatterTypeInfo, registerType } from "../../base";
import { DateFormatter } from "./dateformatter";

export class DateTimeFormatter extends DateFormatter {
    static typeInfo = formatterTypeInfo("Serenity.DateTimeFormatter"); static { registerType(this); }
    constructor(props: { displayFormat?: string } = {}) {
        super({ displayFormat: Culture.dateTimeFormat, ...props });
    }
}
