import { Criteria } from "@/q";

test('tokenize expression', function () {

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

test('should parse in query with integers', () => {
    expect((Criteria.parse("abc in (2)"))).toEqual(
        [['abc'], 'in', [[2]]]
    );

    expect((Criteria.parse("abc in (2, 3)"))).toEqual(
        [['abc'], 'in', [[2, 3]]]
    );
});

test('should parse in query with decimals', () => {
    expect((Criteria.parse("abc in (2.12)"))).toEqual(
        [['abc'], 'in', [[2.12]]]
    );

    expect((Criteria.parse("abc in (2.34, 3.67)"))).toEqual(
        [['abc'], 'in', [[2.34, 3.67]]]
    );
});

test('should throw expected "null" or "not" error if there isn\'t any identifier after "is"', () => {
    expect(() => Criteria.parse("abc is   0")).toThrow('expected "null" or "not" keyword')
});

test('should throw expected "null" error if the identifier after is isn\'t null or not', () => {
    expect(() => Criteria.parse("abc is bcd")).toThrow('expected "null"')
});

test('should throw expected "null" error if identifier after "is not" isn\'t null', () => {
    expect(() => Criteria.parse("abc is   not    ")).toThrow('expected "null"')
});

test('should throw expected parenthesis error if in query doesnt start with a parenthesis', () => {
    expect(() => Criteria.parse("abc in 345)")).toThrow('expected parenthesis')
});

test('should throw expected parenthesis error if in query doesnt end with a parenthesis', () => {
    expect(() => Criteria.parse("abc in (345")).toThrow('expected parenthesis')
});

test('should throw unexpected comma error if in query starts with a comma', () => {
    expect(() => Criteria.parse("abc in (,345)")).toThrow('unexpected comma')
    expect(() => Criteria.parse("abc in (   ,    345)")).toThrow('unexpected comma')
});

test('should throw unexpected token error if in query has closing parenthesis after a comma', () => {
    expect(() => Criteria.parse("abc in (345,)")).toThrow('unexpected token')
    expect(() => Criteria.parse("abc in (345   , )")).toThrow('unexpected token')
});

test('should throw expected comma error if in query doesnt have a comma after value', () => {
    expect(() => Criteria.parse("abc in (345 412)")).toThrow('expected comma')
    expect(() => Criteria.parse("abc in (345     412  )")).toThrow('expected comma')
});

test('should throw unexpected token error if in query have items with types different than string, number, null', () => {
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

test('wwwwwwwwwww', () => {
    expect(() => Criteria.parse("abc is bcd")).toThrow('expected "null"')
});

test('parse expression', function () {
    expect((Criteria.parse('a=5'))).toEqual(
        [['a'], '=', 5]);
});

test('can parse like and equal', () => {
    expect((Criteria.parse("xyz = '5' and abc like 'test%'"))).toEqual(
        [[["xyz"], "=", "5"], "and", [["abc"], "like", "test%"]]);
});

test('parse expression with params', function () {
    const p1 = 5;
    expect((Criteria.parse('a=@p1', { p1 }))).toEqual(
        [['a'], '=', 5]);
});

test('can parse like and equal with params', () => {
    const p1 = '5';
    const p2 = 'test%';
    expect((Criteria.parse("xyz = @p1 and abc like @p2", { p1, p2}))).toEqual(
        [[["xyz"], "=", "5"], "and", [["abc"], "like", "test%"]]);
});

test('can parse tagged where', function () {
    const p1 = 5;
    expect(Criteria.parse`a=${p1}`).toEqual(
        [['a'], '=', 5]);
});

test('can parse tagged like and equal', () => {
    var a = '5';
    var b = 'test%';
    expect((Criteria.parse`xyz = ${a} and abc like ${b}`)).toEqual(
        [[["xyz"], "=", "5"], "and", [["abc"], "like", "test%"]]);
});