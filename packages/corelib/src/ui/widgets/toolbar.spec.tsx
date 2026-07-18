import { Toolbar, ToolbarButton, ToolButton } from "./toolbar";

describe("ToolButton", () => {
    it('clicking .tool-button directly calls onClick if it does not have disabled class', function () {
        const onClick = vi.fn();
        var btn = <ToolbarButton onClick={onClick}></ToolbarButton> as HTMLElement;
        btn.click();
        expect(onClick).toHaveBeenCalledTimes(1);
    });

    it('clicking .button-inner calls onClick if tool-button element does not have disabled class', function () {
        const onClick = vi.fn();
        var btn = <ToolbarButton onClick={onClick}></ToolbarButton> as HTMLElement;
        var inner = btn.querySelector<HTMLElement>(".button-inner");
        expect(inner).not.toBeNull();
        inner.click();
        expect(onClick).toHaveBeenCalledTimes(1);
    });

    it('clicking .tool-button directly does not call onClick if it has disabled class', function () {
        const onClick = vi.fn();
        var btn = <ToolbarButton onClick={onClick}></ToolbarButton> as HTMLElement;
        btn.classList.add("disabled");
        btn.click();
        expect(onClick).not.toHaveBeenCalled();
    });

    it('clicking .button-inner does not call onClick if tool-button element has disabled class', function () {
        const onClick = vi.fn();
        var btn = <ToolbarButton onClick={onClick}></ToolbarButton> as HTMLElement;
        btn.classList.add("disabled");
        var inner = btn.querySelector<HTMLElement>(".button-inner");
        expect(inner).not.toBeNull();
        inner.click();
        expect(onClick).not.toHaveBeenCalled();
    });

    it("can use ref with jsx", function () {
        const ref = vi.fn();
        var btn = <ToolbarButton ref={ref}></ToolbarButton> as HTMLElement;
        expect(ref).toHaveBeenCalledTimes(1);
        expect(ref).toHaveBeenCalledWith(btn);
    });

    it("can use ref without jsx", function () {
        const ref = vi.fn();
        var btn = ToolbarButton({ ref }) as HTMLElement;
        expect(ref).toHaveBeenCalledTimes(1);
        expect(ref).toHaveBeenCalledWith(btn);
    });

    it('renders with icon class when icon is specified', function () {
        var btn = ToolbarButton({ icon: "fa fa-plus" }) as HTMLElement;
        expect(btn.classList.contains("icon-tool-button")).toBe(true);
        expect(btn.querySelector("i")).not.toBeNull();
    });

    it('renders no-text class when no title is provided', function () {
        var btn = ToolbarButton({}) as HTMLElement;
        expect(btn.classList.contains("no-text")).toBe(true);
    });

    it('does not add no-text class when title is provided', function () {
        var btn = ToolbarButton({ title: "Click Me" }) as HTMLElement;
        expect(btn.classList.contains("no-text")).toBe(false);
    });

    it('sets data-action attribute', function () {
        var btn = ToolbarButton({ action: "save" }) as HTMLElement;
        expect(btn.dataset.action).toBe("save");
    });

    it('hides button when visible is false', function () {
        var btn = ToolbarButton({ visible: false }) as HTMLElement;
        expect(btn.hidden).toBe(true);
    });

    it('adds disabled class when disabled is true', function () {
        var btn = ToolbarButton({ disabled: true }) as HTMLElement;
        expect(btn.classList.contains("disabled")).toBe(true);
    });

    it('handles visible as a function', function () {
        const visibleFn = vi.fn(() => true);
        var btn = ToolbarButton({ visible: visibleFn }) as HTMLElement;
        // The updateInterface event should trigger the function
        btn.dispatchEvent(new Event("updateInterface"));
        expect(visibleFn).toHaveBeenCalled();
        expect(btn.style.display).not.toBe("none");
    });

    it('handles disabled as a function', function () {
        const disabledFn = vi.fn(() => true);
        var btn = ToolbarButton({ disabled: disabledFn }) as HTMLElement;
        btn.dispatchEvent(new Event("updateInterface"));
        expect(disabledFn).toHaveBeenCalled();
        expect(btn.classList.contains("disabled")).toBe(true);
    });

    it('sets hint as title attribute', function () {
        var btn = ToolbarButton({ hint: "Tooltip text" }) as HTMLElement;
        expect(btn.title).toBe("Tooltip text");
    });
});

