/**
 * CriteriaBuilder is a class that allows to build unary or binary criteria with completion support.
 */
export class CriteriaBuilder extends Array {
    /**
     * Creates a between criteria.
     * @param fromInclusive from value
     * @param toInclusive to value
     */
    bw(fromInclusive: any, toInclusive: any): Array<any> {
        return [[this, '>=', fromInclusive], 'and', [this, '<=', toInclusive]];
    }

    /**
     * Creates a contains criteria
     * @param value contains value
     */
    contains(value: string): Array<any> {
        return [this, 'like', '%' + value + '%'];
    }

    /**
     * Creates a endsWith criteria
     * @param value endsWith value
     */
    endsWith(value: string): Array<any> {
        return [this, 'like', '%' + value];
    }

    /**
     * Creates an equal (=) criteria
     * @param value equal value
     */
    eq(value: any): Array<any> {
        return [this, '=', value];
    }

    /**
     * Creates a greater than criteria
     * @param value greater than value
     */
    gt(value: any): Array<any> {
        return [this, '>', value];
    }

    /**
     * Creates a greater than or equal criteria
     * @param value greater than or equal value
     */
    ge(value: any): Array<any> {
        return [this, '>=', value];
    }

    /**
     * Creates a in criteria
     * @param values in values
     */
    in(values: any[]): Array<any> {
        return [this, 'in', [values]];
    }

    /**
     * Creates a IS NULL criteria
     */
    isNull(): Array<any> {
        return ['is null', this];
    }

    /**
     * Creates a IS NOT NULL criteria
     */
    isNotNull(): Array<any> {
        return ['is not null', this];
    }

    /**
     * Creates a less than or equal to criteria
     * @param value less than or equal to value
     */
    le(value: any): Array<any> {
        return [this, '<=', value];
    }

    /**
     * Creates a less than criteria
     * @param value less than value
     */
    lt(value: any): Array<any> {
        return [this, '<', value];
    }

    /**
     * Creates a not equal criteria
     * @param value not equal value
     */
    ne(value: any): Array<any> {
        return [this, '!=', value];
    }

    /**
     * Creates a LIKE criteria
     * @param value like value
     */
    like(value: any): Array<any> {
        return [this, 'like', value];
    }

    /**
     * Creates a STARTS WITH criteria
     * @param value startsWith value
     */
    startsWith(value: string): Array<any> {
        return [this, 'like', value + '%'];
    }

    /**
     * Creates a NOT IN criteria
     * @param values array of NOT IN values
     */
    notIn(values: any[]): Array<any> {
        return [this, 'not in', [values]];
    }

