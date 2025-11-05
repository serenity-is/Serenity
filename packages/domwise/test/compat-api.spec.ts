import { createElement, useImperativeHandle } from "../src/compat-api"
import { useRef } from "../src/hooks";

describe("createElement", () => {
    it("creates element with props and children", () => {
        const el = createElement('div', { className: 'foo' }, 'child1', 'child2');
        expect(el.tagName).toBe('DIV');
        expect(el.className).toBe('foo');
        expect(el.childNodes.length).toBe(2);
        expect(el.childNodes[0].textContent).toBe('child1');
        expect(el.childNodes[1].textContent).toBe('child2');
    });

    it("creates element with null props", () => {
        const el = createElement('span', null, 'text');
        expect(el.tagName).toBe('SPAN');
        expect(el.textContent).toBe('text');
    });

    it("handles children in props object", () => {
        const el = createElement('div', { children: 'inner' });
        expect(el.tagName).toBe('DIV');
        expect(el.textContent).toBe('inner');
    });

    it("treats string attr as child when props omitted", () => {
        const el = createElement('p', 'content');
        expect(el.tagName).toBe('P');
        expect(el.textContent).toBe('content');
    });

    it("treats array attr as child when props omitted", () => {
        const el = createElement('ul', ['item1', 'item2']);
        expect(el.tagName).toBe('UL');
        expect(el.childNodes.length).toBe(2);
        expect(el.childNodes[0].textContent).toBe('item1');
        expect(el.childNodes[1].textContent).toBe('item2');
    });

    it("prioritizes rest children over children in props", () => {
        const el = createElement('div', { children: 'from props' }, 'from rest');
        expect(el.tagName).toBe('DIV');
        expect(el.childNodes.length).toBe(1);
        expect(el.childNodes[0].textContent).toBe('from rest');
    });

    it("handles multiple children correctly", () => {
        const el = createElement('div', {}, createElement('span', null), 'text');
        expect(el.tagName).toBe('DIV');
        expect(el.childNodes.length).toBe(2);
        expect(el.childNodes[0].tagName).toBe('SPAN');
        expect(el.childNodes[1].textContent).toBe('text');
    });
});

describe("useImperativeHandle", () => {
    it("sets ref to init result", () => {
        const ref = useRef();
        useImperativeHandle(ref, () => 'value');
        expect(ref.current).toBe('value');
    });
});

