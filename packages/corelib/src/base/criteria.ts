/**
 * Fluent builder for Serenity criteria expressions with completion support.
 *
 * @remarks
 * Extends `Array` so an instance itself acts as a field-reference token (e.g. `["Amount"]`).
 * Create instances via {@link Criteria}`("FieldName")` rather than `new CriteriaBuilder()`.
 * Each method returns a Serenity criteria tuple/array that can be combined with
 * {@link Criteria.and}, {@link Criteria.or}, {@link Criteria.join}, or the
 * {@link parseCriteria} parser. `bw` stands for "between" (inclusive).
 * @example
 * Criteria("Age").ge(18); // [["Age"], ">=", 18]
 * @example
 * Criteria("Status").in([1, 2, 3]); // [["Status"], "in", [[1, 2, 3]]]
 */
export class CriteriaBuilder extends Array {
    /**
     * Creates a BETWEEN (inclusive) criteria: `field >= from AND field <= to`.
     *
     * @param fromInclusive - Lower bound (inclusive).
     * @param toInclusive - Upper bound (inclusive).
     * @returns Composite criteria `[[field, ">=", from], "and", [field, "<=", to]]`.
     * @example
     * Criteria("Amount").bw(10, 20);
     */
    bw(fromInclusive: any, toInclusive: any): Array<any> {
        return [[this, '>=', fromInclusive], 'and', [this, '<=', toInclusive]];
    }

    /**
     * Creates a `LIKE '%value%'` (contains) criteria.
     *
     * @param value - Substring to search for. Wrapped with `%` on both sides.
     * @returns Criteria `[field, "like", "%value%"]`.
     * @example
     * Criteria("Name").contains("ser"); // [["Name"], "like", "%ser%"]
     */
    contains(value: string): Array<any> {
        return [this, 'like', '%' + value + '%'];
    }

    /**
     * Creates a `LIKE '%value'` (ends-with) criteria.
     *
     * @param value - Suffix to match. Prefixed with `%`.
     * @returns Criteria `[field, "like", "%value"]`.
     * @example
     * Criteria("Email").endsWith("@example.com");
     */
    endsWith(value: string): Array<any> {
        return [this, 'like', '%' + value];
    }

    /**
     * Creates an equality (`=`) criteria.
     *
     * @param value - Value to compare for equality.
     * @returns Criteria `[field, "=", value]`.
     * @example
     * Criteria("IsActive").eq(true);
     */
    eq(value: any): Array<any> {
        return [this, '=', value];
    }

    /**
     * Creates a greater-than (`>`) criteria.
     *
     * @param value - Lower exclusive bound.
     * @returns Criteria `[field, ">", value]`.
     */
    gt(value: any): Array<any> {
        return [this, '>', value];
    }

    /**
     * Creates a greater-than-or-equal (`>=`) criteria.
     *
     * @param value - Lower inclusive bound.
     * @returns Criteria `[field, ">=", value]`.
     */
    ge(value: any): Array<any> {
        return [this, '>=', value];
    }

    /**
     * Creates an `IN` criteria.
     *
     * @param values - Array of allowed values. Wrapped as `[values]` per Serenity wire format.
     * @returns Criteria `[field, "in", [values]]`.
     * @example
     * Criteria("Status").in([1, 2]); // [["Status"], "in", [[1, 2]]]
     */
    in(values: any[]): Array<any> {
        return [this, 'in', [values]];
    }

    /**
     * Creates an `IS NULL` criteria.
     *
     * @returns Criteria `["is null", field]`.
     */
    isNull(): Array<any> {
        return ['is null', this];
    }

    /**
     * Creates an `IS NOT NULL` criteria.
     *
     * @returns Criteria `["is not null", field]`.
     */
    isNotNull(): Array<any> {
        return ['is not null', this];
    }

    /**
     * Creates a less-than-or-equal (`<=`) criteria.
     *
     * @param value - Upper inclusive bound.
     * @returns Criteria `[field, "<=", value]`.
     */
    le(value: any): Array<any> {
        return [this, '<=', value];
    }

