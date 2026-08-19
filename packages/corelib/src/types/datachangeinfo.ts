export { };

/**
 * Event payload broadcast when an entity is inserted / updated / deleted via a dialog or grid.
 * Listen via bubbled `datachange` events or {@link SubDialogHelper}.
 */
export interface DataChangeInfo extends Event {
    /** Operation that triggered the event (e.g. insert / update / delete). */
    operationType: string;
    /** Primary key of the affected entity, if available. */
    entityId: any;
    /** Full entity payload, if available. */
    entity: any;
}
