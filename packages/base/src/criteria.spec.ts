import { Criteria } from "./criteria";

describe("Criteria.isEmpty", () => {
    it('returns true for null, undefined, empty array and array with one empty string', () => {
        expect(Criteria.isEmpty(null)).toBe(true);
        expect(Criteria.isEmpty(undefined)).toBe(true);
        expect(Criteria.isEmpty([])).toBe(true);
        expect(Criteria.isEmpty([""])).toBe(true);
    });

    it('returns false for any other array', () => {
        expect(Criteria.isEmpty([[]])).toBe(false);
        expect(Criteria.isEmpty([1])).toBe(false);
        expect(Criteria.isEmpty([[false]])).toBe(false);
        expect(Criteria.isEmpty(["a"])).toBe(false);
        expect(Criteria.isEmpty(["0"])).toBe(false);
    });
});

describe("Criteria.join", () => {
    it('ignores empty criteria', () => {
        expect(Criteria.join([], 'and', [])).toEqual([]);
        expect(Criteria.join(["a"], 'and', [])).toEqual(["a"]);
        expect(Criteria.join([], 'and', ["a"])).toEqual(["a"]);
    });

    it('puts operator between', () => {
        expect(Criteria.join(["a"], 'and', ["b"])).toEqual([["a"], "and", ["b"]]);
        expect(Criteria.join(["a"], 'or', ["b"])).toEqual([["a"], "or", ["b"]]);
    });
});

describe("Criteria.and", () => {
    it('ignores empty criteria', () => {
        expect(Criteria.and([], [])).toEqual([]);
        expect(Criteria.and(["a"], [])).toEqual(["a"]);
        expect(Criteria.and([], ["a"])).toEqual(["a"]);
    });

    it('puts AND between', () => {
        expect(Criteria.and(["a"], ["b"])).toEqual([["a"], 'and', ["b"]]);
    });

    it('can AND more than two ignoring empty ones', () => {
        expect(Criteria.and(["a"], ["b"], [])).toEqual([["a"], 'and', ["b"]]);
        expect(Criteria.and([], ["a"], ["b"], [])).toEqual([["a"], 'and', ["b"]]);
        expect(Criteria.and(["a"], ["b"], [], ["c"])).toEqual([[["a"], 'and', ["b"]], 'and', ["c"]]);
    });
});

describe("Criteria.or", () => {
    it('ignores empty criteria', () => {
        expect(Criteria.or([], [])).toEqual([]);
        expect(Criteria.or(["a"], [])).toEqual(["a"]);
        expect(Criteria.or([], ["a"])).toEqual(["a"]);
    });

    it('puts AND between', () => {
        expect(Criteria.or(["a"], ["b"])).toEqual([["a"], 'or', ["b"]]);
    });

    it('can AND more than two ignoring empty ones', () => {
        expect(Criteria.or(["a"], ["b"], [])).toEqual([["a"], 'or', ["b"]]);
        expect(Criteria.or([], ["a"], ["b"], [])).toEqual([["a"], 'or', ["b"]]);
        expect(Criteria.or(["a"], ["b"], [], ["c"])).toEqual([[["a"], 'or', ["b"]], 'or', ["c"]]);
    });
});

describe("Criteria.not", () => {
    it('puts not', () => {
        expect(Criteria.not([])).toEqual(['not', []]);
        expect(Criteria.not(["a"])).toEqual(['not', ["a"]]);
    });
});

describe("Criteria.paren", () => {
    it('ignores empty', () => {
        expect(Criteria.paren([])).toEqual([]);
    });

    it('puts parens', () => {
        expect(Criteria.paren(["a"])).toEqual(['()', ["a"]]);
    });
});