    /**
     * Creates a less-than (`<`) criteria.
     *
     * @param value - Upper exclusive bound.
     * @returns Criteria `[field, "<", value]`.
     */
    lt(value: any): Array<any> {
        return [this, '<', value];
    }

    /**
     * Creates a not-equal (`!=`) criteria.
     *
     * @param value - Value that the field must not equal.
     * @returns Criteria `[field, "!=", value]`.
     */
    ne(value: any): Array<any> {
        return [this, '!=', value];
    }

    /**
     * Creates a `LIKE` criteria with the exact pattern provided.
     *
     * @param value - SQL LIKE pattern (use `%` / `_` wildcards as needed).
     * @returns Criteria `[field, "like", value]`.
     * @example
     * Criteria("Name").like("A%");
     */
    like(value: any): Array<any> {
        return [this, 'like', value];
    }

    /**
     * Creates a `LIKE 'value%'` (starts-with) criteria.
     *
     * @param value - Prefix to match. Suffixed with `%`.
     * @returns Criteria `[field, "like", "value%"]`.
     * @example
     * Criteria("Name").startsWith("Jo"); // [["Name"], "like", "Jo%"]
     */
    startsWith(value: string): Array<any> {
        return [this, 'like', value + '%'];
    }

    /**
     * Creates a `NOT IN` criteria.
     *
     * @param values - Array of disallowed values. Wrapped as `[values]`.
     * @returns Criteria `[field, "not in", [values]]`.
     */
    notIn(values: any[]): Array<any> {
        return [this, 'not in', [values]];
    }

    /**
     * Creates a `NOT LIKE` criteria.
     *
     * @param value - SQL LIKE pattern that the field must not match.
     * @returns Criteria `[field, "not like", value]`.
     */
    notLike(value: any): Array<any> {
        return [this, 'not like', value];
    }
}

const TOKEN_IDENTIFIER = 1;
const TOKEN_OPERATOR = 2;
const TOKEN_VALUE = 3;
const TOKEN_PARAM = 4;

interface Token {
    t: number;
    v: any;
}

interface ParseError {
    error: string;
    pos: number;
}

class ParseError extends Error {
    declare expression: string;
    declare error: string;
    declare position: number;

    constructor(expression: string, error: string, position: number) {
        super('Error parsing expression: "' + expression + '", "' +
            error + ', position: ' + position);
        this.expression = expression;
        this.error = error;
        this.position = position;
    }
}

