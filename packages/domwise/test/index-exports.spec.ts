import { describe, expect, it } from "vitest";
import { isReadonlySignal, isWritableSignal, Show } from "../src/index";

describe("public exports", () => {
    it("exports Show", () => {
        expect(typeof Show).toBe("function");
    });

    it("exports signal type guards", () => {
        expect(typeof isWritableSignal).toBe("function");
        expect(typeof isReadonlySignal).toBe("function");
    });
});
