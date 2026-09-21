import { describe, expect, it } from "vitest";

describe("jsx factory", () => {
    it("applies a default whose key collides with Object.prototype", () => {
        function Comp(props: any) {
            return <div>{String(props.toString)}</div>;
        }
        (Comp as any).defaultProps = { toString: "X" };
        const node = <Comp /> as unknown as HTMLElement;
        expect(node.textContent).toBe("X");
    });
});
