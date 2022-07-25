/***
 * A base class that all special / non-data rows (like Group and GroupTotals) derive from.
 */
export class NonDataRow {
    __nonDataRow: boolean = true;
}

export const preClickClassName = "slick-edit-preclick";

//@ts-ignore
typeof window !== "undefined" && window.Slick && (window.Slick.Map = Map);