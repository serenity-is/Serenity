import { useClassList, usePropBinding, useRef, useText } from "../src/hooks";
import { initPropHookSymbol } from "../src/prop-hook";

vi.mock("../src/disposing-listener", async (importActual) => {
    const actual = await importActual<typeof import("../src/disposing-listener")>();
    return {
        addDisposingListener: vi.fn(actual.addDisposingListener),
        removeDisposingListener: vi.fn(actual.removeDisposingListener),
    }
});

afterEach(() => {
    vi.clearAllMocks();
});

describe("useClassList", () => {
    it("creates a BasicClassList with initial value", () => {
        const classes = useClassList(['foo', 'bar']);
        expect(classes.value).toBe('foo bar');
        expect(classes.size).toBe(2);
    });

    it("creates empty class list without initial value", () => {
        const classes = useClassList();
        expect(classes.value).toBe('');
        expect(classes.size).toBe(0);
    });

    it("adds and removes classes", () => {
        const classes = useClassList();
        classes.add('test');
        expect(classes.contains('test')).toBe(true);
        expect(classes.value).toBe('test');
        classes.remove('test');
        expect(classes.contains('test')).toBe(false);
        expect(classes.value).toBe('');
    });

    it("toggles classes", () => {
        const classes = useClassList();
        classes.toggle('test');
        expect(classes.contains('test')).toBe(true);
        classes.toggle('test');
        expect(classes.contains('test')).toBe(false);
        classes.toggle('test', true);
        expect(classes.contains('test')).toBe(true);
        classes.toggle('test', false);
        expect(classes.contains('test')).toBe(false);
    });

    it("applies classes to element when called as function", () => {
        const classes = useClassList(['initial']);
        classes.add('added');
        const el = document.createElement('div');
        classes[initPropHookSymbol](el, "class");
        expect(el.className).toBe('initial added');
    });

    it("works as class attribute in JSX", () => {
        const classes = useClassList(['jsx-class']);
        classes.add('dynamic');
        const el = <div class={classes}>Test</div> as HTMLDivElement;
        expect(el.className).toBe('jsx-class dynamic');
    });

    it("updates JSX element reactively when classes change", () => {
        const classes = useClassList(['base']);
        const el = <div class={classes}>Test</div> as HTMLDivElement;
        expect(el.className).toBe('base');
        classes.add('updated');
        // Since the function call binds list to el.classList, changes affect el
        expect(el.className).toBe('base updated');
    });

    it("returns a function that returns a DOM token list", () => {
        const classes = useClassList(['initial']);
        const list = classes();
        expect(list).toBeInstanceOf(DOMTokenList);
        expect(list.value).toBe('initial');
    });

    it("returns the node's class list after initPropHook is called", () => {
        const classes = useClassList(['initial']);
        const el = document.createElement('div');
        classes[initPropHookSymbol](el, "class");
        const list = classes();
        expect(list).toBe(el.classList);
    });

    it("cleans up when element is disposed", () => {
        const accessor = useClassList(["test"]);
        const el = document.createElement("div");
        accessor[initPropHookSymbol](el, "class");
        expect(el.getAttribute("class")).toBe("test");
        expect(accessor() instanceof DOMTokenList).toBe(true);
        el.dispatchEvent(new Event("disposing"));
        expect(accessor()).toBeFalsy();
    });

    it("throws error when used with non-class attribute", () => {
        const classes = useClassList(['test']);
        const el = document.createElement('div');
        expect(() => classes[initPropHookSymbol](el, "id")).toThrow("useClassList can only be used for 'class' attribute.");
    });

    it("does not fail if attached to a null element", () => {
        const classes = useClassList(['test']);
        expect(() => classes[initPropHookSymbol](null, "class")).not.toThrow();
    });
});

describe("useText", () => {
    it("creates a Text node with initial value", () => {
        const [text] = useText('initial');
        expect(text.textContent).toBe('initial');
        expect(text.toString()).toBe('initial');
    });

    it("creates empty Text node without initial value", () => {
        const [text] = useText();
        expect(text.textContent).toBe('');
    });

    it("updates text content via setter", () => {
        const [text, setText] = useText('start');
        setText('updated');
        expect(text.textContent).toBe('updated');
        expect(text.toString()).toBe('updated');
    });
});

describe("useRef", () => {
    it("creates a ref object", () => {
        const ref = useRef();
        expect(ref).toEqual({ current: null });
    });
});

describe("usePropBinding", () => {
    it("creates a prop accessor with initial value", () => {
        const accessor = usePropBinding("initial");
        expect(accessor()).toBe("initial");
    });

    it("creates accessor with undefined initial value", () => {
        const accessor = usePropBinding();
        expect(accessor()).toBeUndefined();
    });

    it("sets value via function call", () => {
        const accessor = usePropBinding("start");
        expect(accessor("updated")).toBe("updated");
        expect(accessor()).toBe("updated");
    });

    it("binds to element and attribute via initPropHook", () => {
        const accessor = usePropBinding("value");
        const el = document.createElement("div");
        (accessor as any)[initPropHookSymbol](el, "data-test");
        expect(el.getAttribute("data-test")).toBe("value");
    });

    it("updates element when set after binding", () => {
        const accessor = usePropBinding("initial");
        const el = document.createElement("input");
        (accessor as any)[initPropHookSymbol](el, "value");
        expect(el.value).toBe("initial");
        accessor("changed");
        expect(el.value).toBe("changed");
    });

    it("throws error when used with multiple elements", () => {
        const accessor = usePropBinding("test");
        const el1 = document.createElement("div");
        const el2 = document.createElement("div");
        (accessor as any)[initPropHookSymbol](el1, "data-test");
        expect(() => (accessor as any)[initPropHookSymbol](el2, "data-test")).toThrow("usePropBinding can only be used with one element and one attribute. Create a new setter for each element / prop.");
    });

    it("throws error when used with different props on same element", () => {
        const accessor = usePropBinding("test");
        const el = document.createElement("div");
        (accessor as any)[initPropHookSymbol](el, "data-test");
        expect(() => (accessor as any)[initPropHookSymbol](el, "data-other")).toThrow("usePropBinding can only be used with one element and one attribute. Create a new setter for each element / prop.");
    });

    it("cleans up when element is disposed", () => {
        const accessor = usePropBinding("test");
        const el = document.createElement("div");
        accessor[initPropHookSymbol](el, "data-test");
        expect(el.getAttribute("data-test")).toBe("test");
        el.dispatchEvent(new Event("disposing"));
        accessor("new value");
        expect(el.getAttribute("data-test")).toBe("test"); // Should remain unchanged
    });

    it("cleans up when propBindingInit is called with null", () => {
        const accessor = usePropBinding("test");
        const el = document.createElement("div");
        accessor[initPropHookSymbol](el, "data-test");
        expect(el.getAttribute("data-test")).toBe("test");
        accessor[initPropHookSymbol](null, "data-test");
        accessor("new value");
        expect(el.getAttribute("data-test")).toBe("test"); // Should remain unchanged
    });
});
