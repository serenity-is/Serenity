/**
 * Metadata that describes a row type for grid and dialog integration.
 * Implementations are resolved from the row type registry and used for
 * permissions, identity and display name resolution.
 */
export interface IRowDefinition {
    /** Permission required to delete rows. */
    readonly deletePermission?: string;
    /** Name of the identity / primary key property. */
    readonly idProperty?: string;
    /** Permission required to insert rows. */
    readonly insertPermission?: string;
    /** Name of the boolean property that marks a row as active. */
    readonly isActiveProperty?: string;
    /** Name of the boolean property that marks a row as soft-deleted. */
    readonly isDeletedProperty?: string;
    /** Local text prefix for entity texts (display names, dialogs). */
    readonly localTextPrefix?: string;
    /** Name of the property used as the display / name field. */
    readonly nameProperty?: string;
    /** Permission required to read rows. */
    readonly readPermission?: string;
    /** Permission required to update rows. */
    readonly updatePermission?: string;
}