import { describe, expect, it } from "vitest";
import { FormatterBase } from "./formatterbase";

describe("FormatterBase", () => {
    it("registers type info for a formatter subclass", () => {
        class MyFormatter extends FormatterBase {
            static override[Symbol.typeInfo] = this.registerFormatter("My.Formatter");
            format() { return "formatted"; }
        }
        expect(MyFormatter[Symbol.typeInfo]).toBeTruthy();
        const formatter = new MyFormatter();
        expect(formatter.format()).toBe("formatted");
    });

    it("registers type info with interfaces", () => {
        class AttributedFormatter extends FormatterBase {
            static override[Symbol.typeInfo] = this.registerFormatter("My.Attributed");
            format() { return ""; }
        }
        expect(AttributedFormatter[Symbol.typeInfo]).toBeTruthy();
    });

    it("throws when a formatter already has its own type info", () => {
        class HelperFormatter extends FormatterBase {
            format() { return ""; }
            static forceRegister() {
                return this.registerFormatter("Helper.Formatter");
            }
        }
        expect(HelperFormatter.forceRegister()).toBeTruthy();
        expect(() => HelperFormatter.forceRegister()).toThrow(/already has a typeInfo/);
    });
});
