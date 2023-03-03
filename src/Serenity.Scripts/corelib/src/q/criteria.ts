export function Criteria(field: string): Criteria.Builder {
    return Criteria.Builder.of(field) as Criteria.Builder;
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

    export function not(c: any[]) {
        return [Operator.not, c]
    }

    export enum Operator {
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

    export class Builder extends Array {
        bw(fromInclusive: any, toInclusive: any): Array<any> {
            return Criteria.and([this, '>=', fromInclusive], [this, '<=', toInclusive]);
        }
    
        endsWith(value: string): Array<any> { 
            return [this, Operator.like, '%' + value];
        }

        eq(value: any): Array<any> { 
            return [this, Operator.eq, value];
        }
    
        gt(value: any): Array<any> { 
            return [this, Operator.gt, value];
        }
    
        ge(value: any): Array<any> { 
            return [this, Operator.ge, value];
        }
    
        in(values: any[]): Array<any> { 
            return [this, Operator.in, [values]];
        }
    
        isNull(): Array<any> { 
            return [Operator.isNull, this];
        }
        
        isNotNull(): Array<any> { 
            return [Operator.isNotNull, this];
        }
    
        le(value: any): Array<any> { 
            return [this, Operator.le, value];
        }
    
        lt(value: any): Array<any> { 
            return [this, Operator.lt, value];
        }
    
        ne(value: any): Array<any> { 
            return [this, Operator.ne, value];
        }
    
        like(value: any): Array<any> { 
            return [this, Operator.like, value];
        }

        startsWith(value: string): Array<any> { 
            return [this, Operator.like, value + '%'];
        }

        notIn(values: any[]): Array<any> { 
            return [this, Operator.notIn, [values]];
        }
    
        notLike(value: any): Array<any> { 
            return [this, Operator.notLike, value];
        }
    }
}