describe("Toolbar", () => {
    it('creates toolbar with no buttons', () => {
        const toolbar = new Toolbar({}).init();
        expect(toolbar.domNode.classList.contains("s-Toolbar")).toBe(true);
        expect(toolbar.domNode.classList.contains("clearfix")).toBe(true);
        toolbar.destroy();
    });

    it('renders buttons from options', () => {
        const onClick = vi.fn();
        const toolbar = new Toolbar({
            buttons: [
                { title: "Save", onClick }
            ]
        }).init();

        const buttons = toolbar.domNode.querySelectorAll('.tool-button');
        expect(buttons.length).toBe(1);
        expect(buttons[0].textContent).toContain("Save");

        toolbar.destroy();
    });

    it('renders multiple buttons with separators', () => {
        const toolbar = new Toolbar({
            buttons: [
                { title: "A" },
                { separator: true, title: "B" },
                { separator: 'left', title: "C" },
                { separator: 'right', title: "D" },
                { separator: 'both', title: "E" }
            ]
        }).init();

        const groups = toolbar.domNode.querySelectorAll('.tool-group');
        expect(groups.length).toBeGreaterThanOrEqual(2);
        toolbar.destroy();
    });

    it('findButton finds by class name', () => {
        const toolbar = new Toolbar({
            buttons: [
                { title: "Save", cssClass: "save-button" }
            ]
        }).init();

        const btn = toolbar.findButton("save-button");
        expect(btn).toBeTruthy();
        expect(btn.getNode()).toBeTruthy();
        toolbar.destroy();
    });

    it('findButton handles leading dot', () => {
        const toolbar = new Toolbar({
            buttons: [
                { title: "Delete", cssClass: "delete-btn" }
            ]
        }).init();

        const btn = toolbar.findButton(".delete-btn");
        expect(btn.getNode()).toBeTruthy();
        toolbar.destroy();
    });

    it('updateInterface triggers updateInterface on all buttons', () => {
        const visibleFn = vi.fn(() => true);
        const toolbar = new Toolbar({
            buttons: [
                { title: "Dynamic", visible: visibleFn }
            ]
        }).init();

        toolbar.updateInterface();
        expect(visibleFn).toHaveBeenCalled();
        toolbar.destroy();
    });

    it('destroy cleans up event listeners', () => {
        const onClick = vi.fn();
        const toolbar = new Toolbar({
            buttons: [
                { title: "Click", onClick }
            ]
        }).init();

        const domNode = toolbar.domNode;
        toolbar.destroy();
        // After destroy, domNode should have been cleaned up
        expect((toolbar as any).domNode).toBeUndefined();
    });

    it('destroy with mouseTrap cleans up hotkey listeners', () => {
        const mousetrapDestroy = vi.fn();
        const mousetrapBind = vi.fn();

        (window as any).Mousetrap = vi.fn(() => ({
            bind: mousetrapBind,
            destroy: mousetrapDestroy,
            __listeners: [{ node: document.createElement("div"), type: "click", fn: vi.fn() }]
        }));

        const toolbar = new Toolbar({
            buttons: [
                { title: "HK", hotkey: "ctrl+s" }
            ]
        }).init();

        expect((toolbar as any).mouseTrap).toBeTruthy();
        toolbar.destroy();

        delete (window as any).Mousetrap;
    });

    it('destroy with mouseTrap that has no destroy method calls reset', () => {
        const mousetrapReset = vi.fn();

        (window as any).Mousetrap = vi.fn(() => ({
            bind: vi.fn(),
            reset: mousetrapReset,
            __listeners: null
        }));

        const toolbar = new Toolbar({
            buttons: [
                { title: "HK2", hotkey: "ctrl+a" }
            ]
        }).init();

        toolbar.destroy();

        delete (window as any).Mousetrap;
    });

    it('createButton handles separator right and both', () => {
        const toolbar = new Toolbar({}).init();

        const container = document.createElement("div");
        const tb: ToolButton = { title: "Test", separator: 'right' };
        toolbar.createButton(container, tb);
        expect(container.querySelector('.separator')).toBeTruthy();
        toolbar.destroy();
    });

    it('createButton handles ArrayLike container', () => {
        const toolbar = new Toolbar({}).init();

        const container = document.createElement("div");
        const tb: ToolButton = { title: "Test" };
        toolbar.createButton([container] as any, tb);
        expect(container.querySelector('.tool-button')).toBeTruthy();
        toolbar.destroy();
    });

    it('createButton with hotkey initializes mouseTrap if Mousetrap is available', () => {
        // Mock Mousetrap on window
        const mousetrapBind = vi.fn();
        const mousetrapReset = vi.fn();

        (window as any).Mousetrap = vi.fn(() => ({
            bind: mousetrapBind,
            reset: mousetrapReset,
            __listeners: null
        }));

        const toolbar = new Toolbar({
            buttons: [
                { title: "Hotkey", hotkey: "ctrl+s" }
            ]
        }).init();

        // Mousetrap should have been initialized
        expect((toolbar as any).mouseTrap).toBeTruthy();
        toolbar.destroy();

        delete (window as any).Mousetrap;
    });

    it('createButton with both separators creates separator before button', () => {
        const toolbar = new Toolbar({}).init();
        const container = document.createElement("div");
        const tb: ToolButton = { title: "BothSides", separator: 'both' };
        toolbar.createButton(container, tb);
        const separators = container.querySelectorAll('.separator');
        expect(separators.length).toBe(1);
        toolbar.destroy();
    });
});