function tokenize(expression: string): Token[] {
    let end: number, v: any;
    const tokens: Token[] = [];
    const l = expression.length;
    const l1 = expression.length - 1;
    let openParens = 0;
    let index: number;
    let ch: string;

    function skipWhiteSpace() {
        while (index < l) {
            ch = expression.charAt(index);

            if (ch === ' ' || ch === '\t') {
                index++;
                continue;
            }

            break;
        }
    }

    function readString() {
        end = index;
        let foundDoubles = false;
        while (end++ < l1) {
            ch = expression.charAt(end);
            if (ch === "'") {
                if (end < l1 && expression.charAt(end + 1) === "'") {
                    end++;
                    foundDoubles = true;
                    continue;
                }
                else
                    break;
            }
        }

        if (end === index ||
            expression.charAt(end) !== "'")
            throw new ParseError(expression, 'unterminated string', index);

        v = expression.substring(index + 1, end);
        if (foundDoubles)
            v = v.replace(/''/g, "'");
    }

    function readNumber() {
        end = index;
        let foundDot = false;
        while (end < l1) {
            ch = expression.charAt(end + 1);
            if ((ch >= '0' && ch <= '9') ||
                (!foundDot && (ch === '.'))) {
                if (ch === '.')
                    foundDot = true;
                end++;
            }
            else
                break;
        }

        v = parseFloat(expression.substring(index, end + 1));
    }

    function readIdentifier() {
        end = index;
        while (end < l1) {
            ch = expression.charAt(end + 1);
            if (ch == '_' ||
                (ch >= 'A' && ch <= 'Z') ||
                (ch >= 'a' && ch <= 'z') ||
                (ch >= '0' && ch <= '9')) {
                end++;
            }
            else
                break;
        }

        v = expression.substring(index, end + 1);
    }

    function readParam() {
        index++;
        readIdentifier();
        if (!v.length)
            throw new ParseError(expression, 'expected parameter name', index);
    }

    function readArrayList() {
        skipWhiteSpace();

        if (index >= l || expression.charAt(index) != '(') {
            if (index < l && expression.charAt(index) == '@') {
                readParam();
                index = end;
                return;
            }
            throw new ParseError(expression, 'expected parenthesis', index);
        }

        index++;

        let values = [];

        while (true) {
            skipWhiteSpace();

            if (index >= l)
                throw new ParseError(expression, 'expected parenthesis', index);

            ch = expression.charAt(index);

            if (ch == ',') {
                if (values.length == 0)
                    throw new ParseError(expression, 'unexpected comma', index);
                index++;
                skipWhiteSpace();
                ch = expression.charAt(index);
            }
            else if (ch == ')') {
                break;
            }
            else if (values.length > 0)
                throw new ParseError(expression, 'expected comma', index);


            if (ch === "'") {
                readString();

                values.push(v);
                index = end + 1;
                continue;
            }

            if (ch >= '0' && ch <= '9') {
                readNumber();

                values.push(v);
                index = end + 1;
                continue;
            }

            if (ch == 'n') {
                readIdentifier();

                if (v === 'null') {
                    values.push(null)
                    index = end + 1;
                    continue;
                }
            }

            throw new ParseError(expression, 'unexpected token', index);
        }

        v = values;
    }

    for (index = 0; index < l; index++) {
        ch = expression.charAt(index);
        if (ch === ' ' || ch === '\t')
            continue;

        if (ch == '_' ||
            (ch >= 'A' && ch <= 'Z') ||
            (ch >= 'a' && ch <= 'z')) {
            readIdentifier();
            let w = v.toLowerCase();
            if (w == 'is') {
                index = end + 1;
                skipWhiteSpace();

                readIdentifier();
                w = v.toLowerCase();
                if (w === 'not') {
                    index = end + 1;
                    skipWhiteSpace();
                    readIdentifier();
                    if (v.toLowerCase() === 'null') {
                        tokens.push({
                            t: TOKEN_OPERATOR,
                            v: 'is not null'
                        });
                    }
                    else
                        throw new ParseError(expression, 'expected "null"', index);
                }
                else if (w === 'null') {
                    tokens.push({
                        t: TOKEN_OPERATOR,
                        v: 'is null'
                    });
                }
                else
                    throw new ParseError(expression, 'expected "null" or "not" keyword', index);
            }
            else if (w === 'and' || w === 'or' || w === 'xor') {
                tokens.push({
                    t: TOKEN_OPERATOR,
                    v: w
                });
            }
            else if (w === 'not') {
                const currentEnd = end;

                index = end + 1;
                skipWhiteSpace();

                readIdentifier();
                w = v.toLowerCase();

                if (w === 'in') {
                    index = end + 1;

                    tokens.push({
                        t: TOKEN_OPERATOR,
                        v: 'not in'
                    });

                    readArrayList();

                    if (typeof v === "string") {
                        tokens.push({
                            t: TOKEN_PARAM,
                            v: v
                        })
                    }
                    else {
                        tokens.push({
                            t: TOKEN_VALUE,
                            v: v
                        });
                    }
                }
                else if (w === "like") {
                    tokens.push({
                        t: TOKEN_OPERATOR,
                        v: 'not like'
                    });

                    index = end;
                } else {
                    tokens.push({
                        t: TOKEN_OPERATOR,
                        v: 'not'
                    });

                    index = currentEnd;
                }

                continue;
            }
            else if (w === 'in') {
                tokens.push({
                    t: TOKEN_OPERATOR,
                    v: 'in'
                });

                index = end + 1;

                readArrayList();

                if (typeof v === "string") {
                    tokens.push({
                        t: TOKEN_PARAM,
                        v: v
                    })
                }
                else {
                    tokens.push({
                        t: TOKEN_VALUE,
                        v: v
                    });
                }

                continue;
            }
            else if (w === "like") {
                tokens.push({
                    t: TOKEN_OPERATOR,
                    v: 'like'
                });
            }
            else {
                tokens.push({
                    t: TOKEN_IDENTIFIER,
                    v: v
                });
            }

            index = end;
            continue;
        }

        if (ch === '@') {
            readParam();
            tokens.push({
                t: TOKEN_PARAM,
                v: v
            });
            index = end;
            continue;
        }

        if ((((ch === '-') || (ch === '+')) &&
            index < l1 &&
            expression.charAt(index + 1) >= '0' &&
            expression.charAt(index + 1) <= '9') ||
            (ch >= '0' && ch <= '9')) {
            end = index;
            readNumber();

            tokens.push({
                t: TOKEN_VALUE,
                v: v
            });

            index = end;
            continue;
        }

        if (ch === "'") {
            end = index;
            readString();

            tokens.push({
                t: TOKEN_VALUE,
                v: v
            });
            index = end;
            continue;
        }

        if (ch === '=') {
            tokens.push({
                t: TOKEN_OPERATOR,
                v: ch
            });
            continue;
        }

        if (ch === '(') {
            openParens++;
            tokens.push({
                t: TOKEN_OPERATOR,
                v: ch
            });
            continue;
        }

        if (ch == ')') {
            if (openParens <= 0)
                throw new ParseError(expression, 'unexpected parenthesis', index);

            openParens--;
            tokens.push({
                t: TOKEN_OPERATOR,
                v: ch
            });
            continue;
        }

        if (ch === '>' || ch === '<') {
            if (index < l1 &&
                expression.charAt(index + 1) === '=') {
                tokens.push({
                    t: TOKEN_OPERATOR,
                    v: ch + '='
                });
                index++;
            }
            else {
                tokens.push({
                    t: TOKEN_OPERATOR,
                    v: ch
                });
            }
            continue;
        }

        throw new ParseError(expression, 'unknown token', index);
    }

    if (openParens > 0)
        throw new ParseError(expression, 'missing parenthesis', index);

    return tokens;
}

