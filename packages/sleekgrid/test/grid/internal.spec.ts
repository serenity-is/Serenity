import {
    defaultEmptyNode,
    defaultJQueryEmptyNode,
    defaultJQueryRemoveNode,
    defaultRemoveNode,
    simpleArrayEquals
} from '../../src/grid/internal';

function makeNode(): HTMLElement {
    const node = document.createElement("div");
    node.innerHTML = "<span>child</span><em>2</em>";
    document.body.appendChild(node);
    return node;
}

describe('defaultRemoveNode', () => {
    it('is a no-op for null/undefined', () => {
        expect(() => defaultRemoveNode(null)).not.toThrow();
        expect(() => defaultRemoveNode(undefined)).not.toThrow();
    });

    it('removes the node from its parent', () => {
        const node = makeNode();
        defaultRemoveNode(node);
        expect(document.body.contains(node)).toBe(false);
    });
});

describe('defaultEmptyNode', () => {
    it('is a no-op for null/undefined', () => {
        expect(() => defaultEmptyNode(null)).not.toThrow();
        expect(() => defaultEmptyNode(undefined)).not.toThrow();
    });

    it('clears the innerHTML but keeps the node', () => {
        const node = makeNode();
        defaultEmptyNode(node);
        expect(node.innerHTML).toBe("");
        expect(document.body.contains(node)).toBe(true);
    });
});

describe('defaultJQueryEmptyNode', () => {
    it('is a no-op for null', () => {
        expect(() => defaultJQueryEmptyNode.call(undefined, null)).not.toThrow();
    });

    it('uses the native path when called without a bound this', () => {
        const node = makeNode();
        defaultJQueryEmptyNode.call(undefined, node);
        expect(node.innerHTML).toBe("");
        expect(document.body.contains(node)).toBe(true);
    });

    it('uses the native path when this is a jQuery static (has .fn)', () => {
        const node = makeNode();
        const jqInstance = { empty: vi.fn(), remove: vi.fn() };
        const jqStatic = (() => jqInstance) as any;
        jqStatic.fn = {};
        defaultJQueryEmptyNode.call(jqStatic, node);
        expect(node.innerHTML).toBe("");
        expect(jqInstance.empty).not.toHaveBeenCalled();
    });

    it('uses the jQuery instance path when this is callable without .fn', () => {
        const node = makeNode();
        const empty = vi.fn();
        const jqInstance = (() => ({ empty, remove: vi.fn() })) as any;
        defaultJQueryEmptyNode.call(jqInstance, node);
        expect(empty).toHaveBeenCalled();
        // native path is not taken
        expect(node.innerHTML).not.toBe("");
    });
});

describe('defaultJQueryRemoveNode', () => {
    it('is a no-op for null', () => {
        expect(() => defaultJQueryRemoveNode.call(undefined, null)).not.toThrow();
    });

    it('uses the native path when called without a bound this', () => {
        const node = makeNode();
        defaultJQueryRemoveNode.call(undefined, node);
        expect(document.body.contains(node)).toBe(false);
    });

    it('uses the native path when this is a jQuery static (has .fn)', () => {
        const node = makeNode();
        const jqInstance = { empty: vi.fn(), remove: vi.fn() };
        const jqStatic = (() => jqInstance) as any;
        jqStatic.fn = {};
        defaultJQueryRemoveNode.call(jqStatic, node);
        expect(document.body.contains(node)).toBe(false);
        expect(jqInstance.remove).not.toHaveBeenCalled();
    });

    it('uses the jQuery instance path when this is callable without .fn', () => {
        const node = makeNode();
        const remove = vi.fn();
        const jqInstance = (() => ({ remove, empty: vi.fn() })) as any;
        defaultJQueryRemoveNode.call(jqInstance, node);
        expect(remove).toHaveBeenCalled();
        // native path is not taken
        expect(document.body.contains(node)).toBe(true);
    });
});

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
