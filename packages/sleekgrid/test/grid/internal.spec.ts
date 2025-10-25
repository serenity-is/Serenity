import {
    simpleArrayEquals
} from '../../src/grid/internal';

describe('simpleArrayEquals', () => {
    it('should return false when first argument is not an array', () => {
        expect(simpleArrayEquals(null, [])).toBe(false);
    });

    it('should return false when second argument is not an array', () => {
        expect(simpleArrayEquals([], null)).toBe(false);
    });

    it('should return false when arrays have different length', () => {
        expect(simpleArrayEquals([1, 2], [1, 2, 3])).toBe(false);
    });

    it('should return false when arrays have different values', () => {
        expect(simpleArrayEquals([1, 2], [1, 3])).toBe(false);
    });

    it('should return true when arrays have same values', () => {
        expect(simpleArrayEquals([1, 2], [2, 1])).toBe(true);
    });

    it('should return true when arrays have same values but different order', () => {
        expect(simpleArrayEquals([1, 2], [2, 1])).toBe(true);
    });

    it('should not modify the original arrays', () => {
        const arr1 = [1, 2];
        const arr2 = [2, 1];

        simpleArrayEquals(arr1, arr2);

        expect(arr1).toEqual([1, 2]);
        expect(arr2).toEqual([2, 1]);
    });
});