const operatorPrecedence: Record<string, number> = {
    '=': 4,
    '>': 4,
    '<': 4,
    '>=': 4,
    '<=': 4,
    '<>': 4,
    '!=': 4,
    'like': 5,
    'not like': 5,
    'in': 5,
    'not in': 5,
    'is null': 5,
    'is not null': 5,
    'not': 6,
    'and': 7,
    'xor': 8,
    'or': 9
}

function shuntingYard(tokens: Token[]): Token[] {
    const result: Token[] = [];
    const stack: Token[] = [];
    for (const token of tokens) {
        if (token.t === TOKEN_OPERATOR) {
            const precedence = operatorPrecedence[token.v];

            if (precedence != null) {
                while (stack.length) {
                    const prev = stack[stack.length - 1];
                    if (prev.t !== TOKEN_OPERATOR || prev.v == '(')
                        break;
                    const prevPrecedence = operatorPrecedence[prev.v];
                    if (prevPrecedence == null || prevPrecedence > precedence)
                        break;

                    result.push(stack.pop());
                }

                stack.push(token);
            }
            else if (token.v === '(') {
                stack.push(token);
            }
            else if (token.v === ')') {
                while (stack.length &&
                    (stack[stack.length - 1].t !== TOKEN_OPERATOR ||
                        stack[stack.length - 1].v !== '(')) {
                    result.push(stack.pop());
                }

                stack.pop();
            }
            else
                result.push(token);
        }
        else
            result.push(token);
    }

    while (stack.length) {
        const tok = stack.pop();

        if (tok.t == TOKEN_OPERATOR &&
            (tok.v === '(' || tok.v === ')'))
            throw new Error("Mismatched parentheses in criteria expression!");

        result.push(tok);
    }

    return result;
}

