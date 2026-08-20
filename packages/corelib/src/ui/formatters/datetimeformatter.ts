import { Culture, formatterTypeInfo, nsSerenity, registerType } from "../../base";
import { DateFormatter } from "./dateformatter";

/** Variant of {@link DateFormatter} that defaults to `Culture.dateTimeFormat`. */
export class DateTimeFormatter extends DateFormatter {
    static override[Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }
    /**
     * Creates a new DateTimeFormatter.
     * @param props - Formatter options.
     * @param props.displayFormat - Date-time format string (default `Culture.dateTimeFormat`).
     */
    constructor(props: { displayFormat?: string } = {}) {
        super({ displayFormat: Culture.dateTimeFormat, ...props });
    }
}
