import { IFiltering } from "./ifiltering";

describe("IFiltering", () => {
    it("is defined and has typeInfo", () => {
        expect(IFiltering).toBeDefined();
        expect(IFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });
});
