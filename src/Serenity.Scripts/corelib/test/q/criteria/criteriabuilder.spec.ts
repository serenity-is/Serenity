import { Criteria } from "@/q";

const Operator = Criteria.Operator;

it('should handle comparison operators with string values', () => {
    expect(Criteria('a').eq('b')).toEqual([['a'], Operator.eq, 'b']);
    expect(Criteria('a').gt('b')).toEqual([['a'], Operator.gt, 'b']);
    expect(Criteria('a').ge('b')).toEqual([['a'], Operator.ge, 'b']);
    expect(Criteria('a').le('b')).toEqual([['a'], Operator.le, 'b']);
    expect(Criteria('a').lt('b')).toEqual([['a'], Operator.lt, 'b']);
    expect(Criteria('a').ne('b')).toEqual([['a'], Operator.ne, 'b']);
    expect(Criteria('a').bw('b', 'c')).toEqual([[['a'], Operator.ge, 'b'], 'and', [['a'], Operator.le, 'c']]);
});

it('should handle comparison operators with integer values', () => {
    expect(Criteria('a').eq(1)).toEqual([['a'], Operator.eq, 1]);
    expect(Criteria('a').gt(1)).toEqual([['a'], Operator.gt, 1]);
    expect(Criteria('a').ge(1)).toEqual([['a'], Operator.ge, 1]);
    expect(Criteria('a').le(1)).toEqual([['a'], Operator.le, 1]);
    expect(Criteria('a').lt(1)).toEqual([['a'], Operator.lt, 1]);
    expect(Criteria('a').ne(1)).toEqual([['a'], Operator.ne, 1]);
    expect(Criteria('a').bw(1, 3)).toEqual([[['a'], Operator.ge, 1], 'and', [['a'], Operator.le, 3]]);
});

it('should handle comparison operators with another field', () => {
    expect(Criteria('a').eq(Criteria('b'))).toEqual([['a'], Operator.eq, ['b']]);
    expect(Criteria('a').gt(Criteria('b'))).toEqual([['a'], Operator.gt, ['b']]);
    expect(Criteria('a').ge(Criteria('b'))).toEqual([['a'], Operator.ge, ['b']]);
    expect(Criteria('a').le(Criteria('b'))).toEqual([['a'], Operator.le, ['b']]);
    expect(Criteria('a').lt(Criteria('b'))).toEqual([['a'], Operator.lt, ['b']]);
    expect(Criteria('a').ne(Criteria('b'))).toEqual([['a'], Operator.ne, ['b']]);
    expect(Criteria('a').bw(Criteria('b'), Criteria('c'))).toEqual([[['a'], Operator.ge, ['b']], 'and', [['a'], Operator.le, ['c']]]);
});

it('should handle in operators with string values', () => {
    expect(Criteria('a').in(['b', 'c'])).toEqual([['a'], Operator.in, [['b', 'c']]]);
    expect(Criteria('a').notIn(['b', 'c'])).toEqual([['a'], Operator.notIn, [['b', 'c']]]);
});

it('should handle in operators with integer values', () => {
    expect(Criteria('a').in([1, 2])).toEqual([['a'], Operator.in, [[1, 2]]]);
    expect(Criteria('a').notIn([1, 2])).toEqual([['a'], Operator.notIn, [[1, 2]]]);
});

it('should handle like operators', () => {
    expect(Criteria('a').like('b%')).toEqual([['a'], Operator.like, 'b%']);
    expect(Criteria('a').notLike('b%')).toEqual([['a'], Operator.notLike, 'b%']);
    expect(Criteria('a').startsWith('b')).toEqual([['a'], Operator.like, 'b%']);
    expect(Criteria('a').endsWith('b')).toEqual([['a'], Operator.like, '%b']);
});

it('should handle is null operators', () => {
    expect(Criteria('a').isNull()).toEqual([Operator.isNull, ['a']]);
    expect(Criteria('a').isNotNull()).toEqual([Operator.isNotNull, ['a']]);
});