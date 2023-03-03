import { Criteria } from "@/q";

const Operator = Criteria.Operator;
const Builder = Criteria.Builder;

it('should correctly return equals condition', () => {
    expect(
        Builder()
            .equals('a', 'b')
            .build()
    ).toEqual([['a'], Operator.eq, 'b']);
});

it('should correctly return equals condition with another field', () => {
    expect(
        Builder()
            .equals('a', Criteria('b'))
            .build()
    ).toEqual([['a'], Operator.eq, ['b']]);
});

it('should correctly return not equals condition', () => {
    expect(
        Builder()
            .not.equals('a', 'b')
            .build()
    ).toEqual([['a'], Operator.ne, 'b']);
});

it('should correctly return greater than condition', () => {
    expect(
        Builder()
            .greaterThan('a', Criteria('b'))
            .build()
    ).toEqual([['a'], Operator.gt, ['b']]);
});

it('should correctly return not greater than condition', () => {
    expect(
        Builder()
            .not.greaterThan('a', 2)
            .build()
    ).toEqual([['a'], Operator.le, 2]);
});

it('should correctly return greater than or equal condition', () => {
    expect(
        Builder()
            .greaterThanOrEqual('a', 'b')
            .build()
    ).toEqual([['a'], Operator.ge, 'b']);
});

it('should correctly return not greater than or equal condition', () => {
    expect(
        Builder()
            .not.greaterThanOrEqual('a', 'b')
            .build()
    ).toEqual([['a'], Operator.lt, 'b']);
});

it('should correctly return less than condition', () => {
    expect(
        Builder()
            .lessThan('a', 'b')
            .build()
    ).toEqual([['a'], Operator.lt, 'b']);
});

it('should correctly return not less than condition', () => {
    expect(
        Builder()
            .not.lessThan('a', 'b')
            .build()
    ).toEqual([['a'], Operator.ge, 'b']);
});

it('should correctly return less than or equal condition', () => {
    expect(
        Builder()
            .lessThanOrEqual('a', 'b')
            .build()
    ).toEqual([['a'], Operator.le, 'b']);
});

it('should correctly return not less than or equal condition', () => {
    expect(
        Builder()
            .not.lessThanOrEqual('a', 'b')
            .build()
    ).toEqual([['a'], Operator.gt, 'b']);
});

it('should correctly return in condition', () => {
    expect(
        Builder()
            .in('a', ['b', 'c'])
            .build()
    ).toEqual([['a'], Operator.in, ['b', 'c']]);
});

it('should correctly return not in condition', () => {
    expect(
        Builder()
            .not.in('a', ['b', 'c'])
            .build()
    ).toEqual([['a'], Operator.notIn, ['b', 'c']]);
});

it('should correctly return like condition', () => {
    expect(
        Builder()
            .like('a', 'b%')
            .build()
    ).toEqual([['a'], Operator.like, 'b%']);
});

it('should correctly return not like condition', () => {
    expect(
        Builder()
            .not.like('a', 'b%')
            .build()
    ).toEqual([['a'], Operator.notLike, 'b%']);
});

it('should correctly return is null condition', () => {
    expect(
        Builder()
            .isNull('a')
            .build()
    ).toEqual([Operator.isNull, ['a']]);
});

it('should correctly return is not null condition', () => {
    expect(
        Builder()
            .not.isNull('a')
            .build()
    ).toEqual([Operator.isNotNull, ['a']]);
});

it('should correctly return exists condition', () => {
    expect(
        Builder()
            .exists(
                Builder()
                    .equals('a', 'b')
                    .build())
            .build()
    ).toEqual([
        Operator.exists,
        [['a'], Operator.eq, 'b']
    ]);
});

it('should correctly return not exists condition', () => {
    expect(
        Builder()
            .not.exists(
            Builder()
                .equals('a', 'b')
                .build())
            .build()
    ).toEqual([
        Operator.not,
        [Operator.exists,
            [['a'], Operator.eq, 'b']]
    ]);
});

it('should correctly return between condition', () => {
    expect(
        Builder()
            .between('a', 'b', 'c')
            .build()
    ).toEqual([
        [['a'], Operator.ge, 'b'],
        Operator.and,
        [['a'], Operator.le, 'c']
    ]);
});

