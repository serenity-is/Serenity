import { deepClone } from "./system";

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


    interface ICriteriaBuilderGate extends Omit<ICriteriaBuilder, "and" | "or" | "xor" | "build"> { }
    interface ICriteriaBuilderParen extends Omit<ICriteriaBuilder, "paren" | "not" | "build"> {
        not: Omit<ICriteriaBuilderNot, "paren">;
    }
    interface ICriteriaBuilderNot extends Omit<ICriteriaBuilder, "not" | "paren" | "build"> {
        paren: Omit<ICriteriaBuilderParen, "not">;
    }

    interface ICriteriaBuilderCondition {
        // Joins the next condition with an AND
        and: ICriteriaBuilderGate;
        // Joins the next condition with an OR
        or: ICriteriaBuilderGate;
        // Joins the next condition with an XOR
        xor: ICriteriaBuilderGate;
        // Builds the criteria
        build(): any[];
    }

    interface ICriteriaBuilder {
        // Inverts the next condition
        not: ICriteriaBuilderNot;
        // Parenthesizes the next condition
        paren: ICriteriaBuilderParen;
        // Creates an equals condition with the given field and value (value can be a field name but it must be used with the Criteria function)
        equals(fieldName: string, value: any): ICriteriaBuilderCondition;
        // Creates a not greater than condition with the given field and value (value can be a field name but it must be used with the Criteria function)
        greaterThan(fieldName: string, value: any): ICriteriaBuilderCondition;
        // Creates a not greater than or equal condition with the given field and value (value can be a field name but it must be used with the Criteria function)
        greaterThanOrEqual(fieldName: string, value: any): ICriteriaBuilderCondition;
        // Creates a not less than condition with the given field and value (value can be a field name but it must be used with the Criteria function)
        lessThan(fieldName: string, value: any): ICriteriaBuilderCondition;
        // Creates a not less than or equal condition with the given field and value (value can be a field name but it must be used with the Criteria function)
        lessThanOrEqual(fieldName: string, value: any): ICriteriaBuilderCondition;
        // Creates a between condition with the given field and value (values can be a field names but they must be used with the Criteria function)
        between(fieldName: string, fromInclusive: any, toInclusive: any): ICriteriaBuilderCondition;
        // Creates an in condition with the given field and value (values can be a field name but they must be used with the Criteria function)
        in(fieldName: string, values: any[]): ICriteriaBuilderCondition;
        // Creates a like condition with the given field and value (value can be a field name but it must be used with the Criteria function)
        like(fieldName: string, value: any): ICriteriaBuilderCondition;
        // Creates an is null condition with the given field
        isNull(fieldName: string): ICriteriaBuilderCondition;
        // Creates an exists condition with the given sub-query
        exists(subQuery: any[]): ICriteriaBuilderCondition;
        // Joins criteria with current flags
        join(criteria: any[] | ICriteriaBuilderCondition): ICriteriaBuilderCondition;
    }

    export function Builder(): ICriteriaBuilder;
    export function Builder(criteria: any[]): ICriteriaBuilderCondition;
    export function Builder(criteria?: any[]): ICriteriaBuilder | ICriteriaBuilderCondition {
        return new CriteriaBuilderImpl(criteria);
    }

    const NegatedOperatorMap: { [key: string]: Operator } = {
        [Operator.eq]: Operator.ne,
        [Operator.in]: Operator.notIn,
        [Operator.like]: Operator.notLike,
        [Operator.isNull]: Operator.isNotNull,
        [Operator.gt]: Operator.le,
        [Operator.ge]: Operator.lt,
        [Operator.lt]: Operator.ge,
        [Operator.le]: Operator.gt
    };

    class CriteriaBuilderImpl implements ICriteriaBuilder {
        private condition: any[];
        private notFlag: boolean = false
        private andFlag: boolean = false;
        private orFlag: boolean = false;
        private parenFlag: boolean = false;
        private xorFlag: boolean = false;

        constructor(condition?: any[]) {
            this.condition = condition ?? [];
        }

        isEmpty(c?: any[]): boolean {
            c ??= this.condition;
            return c == null ||
                c.length === 0 ||
                (c.length === 1 &&
                    typeof c[0] == "string" &&
                    c[0].length === 0);
        }

        private andCondition(c1: any[], c2: any[]): any[] {
            return [c1, Operator.and, c2];
        }

        private orCondition(c1: any[], c2: any[]): any[] {
            return [c1, Operator.or, c2];
        }

        private xorCondition(c1: any[], c2: any[]): any[] {
            return [c1, Operator.xor, c2];
        }

        get not(): ICriteriaBuilderGate {
            this.notFlag = true;
            return this;
        }

        get and(): ICriteriaBuilderGate {
            this.andFlag = true;
            return this;
        }

        get or(): ICriteriaBuilderGate {
            this.orFlag = true;
            return this;
        }

        get xor(): ICriteriaBuilderGate {
            this.xorFlag = true;
            return this;
        }

        get paren(): ICriteriaBuilderParen {
            this.parenFlag = true;
            return this;
        }

        private parenthesize(condition: any[]): any[] {
            return this.isEmpty(condition) ? condition : [Operator.paren, condition];
        }

        private appendCondition(condition: any[]): ICriteriaBuilder {
            if (this.notFlag) {
                const operatorIndex = condition.length === 3 ? 1 : 0;
                const operator: Operator = NegatedOperatorMap[condition[operatorIndex]];

                if (operator)
                    condition[operatorIndex] = operator;
                else
                    condition = [Operator.not, condition];

                this.notFlag = false;
            }

            if (this.parenFlag) {
                condition = this.parenthesize(condition)
                this.parenFlag = false;
            }

            if (this.andFlag) {
                condition = this.andCondition(this.condition, condition);
                this.andFlag = false;
            } else if (this.orFlag) {
                condition = this.orCondition(this.condition, condition);
                this.orFlag = false;
            } else if (this.xorFlag) {
                condition = this.xorCondition(this.condition, condition);
                this.xorFlag = false;
            }

            this.condition = condition;
            return this;
        }

        equals(fieldName: string, value: any): ICriteriaBuilderCondition {
            this.appendCondition([[fieldName], Operator.eq, value]);
            return this;
        }

        isNull(fieldName: string): ICriteriaBuilderCondition {
            this.appendCondition([Operator.isNull, [fieldName]]);
            return this;
        }

        exists(subQuery: any[]): ICriteriaBuilderCondition {
            this.appendCondition([Operator.exists, subQuery]);
            return this;
        }

        greaterThan(fieldName: string, value: any): ICriteriaBuilderCondition {
            this.appendCondition([[fieldName], Operator.gt, value]);
            return this;
        }

        greaterThanOrEqual(fieldName: string, value: any): ICriteriaBuilderCondition {
            this.appendCondition([[fieldName], Operator.ge, value]);
            return this;
        }

        lessThan(fieldName: string, value: any): ICriteriaBuilderCondition {
            this.appendCondition([[fieldName], Operator.lt, value]);
            return this;
        }

        lessThanOrEqual(fieldName: string, value: any): ICriteriaBuilderCondition {
            this.appendCondition([[fieldName], Operator.le, value]);
            return this;
        }

        between(fieldName: string, fromInclusive: any, toInclusive: any): ICriteriaBuilderCondition {
            if (fromInclusive == null || toInclusive == null)
                throw new Error("fromInclusive and toInclusive must not be null");

            if (fromInclusive > toInclusive)
                throw new Error("fromInclusive must be less than or equal to toInclusive");

            const parenFlag = this.parenFlag;
            this.parenFlag = false;
            let condition;

            if (this.notFlag) {
                this.notFlag = false;
                condition = this.orCondition(this.lessThan(fieldName, fromInclusive).build(), this.greaterThan(fieldName, toInclusive).build());
            }
            else
                condition = this.andCondition(this.greaterThanOrEqual(fieldName, fromInclusive).build(), this.lessThanOrEqual(fieldName, toInclusive).build());

            this.parenFlag = parenFlag;
            this.appendCondition(condition);

            return this;
        }

        in(fieldName: string, values: any[]): ICriteriaBuilderCondition {
            this.appendCondition([[fieldName], Operator.in, values]);
            return this;
        }

        like(fieldName: string, value: any): ICriteriaBuilderCondition {
            this.appendCondition([[fieldName], Operator.like, value]);
            return this;
        }

        join(criteria: any[] | ICriteriaBuilderCondition): ICriteriaBuilderCondition {
            let crit: any[] = criteria as any[]

            if (criteria instanceof CriteriaBuilderImpl)
                crit = criteria.build();

            this.appendCondition(crit);
            return this;
        }

        build(): any[] {
            return deepClone(this.condition);
        }
    }
}