import { IQuickFiltering } from "./iquickfiltering";

describe("IQuickFiltering", () => {
    it("is defined and has typeInfo", () => {
        expect(IQuickFiltering).toBeDefined();
        expect(IQuickFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });
});
