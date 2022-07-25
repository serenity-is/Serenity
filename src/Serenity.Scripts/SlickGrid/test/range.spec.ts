import { Range } from "../range";

describe('Range', () => {
    it('sets toRow to fromRow if toRow is undefined', () => {
        const range = new Range(1, 2);
        
        expect(range.toRow).toBe(range.fromRow);
    });

    it('sets toCell to fromCell if toRow is undefined', () => {
        const range = new Range(1, 2);

        expect(range.toCell).toBe(range.fromCell);
    });

    it('isSingleRow returns true if toRow and fromRow is the same', () => {
        const range = new Range(1, 2);

        expect(range.isSingleRow()).toBeTruthy();
    });

    it('isSingleRow returns false if toRow and fromRow is not the same', () => {
        const range = new Range(1, 2, 3, 2);

        expect(range.isSingleRow()).toBeFalsy();
    });

    it('isSingleCell returns true if row and the cell is the same ', () => {
        const range = new Range(1, 2, 1, 2);

        expect(range.isSingleCell()).toBeTruthy();
    });

    it('isSingleCell returns false if row is not the same ', () => {
        const range = new Range(1, 2, 2, 2);

        expect(range.isSingleCell()).toBeFalsy();
    });

    it('isSingleCell returns false if cell is not the same ', () => {
        const range = new Range(1, 2, 1, 3);

        expect(range.isSingleCell()).toBeFalsy();
    });

    it('isSingleCell returns false if row and cell is not the same ', () => {
        const range = new Range(1, 2, 2, 3);

        expect(range.isSingleCell()).toBeFalsy();
    });

    it('contains returns true if range contains it', () => {
        const [fromRow, fromCell, toRow, toCell] = [2, 5, 3, 6];
        const range = new Range(fromRow, fromCell, toRow, toCell);
        
        for (let currentRow = 0; currentRow < toRow + 4; currentRow++) {
            for (let currentCell = 0; currentCell < toCell + 4; currentCell++) {
                
                const expected = currentRow >= fromRow && currentRow <= toRow &&
                    currentCell >= fromCell && currentCell <= toCell;
                
                expect(range.contains(currentRow, currentCell)).toBe(expected);
            }
        }
    });

    it('toString should return only (fromRow:fromCell) if range contains only one cell', () => {
        const range = new Range(1, 2);
        
        expect(range.toString()).toBe('(1:2)');
    });

    it('toString should return (fromRow:fromCell - toRow:toCell) if range is not same row', () => {
        const range = new Range(1, 2, 2, 2);

        expect(range.toString()).toBe('(1:2 - 2:2)');
    });

    it('toString should return (fromRow:fromCell - toRow:toCell) if range is not same cell', () => {
        const range = new Range(1, 2, 1, 3);

        expect(range.toString()).toBe('(1:2 - 1:3)');
    });

    it('toString should return (fromRow:fromCell - toRow:toCell) if range is not single cell', () => {
        const range = new Range(1, 2, 2, 3);

        expect(range.toString()).toBe('(1:2 - 2:3)');
    });
});