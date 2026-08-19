import { Column } from "@serenity-is/sleekgrid";
import { localText } from "../../base";

/**
 * Helper functions for sleek grids.
 */
export namespace SlickHelper {
    /**
     * Applies default values to column definitions, such as sortability, id,
     * and localized names.
     * @param columns - The column definitions to update.
     * @param localTextPrefix - Optional local text prefix used to localize column names.
     * @returns The updated column definitions.
     */
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