    /**
     * Creates a NOT LIKE criteria
     * @param value not like value
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

const ParseError: any = function (expression: string, error: string, position: number) {
    this.expression = expression;
    this.error = error;
    this.position = position;
    this.toString = function () {
        return 'Error parsing expression: "' + expression + '", "' +
            error + ', position: ' + position;
    };
}

function tokenize(expression: string): Token[] {
    var end: number, v: any;
    var tokens: Token[] = [];
    var l = expression.length;
    var l1 = expression.length - 1;
    var openParens = 0;
    var index: number;
    var ch: string;

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
        var foundDoubles = false;
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

        v = expression.substr(index + 1, end - index - 1);
        if (foundDoubles)
            v = v.replace(/''/g, "'");
    }

    function readNumber() {
        end = index;
        var foundDot = false;
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

        v = expression.substr(index, end - index + 1);
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
            var w = v.toLowerCase();
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
    var result: Token[] = [];
    var stack: Token[] = [];
    for (var token of tokens) {
        if (token.t === TOKEN_OPERATOR) {
            var precedence = operatorPrecedence[token.v];

            if (precedence != null) {
                while (stack.length) {
                    var prev = stack[stack.length - 1];
                    if (prev.t !== TOKEN_OPERATOR || prev.v == '(')
                        break;
                    var prevPrecedence = operatorPrecedence[prev.v];
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
        var tok = stack.pop();

        if (tok.t == TOKEN_OPERATOR &&
            (tok.v === '(' || tok.v === ')'))
            throw "Mismatched parentheses in criteria expression!";

        result.push(tok);
    }

    return result;
}

function rpnTokensToCriteria(rpnTokens: Token[], getParam?: (name: string) => any): any[] {
    var stack: any[] = [];

    for (var token of rpnTokens) {
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
                    var prm = getParam(token.v)
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
                                throw new Error("Unary operator " + token.v + " requires a value!");

                            stack.push([token.v, stack.pop()]);
                            break;
                        default:
                            if (stack.length < 2)
                                throw new Error("Binary operator " + token.v + " requires two values!");

                            var r = stack.pop();
                            var l = stack.pop();
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
    var tokens = tokenize(expression);
    var rpnTokens = shuntingYard(tokens);
    return rpnTokensToCriteria(rpnTokens, getParam);
}

/** 
 * Parses a criteria expression to Serenity Criteria array format.
 * The string may optionally contain parameters like `A >= @p1 and B < @p2`.
 * @param expression The criteria expression.
 * @param params The dictionary containing parameter values like { p1: 10, p2: 20 }.
 * @example
 * parseCriteria('A >= @p1 and B < @p2', { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]
 */
export function parseCriteria(expression: string, params?: any): any[];
/** 
 * Parses a criteria expression to Serenity Criteria array format.
 * The expression may contain parameter placeholders like `A >= ${p1}`
 * where p1 is a variable in the scope.
 * @param strings The string fragments.
 * @param values The tagged template arguments.
 * @example 
 * var a = 5, b = 4;
 * parseCriteria`A >= ${a} and B < ${b}` // [[[a], '>=' 5], 'and', [[b], '<', 4]]
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

    var expression = String.raw({ raw: exprOrStrings }, ...values.map((x, i) => '@__' + i));
    return internalParse(expression, name => name.startsWith('__') ?
        values[parseInt(name.substring(2), 10)] : void 0);
}

/**
 * Enumeration of Criteria operator keys.
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
 * Creates a new criteria builder containg the passed field name.
 * @param field The field name.
 */
export function Criteria(field: string) {
    var builder = CriteriaBuilder.of(field);
    // workaround for subclassing array until corelib switched to ES6
    !(builder as any).eq && ((builder as any).__proto__ = CriteriaBuilder.prototype);
    return builder as CriteriaBuilder
}

/**
 * Ands two or more criteria together.
 * @param c1 First criteria.
 * @param c2 Second criteria.
 * @param rest Other criteria.
 */
Criteria.and = function and(c1: any[], c2: any[], ...rest: any[][]) {
    var result = Criteria.join(c1, 'and', c2);
    if (rest) {
        for (let k of rest)
            result = Criteria.join(result, 'and', k);
    }

    return result;
};

/** Provides access to the `CriteriaOperator` enum, e.g list of operator keys */
Criteria.Operator = CriteriaOperator;

/** 
 * Determines if a criteria is empty.
 */
Criteria.isEmpty = function isEmpty(c: any[]): boolean {
    return c == null ||
        c.length === 0 ||
        (c.length === 1 && typeof c[0] === "string" && c[0].length === 0);
};

/**
 * Joins two criteria together.
 * @param c1 First criteria.
 * @param op Operator to insert between, e.g. 'or', 'and'.
 * @param c2 Second criteria
 */
Criteria.join = function join(c1: any[], op: string, c2: any[]): any[] {
    if (Criteria.isEmpty(c1))
        return c2;

    if (Criteria.isEmpty(c2))
        return c1;

    return [c1, op, c2];
};

/**
 * Negates a criteria.
 * @param c Criteria to negate.
 */
Criteria.not = function not(c: any[]) {
    return ['not', c]
}

/**
 * Ors two or more criteria together.
 * @param c1 First criteria.
 * @param c2 Second criteria.
 * @param rest Other criteria.
 */
Criteria.or = function or(c1: any[], c2: any[], ...rest: any[][]) {
    var result = Criteria.join(c1, 'or', c2);

    if (rest) {
        for (let k of rest)
            result = Criteria.join(result, 'or', k);
    }

    return result;
}

/**
 * Puts a criteria in parens. Exists only for compatibility reasons.
 */
Criteria.paren = function parent(c: any[]): any[] {
    return Criteria.isEmpty(c) ? c : ['()', c];
}

/** 
 * Parses a criteria expression to Serenity Criteria array format.
 * The expression string may be a string literal, optionally containining 
 * parameters like `A >= @p1 and B < @p2`.
 * 
 * Or, the expression might be a tagged string literal that 
 * contain parameter placeholders like `A >= ${p1}`
 * where p1 is a variable in the scope.
 *
 * @example
 * Criteria.parse("A >= @p1 and B < @p2", { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]
 * 
 * @example
 * var a = 5; b = 4;
 * Criteria.parse`A >= ${a} and B < ${b}` // [[[a], '>=' 5], 'and', [[b], '<', 4]]
*/
Criteria.parse = parseCriteria;