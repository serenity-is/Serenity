import type { Column } from "@serenity-is/sleekgrid";

/**
 * Helpers for the utility columns of a grid, which are the columns the framework adds for its own
 * purposes, like the checkbox column of {@link GridRowSelectionMixin}, rather than showing data.
 * The id of a utility column starts and ends with two underscores, like `__select__`.
 *
 * The functions are meant to be overridable by assigning to them, e.g. `UtilityColumns.isExportable = ...`.
 * They call each other through the namespace, so an override of one is also used by the others.
 */
export namespace UtilityColumns {
    /** The prefix and the suffix of the id of a utility column. */
    export const idAffix = "__";
    /** The id of the column that shows the checkboxes of the rows. */
    export const selectId = "__select__";
    /** The id of the column that shows the radio buttons of the rows. */
    export const radioId = "__radio__";
    /** The id of the column that shows the row actions. */
    export const actionsId = "__actions__";

    const fallbackNames: Record<string, string> = {
        [selectId]: "[×]",
        [radioId]: "(o)",
        [actionsId]: "[…]"
    };

    /**
     * Checks if a column is a utility column, which means its id starts and ends with `__`,
     * like `__select__`, `__radio__` or `__actions__`.
     * @param column The column to check.
     * @returns True if the column is a utility column, otherwise false.
     */
    export function isUtility(column: Column): boolean {
        const id = column?.id;
        return typeof id === "string" && id.length > 2 * idAffix.length &&
            id.startsWith(idAffix) && id.endsWith(idAffix);
    }

    /**
     * Checks if a column can be included in an export, which only data columns can.
     * @param column The column to check.
     * @returns True if the column is not a utility column, otherwise false.
     */
    export function isExportable(column: Column): boolean {
        return !UtilityColumns.isUtility(column);
    }

    /**
     * Gets the text that identifies a column to the user in places like the column picker.
     * For a utility column, the name is often blank, so this falls back to its tooltip,
     * then to a symbol for the known ones, and then to its id without the underscores.
     * For other columns it is the name, the tooltip or the id.
     * @param column The column to get the display name of.
     * @returns The display name, or an empty string if the column is null.
     */
    export function getDisplayName(column: Column): string {
        if (!column)
            return "";

        const name = column.name?.trim();
        if (name)
            return name;

        const toolTip = column.toolTip?.trim();
        if (toolTip)
            return toolTip;

        if (!UtilityColumns.isUtility(column))
            return column.id ?? "";

        return fallbackNames[column.id] ??
            column.id.substring(idAffix.length, column.id.length - idAffix.length);
    }
}