it('should correctly return not between condition', () => {
    expect(
        Builder()
            .not.between('a', 'b', 'c')
            .build()
    ).toEqual([
        [['a'], Operator.lt, 'b'],
        Operator.or,
        [['a'], Operator.gt, 'c']
    ]);
});

it('should correctly return not paren between', () => {
    expect(
        Builder()
            .not.paren.between('a', 'b', 'c')
            .build()
    ).toEqual([
        Operator.paren,
        [
            [['a'], Operator.lt, 'b'],
            Operator.or,
            [['a'], Operator.gt, 'c']
        ]
    ]);
});

it('should correctly return paren condition', () => {
    expect(
        Builder()
            .paren.equals('a', 'b')
            .build()
    ).toEqual([
        Operator.paren,
        [['a'], Operator.eq, 'b']
    ]);
});

it('should correctly return not equals condition', () => {
    expect(
        Builder()
            .not.equals('a', 'b')
            .build()
    ).toEqual([['a'], Operator.ne, 'b']);
});

it('should correctly return and condition', () => {
    expect(
        Builder()
            .equals('a', 'b')
            .and.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.eq, 'b'],
        Operator.and,
        [['c'], Operator.eq, 'd']
    ]);
});

it('should correctly return or condition', () => {
    expect(
        Builder()
            .equals('a', 'b')
            .or.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.eq, 'b'],
        Operator.or,
        [['c'], Operator.eq, 'd']
    ]);
});

it('should correctly return and condition with not', () => {
    expect(
        Builder()
            .not.equals('a', 'b')
            .and.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.ne, 'b'],
        Operator.and,
        [['c'], Operator.eq, 'd']
    ]);
});

it('should correctly return or condition with not', () => {
    expect(
        Builder()
            .not.equals('a', 'b')
            .or.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.ne, 'b'],
        Operator.or,
        [['c'], Operator.eq, 'd']
    ]);
});

it('should correctly return xor condition', () => {
    expect(
        Builder()
            .equals('a', 'b')
            .xor.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.eq, 'b'],
        Operator.xor,
        [['c'], Operator.eq, 'd']
    ]);
});

it('should correctly return xor condition with not', () => {
    expect(
        Builder()
            .not.equals('a', 'b')
            .xor.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.ne, 'b'],
        Operator.xor,
        [['c'], Operator.eq, 'd']
    ]);
});


it('should correctly return and condition with not and paren', () => {
    expect(
        Builder()
            .not.equals('a', 'b')
            .and.paren.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.ne, 'b'],
        Operator.and,
        [Operator.paren,
            [['c'], Operator.eq, 'd']]
    ]);
});

it('should correctly return or condition with not and paren', () => {
    expect(
        Builder()
            .not.equals('a', 'b')
            .or.paren.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.ne, 'b'],
        Operator.or,
        [Operator.paren,
            [['c'], Operator.eq, 'd']]
    ]);
});

it('should correctly return exists and paren', () => {
    expect(
        Builder()
            .paren.exists(
            Builder()
                .equals('a', 'b')
                .build()
        )
            .build()
    ).toEqual([
        Operator.paren,
        [Operator.exists,
            [['a'], Operator.eq, 'b']]
    ]);
});

it('should be able to use existing criteria with and', () => {
    const request = {
        criteria: [['a'], Operator.eq, 'b']
    };

    expect(
        Builder(request.criteria)
            .and.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.eq, 'b'],
        Operator.and,
        [['c'], Operator.eq, 'd']
    ]);
});

it('should be able to use existing criteria with or', () => {
    const request = {
        criteria: [['a'], Operator.eq, 'b']
    };

    expect(
        Builder(request.criteria)
            .or.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.eq, 'b'],
        Operator.or,
        [['c'], Operator.eq, 'd']
    ]);
});

it('should be able to use existing criteria with xor', () => {
    const request = {
        criteria: [['a'], Operator.eq, 'b']
    };

    expect(
        Builder(request.criteria)
            .xor.equals('c', 'd')
            .build()
    ).toEqual([
        [['a'], Operator.eq, 'b'],
        Operator.xor,
        [['c'], Operator.eq, 'd']
    ]);
});

it('should be able to parenthesize multiple criteria', () => {
    expect(
        Builder()
            .paren.join(Builder().equals('a', 'b').and.equals('c', 'd'))
    ).toEqual([
        Operator.paren,
        [
            [['a'], Operator.eq, 'b'],
            Operator.and,
            [['c'], Operator.eq, 'd']
        ]
    ]);
});
