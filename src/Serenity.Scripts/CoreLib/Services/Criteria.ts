export {}

export function Criteria(field: string): any[] {
    return [field];
}

export namespace Criteria {
    import C = Criteria;

    export function isEmpty(c: any[]): boolean {
        return c == null ||
            c.length === 0 ||
            (c.length === 1 &&
                typeof c[0] == "string" &&
                c[0].length === 0);
    }

    export function join(c1: any[], op: string, c2: any[]): any[] {
        if (C.isEmpty(c1))
            return c2;

        if (C.isEmpty(c2))
            return c1;

        return [c1, op, c2];
    }

    export function paren(c: any[]): any[] {
        return C.isEmpty(c) ? c : ['()', c];
    }

    export function and(c1: any[], c2: any[], ...rest: any[][]): any[] {
        var result = join(c1, 'and', c2);
        if (rest) {
            for (let k of rest)
                result = join(result, 'and', k);
        }

        return result;
    }

    export function or(c1: any[], c2: any[], ...rest: any[][]): any[] {
        var result = join(c1, 'or', c2);

        if (rest) {
            for (let k of rest)
                result = join(result, 'or', k);
        }

        return result;
    }

    export const enum Operator {
        paren = "()",
        not = "not",
        isNull = "is null",
        isNotNull = "is not null",
        exists = "exists",
        and = "and",
        or = "or",
        xor = "xor",
        eq = "=",
        ne = "!=",
        gt = ">",
        ge = ">=",
        lt = "<",
        le = "<=",
        in = "in",
        notIn = "not in",
        like = "like",
        notLike = "not like"
    }
}