import { Fluent } from "../../base";
import { Widget } from "../widgets/widget";
import { EditorUtils } from "./editorutils";

describe("EditorUtils.setReadOnly", () => {
    it("can set readonly for single element", () => {
        const el = document.createElement("div");
        EditorUtils.setReadOnly(el, true);
        expect(el.classList.contains("readonly")).toBeTruthy();
        expect(el.hasAttribute("readonly")).toBeTruthy();
    });

    it("can unset readonly for single element", () => {
        const el = document.createElement("div");
        el.classList.add("readonly");
        el.setAttribute("readonly", "readonly");
        EditorUtils.setReadOnly(el, false);
        expect(el.classList.contains("readonly")).toBeFalsy();
        expect(el.hasAttribute("readonly")).toBeFalsy();
    });

    it("can set readonly for Fluent element", () => {
        const el = document.createElement("div");
        EditorUtils.setReadOnly(Fluent(el), true);
        expect(el.classList.contains("readonly")).toBeTruthy();
        expect(el.hasAttribute("readonly")).toBeTruthy();
    });

    it("can unset readonly for Fluent element", () => {
        const el = document.createElement("div");
        el.classList.add("readonly");
        el.setAttribute("readonly", "readonly");
        EditorUtils.setReadOnly(Fluent(el), false);
        expect(el.classList.contains("readonly")).toBeFalsy();
        expect(el.hasAttribute("readonly")).toBeFalsy();
    });

    it("can unset readonly for Fluent element", () => {
        const el = document.createElement("div");
        el.classList.add("readonly");
        el.setAttribute("readonly", "readonly");
        EditorUtils.setReadOnly(Fluent(el), false);
        expect(el.classList.contains("readonly")).toBeFalsy();
        expect(el.hasAttribute("readonly")).toBeFalsy();
    });

    it("calls set_readOnly if available in the widget", () => {
        class MyWidget extends Widget {
            set_readOnly = jest.fn();
        }
        const widget = new MyWidget({});
        EditorUtils.setReadOnly(widget, true);
        expect(widget.set_readOnly).toHaveBeenCalledTimes(1);
        expect(widget.set_readOnly).toHaveBeenCalledWith(true);
        EditorUtils.setReadOnly(widget, false);
        expect(widget.set_readOnly).toHaveBeenCalledTimes(2);
        expect(widget.set_readOnly).toHaveBeenLastCalledWith(false);
    });

    it("skips null elements", () => {
        expect(() => EditorUtils.setReadOnly(null, true)).not.toThrow();
        expect(() => EditorUtils.setReadOnly([null], true)).not.toThrow();
        expect(() => EditorUtils.setReadOnly([document.createElement("div"), null, document.createElement("div")], true)).not.toThrow();
    });

    it("skips widgets with null domNode", () => {
        const widget = new Widget({});
        widget.destroy();
        expect(() => EditorUtils.setReadOnly(widget, true)).not.toThrow();
    });

    it("works with widgets that does not have a set_readOnly method", () => {
        class MyWidget extends Widget {
        }
        const widget = new MyWidget({});
        EditorUtils.setReadOnly(widget, true);
        expect(widget.domNode.classList.contains("readonly")).toBeTruthy();
        expect(widget.domNode.hasAttribute("readonly")).toBeTruthy();        
    });

    it("finds attached widget from the element", () => {
        class MyWidget extends Widget {
            set_readOnly = jest.fn();
        }
        const widget = new MyWidget({});
        EditorUtils.setReadOnly(widget.domNode, true);
        expect(widget.set_readOnly).toHaveBeenCalledTimes(1);
        expect(widget.set_readOnly).toHaveBeenCalledWith(true);
        EditorUtils.setReadOnly(widget, false);
        expect(widget.set_readOnly).toHaveBeenCalledTimes(2);
        expect(widget.set_readOnly).toHaveBeenLastCalledWith(false);
    });
});