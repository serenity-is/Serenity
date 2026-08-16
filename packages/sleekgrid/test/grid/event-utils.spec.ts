import { EventEmitter } from "../../src/core/event";
import { addListener, removeListener, triggerGridEvent } from "../../src/grid/event-utils";

describe('triggerGridEvent', () => {
    it('injects the grid into args and notifies subscribers with the grid as scope', () => {
        const evt = new EventEmitter<any, any>();
        const grid = { id: "grid1" } as any;
        const received: any[] = [];
        evt.subscribe((e, args) => { received.push({ e, args }); });

        const result = triggerGridEvent.call(grid, evt, { row: 1, cell: 2 } as any);

        expect(received.length).toBe(1);
        expect(received[0].args.grid).toBe(grid);
        expect(received[0].args.row).toBe(1);
        expect(received[0].args.cell).toBe(2);
        expect(result.args.grid).toBe(grid);
        expect(result.getReturnValue()).toBeUndefined();
    });

    it('creates an args object when none are passed', () => {
        const evt = new EventEmitter<any, any>();
        const grid = { id: "g" } as any;
        let gotArgs: any;
        evt.subscribe((_e, args) => (gotArgs = args));
        triggerGridEvent.call(grid, evt);
        expect(gotArgs).toBeTruthy();
        expect(gotArgs.grid).toBe(grid);
    });

    it('passes the native event object through to subscribers', () => {
        const evt = new EventEmitter<any, any>();
        const grid = {} as any;
        const e = { type: "click" };
        let gotEvent: any;
        evt.subscribe((ev, _args) => { gotEvent = ev; });
        triggerGridEvent.call(grid, evt, undefined, e as any);
        expect(gotEvent.nativeEvent).toBe(e);
    });
});

describe('addListener', () => {
    function makeJQuery() {
        const jq = { on: vi.fn((_type: any, _handler: any) => jq), off: vi.fn(() => jq) };
        const $ = vi.fn(() => jq);
        return { jq, $ };
    }

    it('uses jQuery.on with the uid namespaced type when no options are given', () => {
        const { jq, $ } = makeJQuery();
        const el = document.createElement("div");
        const listener = vi.fn();
        const self = { jQuery: $, eventDisposer: new AbortController(), uid: "uid1" } as any;
        addListener.call(self, el, "click", listener);
        expect($).toHaveBeenCalledWith(el);
        expect(jq.on).toHaveBeenCalledWith("click.uid1", listener);
    });

    it('falls back to native addEventListener with capture when requested', () => {
        const { jq, $ } = makeJQuery();
        const el = document.createElement("div");
        const listener = vi.fn();
        const addSpy = vi.spyOn(el, "addEventListener");
        const self = { jQuery: $, eventDisposer: new AbortController(), uid: "uid1" } as any;
        addListener.call(self, el, "click", listener, { capture: true });
        expect(jq.on).not.toHaveBeenCalled();
        expect(addSpy).toHaveBeenCalledWith("click", listener, expect.objectContaining({ capture: true }));
    });

    it('falls back to native addEventListener when a signal is passed', () => {
        const el = document.createElement("div");
        const listener = vi.fn();
        const addSpy = vi.spyOn(el, "addEventListener");
        const self = { jQuery: () => { throw new Error("jQuery should not be used"); }, eventDisposer: new AbortController(), uid: "u" } as any;
        const signal = new AbortController().signal;
        addListener.call(self, el, "click", listener, { signal });
        expect(addSpy).toHaveBeenCalledWith("click", listener, expect.objectContaining({ signal }));
    });

    it('falls back to native addEventListener with passive when requested', () => {
        const el = document.createElement("div");
        const listener = vi.fn();
        const addSpy = vi.spyOn(el, "addEventListener");
        const self = { jQuery: () => { throw new Error("no"); }, eventDisposer: new AbortController(), uid: "u" } as any;
        addListener.call(self, el, "click", listener, { passive: true });
        expect(addSpy).toHaveBeenCalledWith("click", listener, expect.objectContaining({ passive: true }));
    });

    it('falls back to native addEventListener when jQuery is missing', () => {
        const el = document.createElement("div");
        const listener = vi.fn();
        const addSpy = vi.spyOn(el, "addEventListener");
        const self = { eventDisposer: new AbortController(), uid: "u" } as any;
        addListener.call(self, el, "click", listener);
        expect(addSpy).toHaveBeenCalledWith("click", listener, expect.objectContaining({ signal: self.eventDisposer.signal }));
    });

    it('dispatches to the listener through the native path', () => {
        const el = document.createElement("div");
        const listener = vi.fn();
        const self = { eventDisposer: new AbortController(), uid: "u" } as any;
        addListener.call(self, el, "click", listener);
        el.dispatchEvent(new MouseEvent("click"));
        expect(listener).toHaveBeenCalledTimes(1);
    });

    it('dispatches to the listener through the jQuery path', () => {
        const { jq, $ } = makeJQuery();
        const el = document.createElement("div");
        const listener = vi.fn();
        const self = { jQuery: $, eventDisposer: new AbortController(), uid: "u" } as any;
        addListener.call(self, el, "click", listener);
        const handler = jq.on.mock.calls[0][1];
        handler.call(el, new MouseEvent("click"));
        expect(listener).toHaveBeenCalledTimes(1);
    });
});

describe('removeListener', () => {
    it('uses jQuery.off with the uid namespaced type when jQuery is available', () => {
        const jq = { off: vi.fn() };
        const $ = vi.fn(() => jq);
        const el = document.createElement("div");
        const listener = vi.fn();
        const self = { jQuery: $, uid: "uid1" } as any;
        removeListener.call(self, el, "click", listener);
        expect($).toHaveBeenCalledWith(el);
        expect(jq.off).toHaveBeenCalledWith("click.uid1", listener);
    });

    it('uses native removeEventListener when jQuery is missing', () => {
        const el = document.createElement("div");
        const listener = vi.fn();
        const removeSpy = vi.spyOn(el, "removeEventListener");
        const self = { uid: "uid1" } as any;
        removeListener.call(self, el, "click", listener);
        expect(removeSpy).toHaveBeenCalledWith("click", listener, false);
    });

    it('passes capture through to native removeEventListener', () => {
        const el = document.createElement("div");
        const listener = vi.fn();
        const removeSpy = vi.spyOn(el, "removeEventListener");
        const self = { uid: "uid1" } as any;
        removeListener.call(self, el, "click", listener, { capture: true });
        expect(removeSpy).toHaveBeenCalledWith("click", listener, true);
    });

    it('actually removes the listener via the native path', () => {
        const el = document.createElement("div");
        const listener = vi.fn();
        el.addEventListener("click", listener);
        removeListener.call({ uid: "u" } as any, el, "click", listener);
        el.dispatchEvent(new MouseEvent("click"));
        expect(listener).not.toHaveBeenCalled();
    });
});
