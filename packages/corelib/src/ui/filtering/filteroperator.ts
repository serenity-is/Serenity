export {}

export interface FilterOperator {
    key?: string;
    title?: string;
    format?: string;
}

export namespace FilterOperators {

    export const isTrue = 'true';
    export const isFalse = 'false';
    export const contains = 'contains';
    export const startsWith = 'startswith';
    export const EQ = 'eq';
    export const NE = 'ne';
    export const GT = 'gt';
    export const GE = 'ge';
    export const LT = 'lt';
    export const LE = 'le';
    export const BW = 'bw';
    export const IN = 'in';
    export const isNull = 'isnull';
    export const isNotNull = 'isnotnull';

    export const toCriteriaOperator: { [key: string]: string } = {
        eq: '=',
        ne: '!=',
        gt: '>',
        ge: '>=',
        lt: '<',
        le: '<='
    };
}