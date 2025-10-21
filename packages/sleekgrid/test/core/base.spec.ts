import { NonDataRow, preClickClassName } from "../../src/core/base";

describe('NonDataRow', () => {
    it('should set __nonDataRow to true', () => {
        const nonDataRow = new NonDataRow();

        expect(nonDataRow.__nonDataRow).toBeTruthy();
    });
});

it('exports not null or undefined preClickClassName longer than zero characters', () => {
    expect(preClickClassName).toBeDefined();
    expect(preClickClassName).not.toBeNull();

    expect(preClickClassName.length).toBeGreaterThan(0);
});
