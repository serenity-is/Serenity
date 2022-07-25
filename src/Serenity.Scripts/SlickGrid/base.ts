/***
 * A base class that all special / non-data rows (like Group and GroupTotals) derive from.
 */
export class NonDataRow {
    __nonDataRow: boolean = true;
}

const map = Map;
export { map as Map }

export const preClickClassName = "slick-edit-preclick";
