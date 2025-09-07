import { Column } from "@serenity-is/sleekgrid";
import { localText } from "../../base";

export namespace SlickHelper {
    export function setDefaults(columns: Column[], localTextPrefix?: string): any {
        for (var col of columns) {
            col.sortable = (col.sortable != null ? col.sortable : true);
            var id = col.id;
            if (id == null) {
                id = col.field;
            }
            col.id = id;

            if (localTextPrefix != null && col.id != null &&
                (col.name == null || col.name.startsWith('~'))) {
                var key = (col.name != null ? col.name.substring(1) : col.id);
                col.name = localText(localTextPrefix + key);
            }
        }

        return columns;
    }
}
