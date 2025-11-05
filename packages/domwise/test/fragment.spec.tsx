import { Fragment } from "../src/fragment";

describe("Fragment", () => {
    it("returns a DocumentFragment", () => {
        const frag = Fragment({});
        expect(frag).toBeInstanceOf(DocumentFragment);
    });

    it("appends children to the fragment", () => {
        const frag = Fragment({ children: ['text1', 'text2'] });
        expect(frag.childNodes.length).toBe(2);
        expect(frag.childNodes[0].textContent).toBe('text1');
        expect(frag.childNodes[1].textContent).toBe('text2');
    });

    it("handles single child", () => {
        const frag = Fragment({ children: 'single' });
        expect(frag.childNodes.length).toBe(1);
        expect(frag.childNodes[0].textContent).toBe('single');
    });

    it("handles element children", () => {
        const el = document.createElement('div');
        const frag = Fragment({ children: el });
        expect(frag.childNodes.length).toBe(1);
        expect(frag.childNodes[0]).toBe(el);
    });

    it("handles mixed children", () => {
        const el = document.createElement('span');
        const frag = Fragment({ children: [el, 'text'] });
        expect(frag.childNodes.length).toBe(2);
        expect(frag.childNodes[0]).toBe(el);
        expect(frag.childNodes[1].textContent).toBe('text');
    });

    it("handles null children", () => {
        const frag = Fragment({ children: null });
        expect(frag.childNodes.length).toBe(0);
    });

    it("handles undefined children", () => {
        const frag = Fragment({});
        expect(frag.childNodes.length).toBe(0);
    });
});