function rpnTokensToCriteria(rpnTokens: Token[], getParam?: (name: string) => any): any[] {
    const stack: any[] = [];

    for (const token of rpnTokens) {
        switch (token.t) {
            case TOKEN_IDENTIFIER:
                {
                    stack.push([token.v]);
                    break;
                }

            case TOKEN_VALUE:
                {
                    stack.push(Array.isArray(token.v) ? [token.v] : token.v);
                    break;
                }

            case TOKEN_PARAM:
                {
                    if (!getParam)
                        throw new Error("getParam must be passed for parameterized expressions!");
                    const prm = getParam(token.v)
                    stack.push(Array.isArray(prm) ? [prm] : prm);
                    break;
                }

            case TOKEN_OPERATOR:
                {
                    switch (token.v as string) {
                        case 'not':
                        case 'is null':
                        case 'is not null':
                            if (!stack.length)
                                throw new Error(`Unary operator "${token.v}" requires a value!`);

                            stack.push([token.v, stack.pop()]);
                            break;
                        default:
                            if (stack.length < 2)
                                throw new Error(`Binary operator "${token.v}" requires two values!`);

                            const r = stack.pop();
                            const l = stack.pop();
                            stack.push([l, token.v, r]);
                            break;
                    }
                    break;
                }
            default:
                throw new Error("Invalid operator type: " + token.t + "!");
        }
    }

    if (stack.length != 1)
        throw new Error("Error evaluating expression!");

    return stack.pop();
}

function internalParse(expression: string, getParam?: (name: string) => any) {
    const tokens = tokenize(expression);
    const rpnTokens = shuntingYard(tokens);
    return rpnTokensToCriteria(rpnTokens, getParam);
}

/**
 * Parses a criteria expression string to Serenity criteria array format.
 *
 * @remarks
 * Supports named parameters via `@name` placeholders. Operator precedence is handled
 * via a shunting-yard pass; string literals use single quotes with `''` escaping.
 * @param expression - Expression text, e.g. `"A >= @p1 and B < @p2"`.
 * @param params - Dictionary mapping parameter names to values, e.g. `{ p1: 5, p2: 4 }`.
 * @returns Serenity criteria array, e.g. `[[["A"], ">=", 5], "and", [["B"], "<", 4]]`.
 * @example
 * `parseCriteria('A >= @p1 and B < @p2', { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]`
 */
export function parseCriteria(expression: string, params?: any): any[];
/**
 * Parses a tagged-template criteria expression to Serenity criteria array format.
 *
 * @remarks
 * Each interpolated value becomes an auto-named `@__N` parameter, avoiding manual
 * parameter dictionaries and SQL-injection-prone concatenation.
 * @param strings - Template string fragments.
 * @param values - Interpolated values (one per placeholder).
 * @returns Serenity criteria array.
 * @example
 * let a = 5, b = 4;
 * parseCriteria`A >= ${a} and B < ${b}`; // [[["A"], ">=", 5], "and", [["B"], "<", 4]]
 */
export function parseCriteria(strings: TemplateStringsArray, ...values: any[]): any[];
export function parseCriteria(exprOrStrings: TemplateStringsArray | string, ...values: any[]): any[] {
    if (!exprOrStrings?.length)
        return [];

    if (typeof exprOrStrings === "string") {
        return internalParse(exprOrStrings,
            values == null || values[0] == null ? null : name => values[0][name]);
    }
    else if (!values.length)
        return internalParse(exprOrStrings.join(''));

    const expression = String.raw({ raw: exprOrStrings }, ...values.map((x, i) => '@__' + i));
    return internalParse(expression, name => name.startsWith('__') ?
        values[parseInt(name.substring(2), 10)] : void 0);
}

/**
 * String constants for every operator that can appear in a Serenity criteria expression.
 *
 * @remarks
 * Values match the wire-format tokens accepted by the server (e.g. `"="`, `"like"`,
 * `"is null"`). Exposed also as {@link Criteria.Operator} for convenience.
 */
