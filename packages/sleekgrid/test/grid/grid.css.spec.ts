import { describe, expect, it } from "vitest";
import { type Column } from "../../src/core";
import { SleekGrid } from "../../src/grid";

describe('useCssVars', () => {

    it('should apply column positions via css vars after switching from style-rules mode', () => {
        const makeCols = (count: number) =>
            Array.from({ length: count }, (_, i): Column => ({
                id: `c${i}`,
                field: `f${i}`,
                name: `n${i}`,
                width: 100
            }));

        const container = document.createElement('div');
        const grid = new SleekGrid(container, [], makeCols(101), {
            useCssVars: true
        });

        grid.setColumns(makeCols(2));

        expect(container.classList.contains('sleek-vars')).toBe(true);
        expect(container.style.getPropertyValue('--l0')).toBe('0px');
        expect(container.style.getPropertyValue('--r0')).toBe('100px');
        expect(container.style.getPropertyValue('--l1')).toBe('100px');
        expect(container.style.getPropertyValue('--r1')).toBe('0px');
    });
});