describe("Criteria function", () => {
    it('should handle comparison operators with string values', () => {
        expect(Criteria('a').eq('b')).toEqual([['a'], '=', 'b']);
        expect(Criteria('a').gt('b')).toEqual([['a'], '>', 'b']);
        expect(Criteria('a').ge('b')).toEqual([['a'], '>=', 'b']);
        expect(Criteria('a').le('b')).toEqual([['a'], '<=', 'b']);
        expect(Criteria('a').lt('b')).toEqual([['a'], '<', 'b']);
        expect(Criteria('a').ne('b')).toEqual([['a'], '!=', 'b']);
        expect(Criteria('a').bw('b', 'c')).toEqual([[['a'], '>=', 'b'], 'and', [['a'], '<=', 'c']]);
    });

    it('should handle comparison operators with integer values', () => {
        expect(Criteria('a').eq(1)).toEqual([['a'], '=', 1]);
        expect(Criteria('a').gt(1)).toEqual([['a'], '>', 1]);
        expect(Criteria('a').ge(1)).toEqual([['a'], '>=', 1]);
        expect(Criteria('a').le(1)).toEqual([['a'], '<=', 1]);
        expect(Criteria('a').lt(1)).toEqual([['a'], '<', 1]);
        expect(Criteria('a').ne(1)).toEqual([['a'], '!=', 1]);
        expect(Criteria('a').bw(1, 3)).toEqual([[['a'], '>=', 1], 'and', [['a'], '<=', 3]]);
    });

    it('should handle comparison operators with another field', () => {
        expect(Criteria('a').eq(Criteria('b'))).toEqual([['a'], '=', ['b']]);
        expect(Criteria('a').gt(Criteria('b'))).toEqual([['a'], '>', ['b']]);
        expect(Criteria('a').ge(Criteria('b'))).toEqual([['a'], '>=', ['b']]);
        expect(Criteria('a').le(Criteria('b'))).toEqual([['a'], '<=', ['b']]);
        expect(Criteria('a').lt(Criteria('b'))).toEqual([['a'], '<', ['b']]);
        expect(Criteria('a').ne(Criteria('b'))).toEqual([['a'], '!=', ['b']]);
        expect(Criteria('a').bw(Criteria('b'), Criteria('c'))).toEqual([[['a'], '>=', ['b']], 'and', [['a'], '<=', ['c']]]);
    });

    it('should handle in operators with string values', () => {
        expect(Criteria('a').in(['b', 'c'])).toEqual([['a'], 'in', [['b', 'c']]]);
        expect(Criteria('a').notIn(['b', 'c'])).toEqual([['a'], 'not in', [['b', 'c']]]);
    });

    it('should handle in operators with integer values', () => {
        expect(Criteria('a').in([1, 2])).toEqual([['a'], 'in', [[1, 2]]]);
        expect(Criteria('a').notIn([1, 2])).toEqual([['a'], 'not in', [[1, 2]]]);
    });

    it('should handle like operators', () => {
        expect(Criteria('a').like('b%')).toEqual([['a'], 'like', 'b%']);
        expect(Criteria('a').notLike('b%')).toEqual([['a'], 'not like', 'b%']);
        expect(Criteria('a').startsWith('b')).toEqual([['a'], 'like', 'b%']);
        expect(Criteria('a').endsWith('b')).toEqual([['a'], 'like', '%b']);
        expect(Criteria('a').contains('b')).toEqual([['a'], 'like', '%b%']);
    });

    it('should handle is null operators', () => {
        expect(Criteria('a').isNull()).toEqual(['is null', ['a']]);
        expect(Criteria('a').isNotNull()).toEqual(['is not null', ['a']]);
    });
});

