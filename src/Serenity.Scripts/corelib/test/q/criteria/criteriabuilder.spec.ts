import { Criteria } from "@/q";

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
});

it('should handle is null operators', () => {
    expect(Criteria('a').isNull()).toEqual(['is null', ['a']]);
    expect(Criteria('a').isNotNull()).toEqual(['is not null', ['a']]);
});