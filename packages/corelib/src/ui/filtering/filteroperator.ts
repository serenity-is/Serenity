export { };

/**
 * Describes a filter operator (e.g. equals, contains, is null).
 */
export interface FilterOperator {
    /** Operator key. */
    key?: string;
    /** Display title. */
    title?: string;
    /** Format string used to build the display text. */
    format?: string;
}

/**
 * Constants for the built-in filter operators.
 */
export namespace FilterOperators {

    /** Is true operator. */
    export const isTrue = 'true';
    /** Is false operator. */
    export const isFalse = 'false';
    /** Contains operator. */
    export const contains = 'contains';
    /** Starts with operator. */
    export const startsWith = 'startswith';
    /** Equals operator. */
    export const EQ = 'eq';
    /** Not equals operator. */
    export const NE = 'ne';
    /** Greater than operator. */
    export const GT = 'gt';
    /** Greater than or equal operator. */
    export const GE = 'ge';
    /** Less than operator. */
    export const LT = 'lt';
    /** Less than or equal operator. */
    export const LE = 'le';
    /** Between operator. */
    export const BW = 'bw';
    /** In operator. */
    export const IN = 'in';
    /** Is null operator. */
    export const isNull = 'isnull';
    /** Is not null operator. */
    export const isNotNull = 'isnotnull';

    /** Maps operator keys to criteria comparison symbols. */
    export const toCriteriaOperator: { [key: string]: string } = {
        eq: '=',
        ne: '!=',
        gt: '>',
        ge: '>=',
        lt: '<',
        le: '<='
    };
}