describe("Criteria.parse", () => {
    it('tokenize expression', function () {

        expect((Criteria.parse('a=5'))).toEqual([['a'], '=', 5]);
        expect((Criteria.parse('_xy12z>=5.3'))).toEqual([['_xy12z'], '>=', 5.3]);
        expect((Criteria.parse("b='abc'"))).toEqual([['b'], '=', "abc"]);
        expect((Criteria.parse("c='d''q'"))).toEqual([['c'], '=', "d'q"]);

        expect((Criteria.parse("a ='b' and\tc=3 or d='x'"))).toEqual(
            [[[['a'], '=', 'b'], 'and', [['c'], '=', 3]], 'or', [['d'], '=', 'x']]
        );

        expect((Criteria.parse("c=5 or (d=6 and e=3)"))).toEqual(
            [[['c'], '=', 5], 'or', [[['d'], '=', 6], 'and', [['e'], '=', 3]]]
        );
    });

    it('should parse in query with integers', () => {
        expect((Criteria.parse("abc in (2)"))).toEqual(
            [['abc'], 'in', [[2]]]
        );

        expect((Criteria.parse("abc in (2, 3)"))).toEqual(
            [['abc'], 'in', [[2, 3]]]
        );
    });

    it('should parse in query with decimals', () => {
        expect((Criteria.parse("abc in (2.12)"))).toEqual(
            [['abc'], 'in', [[2.12]]]
        );

        expect((Criteria.parse("abc in (2.34, 3.67)"))).toEqual(
            [['abc'], 'in', [[2.34, 3.67]]]
        );
    });

    it('can parse expression with is null', () => {
        expect((Criteria.parse("a > 5 or c is null"))).toEqual(
            [[['a'], '>', 5], 'or', ['is null', ['c']]]
        );

        expect((Criteria.parse("a is not null and c is null"))).toEqual(
            [['is not null', ['a']], 'and', ['is null', ['c']]]
        );
    });

    it('can parse expression with not in', () => {
        expect((Criteria.parse("a not in (1)"))).toEqual(
            [['a'], 'not in', [[1]]]
        );

        expect((Criteria.parse("a not in (3, 5, 7)"))).toEqual(
            [['a'], 'not in', [[3, 5, 7]]]
        );

        expect((Criteria.parse("a not in @p", { p: [1, 2, 3] }))).toEqual(
            [['a'], 'not in', [[1, 2, 3]]]
        );

        expect((Criteria.parse`a not in ${[1, 2, 3]}`)).toEqual(
            [['a'], 'not in', [[1, 2, 3]]]
        );
    });

    it('can parse expression with not like', () => {
        expect((Criteria.parse("a not like '%a%'"))).toEqual(
            [['a'], 'not like', '%a%']
        );

        expect((Criteria.parse("a not like @p", { p: '%a%' }))).toEqual(
            [['a'], 'not like', '%a%']
        );

        expect((Criteria.parse`a not like ${'%a%'}`)).toEqual(
            [['a'], 'not like', '%a%']
        );
    });

    it('can parse expression with not', () => {
        expect((Criteria.parse("not (a > 5)"))).toEqual(['not', [['a'], '>', 5]]);
        expect((Criteria.parse("not a > 5"))).toEqual(['not', [['a'], '>', 5]]);
        expect((Criteria.parse("not (a > @p)", { p: 5 }))).toEqual(['not', [['a'], '>', 5]]);
        expect((Criteria.parse("not a > @p", { p: 5 }))).toEqual(['not', [['a'], '>', 5]]);
        expect((Criteria.parse`not (a > ${5})`)).toEqual(['not', [['a'], '>', 5]]);
        expect((Criteria.parse`not a > ${5}`)).toEqual(['not', [['a'], '>', 5]]);
    });


    it('should throw expected "null" or "not" error if there isn\'t any identifier after "is"', () => {
        expect(() => Criteria.parse("abc is   0")).toThrow('expected "null" or "not" keyword')
    });

    it('should throw expected "null" error if the identifier after is isn\'t null or not', () => {
        expect(() => Criteria.parse("abc is bcd")).toThrow('expected "null"')
    });

    it('should throw expected "null" error if identifier after "is not" isn\'t null', () => {
        expect(() => Criteria.parse("abc is   not    ")).toThrow('expected "null"')
    });

    it('should throw expected parenthesis error if in query doesnt start with a parenthesis', () => {
        expect(() => Criteria.parse("abc in 345)")).toThrow('expected parenthesis')
    });

    it('should throw expected parenthesis error if in query doesnt end with a parenthesis', () => {
        expect(() => Criteria.parse("abc in (345")).toThrow('expected parenthesis')
    });

    it('should throw unexpected comma error if in query starts with a comma', () => {
        expect(() => Criteria.parse("abc in (,345)")).toThrow('unexpected comma')
        expect(() => Criteria.parse("abc in (   ,    345)")).toThrow('unexpected comma')
    });

    it('should throw unexpected token error if in query has closing parenthesis after a comma', () => {
        expect(() => Criteria.parse("abc in (345,)")).toThrow('unexpected token')
        expect(() => Criteria.parse("abc in (345   , )")).toThrow('unexpected token')
    });

    it('should throw expected comma error if in query doesnt have a comma after value', () => {
        expect(() => Criteria.parse("abc in (345 412)")).toThrow('expected comma')
        expect(() => Criteria.parse("abc in (345     412  )")).toThrow('expected comma')
    });

    it('should throw unexpected token error if in query have items with types different than string, number, null', () => {
        expect(() => Criteria.parse("abc in (1, {a: 1}")).toThrow('unexpected token')
        expect(() => Criteria.parse("abc in (2, [1, 2])")).toThrow('unexpected token')
        expect(() => Criteria.parse("abc in ('3', [1, 2])")).toThrow('unexpected token')
        expect(() => Criteria.parse("abc in (undefined)")).toThrow('unexpected token')
        expect(() => Criteria.parse("abc in (bcd)")).toThrow('unexpected token')


        expect(() => Criteria.parse("abc in (1)")).not.toThrow('unexpected token')
        expect(() => Criteria.parse("abc in (1.2)")).not.toThrow('unexpected token')
        expect(() => Criteria.parse("abc in ('t ''es  t')")).not.toThrow('unexpected token')
        expect(() => Criteria.parse("abc in (null)")).not.toThrow('unexpected token')
    });

    it('should throw expected parameter name if name is missing', () => {
        expect(() => Criteria.parse("a >= @")).toThrow('expected parameter name')
        expect(() => Criteria.parse("a >= 5 and @")).toThrow('expected parameter name')
    });

    it('should throw unterminated string if closing quote is missing', () => {
        expect(() => Criteria.parse("a >= '")).toThrow('unterminated string')
        expect(() => Criteria.parse("a >= '5' and b like '")).toThrow('unterminated string')
        expect(() => Criteria.parse("' >= c")).toThrow('unterminated string')
    });

    it('should throw unknown token for multiple dots or commas, or dash', () => {
        expect(() => Criteria.parse("1.2.3")).toThrow('unknown token');
        expect(() => Criteria.parse("1.20,3,4")).toThrow('unknown token');
        expect(() => Criteria.parse("->5")).toThrow('unknown token');
    });

    it('should throw evaluation error for numbers followed by letters', () => {
        expect(() => Criteria.parse("4.567abcdefgh")).toThrow('Error evaluating expression!')
    });

    it('should throw unexpected parens for extra closing parens', () => {
        expect(() => Criteria.parse("((a > 5)))")).toThrow('unexpected parenthesis')
    });

    it('should throw unexpected parens for extra opening parens', () => {
        expect(() => Criteria.parse("(((a > 5))")).toThrow('missing parenthesis')
    });

    it('should throw expected "null" if is not followed by null or not', () => {
        expect(() => Criteria.parse("abc is bcd")).toThrow('expected "null"')
    });

    it('should throw if params not passed', () => {
        expect(() => Criteria.parse("a > @p1")).toThrow('getParam must be passed for parameterized expressions!')
    });

    it('cant handle tagged template with params, and uses undefined', () => {
        expect(Criteria.parse`a > @p1 and b > ${5}`)
            .toEqual([[["a"], ">", undefined], "and", [["b"], ">", 5]]);
    });

    it('should return empty criteria for empty string or null', () => {
        expect(Criteria.parse(null)).toEqual([]);
        expect(Criteria.parse("")).toEqual([]);
    });

    it('should work when tagged string does not have any params', () => {
        expect(Criteria.parse`a > 5`).toEqual([['a'], '>', 5]);
        expect(Criteria.parse.call(null, ["a > 5"], null));
    });

    it('parse expression', function () {
        expect((Criteria.parse('a=5'))).toEqual(
            [['a'], '=', 5]);
    });

    it('can parse like and equal', () => {
        expect((Criteria.parse("xyz = '5' and abc like 'test%'"))).toEqual(
            [[["xyz"], "=", "5"], "and", [["abc"], "like", "test%"]]);
    });

    it('parse expression with params', function () {
        const p1 = 5;
        expect((Criteria.parse('a=@p1', { p1 }))).toEqual(
            [['a'], '=', 5]);
    });

    it('can parse like and equal with params', () => {
        const p1 = '5';
        const p2 = 'test%';
        expect((Criteria.parse("xyz = @p1 and abc like @p2", { p1, p2 }))).toEqual(
            [[["xyz"], "=", "5"], "and", [["abc"], "like", "test%"]]);
    });

    it('can parse in with params', () => {
        expect(Criteria.parse("(a in @values) and b in @other", { values: [1, 2], other: ['x'] })).toEqual(
            [[["a"], 'in', [[1, 2]]], 'and', [["b"], 'in', [['x']]]]);
    });

    it('can parse like and equal with params', () => {
        const p1 = '5';
        const p2 = 'test%';
        expect((Criteria.parse("xyz = @p1 and abc like @p2", { p1, p2 }))).toEqual(
            [[["xyz"], "=", "5"], "and", [["abc"], "like", "test%"]]);
    });

    it('can parse in with tagged', () => {
        expect(Criteria.parse`a in ${[1, 2]}`).toEqual(
            [["a"], 'in', [[1, 2]]]);
    });

    it('can parse tagged where', function () {
        const p1 = 5;
        expect(Criteria.parse`a=${p1}`).toEqual(
            [['a'], '=', 5]);
    });

    it('can parse tagged like and equal', () => {
        var a = '5';
        var b = 'test%';
        expect((Criteria.parse`xyz = ${a} and abc like ${b}`)).toEqual(
            [[["xyz"], "=", "5"], "and", [["abc"], "like", "test%"]]);
    });
});