import { Group, GroupTotals } from "../../src/core/group";

describe('Group', () => {
    it('should set readonly __group to true', () => {
        const group = new Group();

        expect(group.__group).toBeTruthy();
    });

    it('should not be equal if value is not equal', () => {
        const group1 = new Group();
        const group2 = new Group();

        group1.value = 1;
        group2.value = 2;

        expect(group1.equals(group2)).toBeFalsy();

    });

    it('should not be equal if count is not equal', () => {
        const group1 = new Group();
        const group2 = new Group();

        group1.count = 1;
        group2.count = 2;

        expect(group1.equals(group2)).toBeFalsy();

    });

    it('should not be equal if collapsed is not equal', () => {
        const group1 = new Group();
        const group2 = new Group();

        group1.collapsed = true;
        group2.collapsed = false;

        expect(group1.equals(group2)).toBeFalsy();

    });

    it('should be equal if properties are empty', () => {
        const group1 = new Group();
        const group2 = new Group();

        expect(group1.equals(group2)).toBeTruthy();
    });

    it('should be equal if properties are equal', () => {
        const group1 = new Group();
        const group2 = new Group();

        group1.value = 1;
        group2.value = 1;

        group1.count = 1;
        group2.count = 1;

        group1.collapsed = true;
        group2.collapsed = true;

        expect(group1.equals(group2)).toBeTruthy();
    });
});

describe('GroupTotals', () => {
    it('should set readonly __group to true', () => {
        const groupTotals = new GroupTotals();

        expect(groupTotals.__groupTotals).toBeTruthy();
    });
});