export enum CriteriaOperator {
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

/**
 * Creates a fluent {@link CriteriaBuilder} for the given field.
 *
 * @remarks
 * The returned builder extends `Array` so it doubles as a field token. A prototype
 * fixup handles environments where subclassing `Array` is unreliable.
 * @param field - Field name / property key, e.g. `"Amount"` or `"Customer.Name"`.
 * @returns A {@link CriteriaBuilder} bound to the field.
 * @example
 * Criteria("Age").ge(18);
 * @example
 * Criteria("Name").contains("acme");
 */
export function Criteria(field: string) {
    const builder = CriteriaBuilder.of(field);
    // workaround for subclassing array until corelib switched to ES6
    !(builder as any).eq && ((builder as any).__proto__ = CriteriaBuilder.prototype);
    return builder as CriteriaBuilder
};

/**
 * Helpers for composing and inspecting Serenity criteria arrays.
 *
 * @remarks
 * Criteria are plain arrays in the form `[left, operator, right]` (binary),
 * `[operator, operand]` (unary), or nested with `"and"`/`"or"` joiners.
 * The static helpers here handle empty-value short-circuiting so callers can
 * unconditionally combine optional filters.
 */
export namespace Criteria {
    /** Alias for {@link CriteriaOperator} — the set of valid operator tokens. */
    export const Operator = CriteriaOperator;

    /**
     * Returns `true` if the criteria is empty / falsy.
     *
     * @param c - Criteria array to test. `null`/`undefined` counts as empty.
     * @returns `true` if `c` is `null`, `undefined`, `[]`, or `[""]`.
     */
    export function isEmpty(c: any[]): boolean {
        return c == null ||
            c.length === 0 ||
            (c.length === 1 && typeof c[0] === "string" && c[0].length === 0);
    };

    /**
     * Joins two criteria with an operator, skipping empty sides.
     *
     * @param c1 - Left criteria. If empty, `c2` is returned as-is.
     * @param op - Join operator, typically `"and"` or `"or"` / `"xor"`.
     * @param c2 - Right criteria. If empty, `c1` is returned as-is.
     * @returns `[c1, op, c2]` or whichever side is non-empty, if the other is empty.
     */
    export function join(c1: any[], op: string, c2: any[]): any[] {
        if (Criteria.isEmpty(c1))
            return c2;

        if (Criteria.isEmpty(c2))
            return c1;

        return [c1, op, c2];
    };

    /**
     * Negates a criteria with the `not` operator.
     *
     * @param c - Criteria to negate.
     * @returns `["not", c]`.
     */
    export function not(c: any[]) {
        return ['not', c]
    }


    /**
     * Combines two or more criteria with `and`, skipping empty entries.
     *
     * @param c1 - First criteria.
     * @param c2 - Second criteria.
     * @param rest - Additional criteria joined incrementally with `and`.
     * @returns Combined criteria or the sole non-empty input if others are empty.
     */
    export function and(c1: any[], c2: any[], ...rest: any[][]) {
        let result = Criteria.join(c1, 'and', c2);
        if (rest) {
            for (const k of rest)
                result = Criteria.join(result, 'and', k);
        }

        return result;
    };

    /**
     * Combines two or more criteria with `or`, skipping empty entries.
     *
     * @param c1 - First criteria.
     * @param c2 - Second criteria.
     * @param rest - Additional criteria joined incrementally with `or`.
     * @returns Combined criteria or the sole non-empty input if others are empty.
     */
    export function or(c1: any[], c2: any[], ...rest: any[][]) {
        let result = Criteria.join(c1, 'or', c2);

        if (rest) {
            for (const k of rest)
                result = Criteria.join(result, 'or', k);
        }

        return result;
    }

    /**
     * Wraps a criteria in parentheses (compatibility helper).
     *
     * @remarks
     * Produces `["()", c]`. The server treats this as a grouping no-op but it can
     * preserve intended precedence when criteria are serialized. Returns `c` unchanged if empty.
     * @param c - Criteria to wrap.
     * @returns `["()", c]` or `c` if empty.
     */
    export function paren(c: any[]): any[] {
        return Criteria.isEmpty(c) ? c : ['()', c];
    }

    /**
     * Alias for {@link parseCriteria} — parses a criteria expression string or tagged template.
     *
     * @remarks
     * Accepts either `"A >= @p1"` with a params object, or a tagged template
     * `` Criteria.parse`A >= ${value}` ``. See {@link parseCriteria} for details.
     * @example
     * Criteria.parse("A >= @p1 and B < @p2", { p1: 5, p2: 4 });
     * @example
     * let a = 5, b = 4;
     * Criteria.parse`A >= ${a} and B < ${b}`;
     */
    export const parse = parseCriteria;
}